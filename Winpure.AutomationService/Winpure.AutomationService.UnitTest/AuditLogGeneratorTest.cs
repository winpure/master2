using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WinPure.Cleansing.Models;
using WinPure.Common.Models;
using WinPure.DataService.AuditLogs;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Winpure.AutomationService.UnitTest
{
    [TestFixture]
    internal class AuditLogGeneratorTest
    {
        private Mock<IAuditLogService> _mockAuditLogService;

        [SetUp]
        public void Setup()
        {
            _mockAuditLogService = new Mock<IAuditLogService>();
            _mockAuditLogService.Setup(x => x.GetNextLogId()).Returns(1);
        }

        [Test]
        public void AuditLogsShouldBeAsExpected()
        {
            var srcJson = File.ReadAllText("Files\\OriginalTable.json");
            var updatedJson = File.ReadAllText("Files\\UpdatedTable.json");
            var settingJson = File.ReadAllText("Files\\CleanSettings.json");
            var logsJson = File.ReadAllText("Files\\AuditLogResults.json");
            var generator = new AuditLogGenerator(_mockAuditLogService.Object);

            var src = JsonConvert.DeserializeObject<DataTable>(srcJson);
            var upd = JsonConvert.DeserializeObject<DataTable>(updatedJson);
            var settings = JsonConvert.DeserializeObject<WinPureCleanSettings>(settingJson);
            var auditLog = JsonConvert.DeserializeObject<List<AuditLog>>(logsJson);

            var result = generator.GetAuditLogs("SampleFile1", src, upd, settings);

            var cmp = new AuditLogComparerIgnoreIdTime();
            var same = auditLog.Count == result.Count
                        && !auditLog.Except(result, cmp).Any()
                        && !result.Except(auditLog, cmp).Any();

            Assert.IsTrue(same);
        }
    }

    sealed class AuditLogComparerIgnoreIdTime : IEqualityComparer<AuditLog>
    {
        public bool Equals(AuditLog x, AuditLog y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            // Ignore Id and Timestamp
            return string.Equals(x.SourceName, y.SourceName, StringComparison.Ordinal) &&
                   string.Equals(x.RecordId, y.RecordId, StringComparison.Ordinal) &&
                   string.Equals(x.AffectedField, y.AffectedField, StringComparison.Ordinal) &&
                   string.Equals(x.OriginalValue, y.OriginalValue, StringComparison.Ordinal) &&
                   string.Equals(x.NewValue, y.NewValue, StringComparison.Ordinal) &&
                   string.Equals(x.Module, y.Module, StringComparison.Ordinal) &&
                   string.Equals(x.Reason, y.Reason, StringComparison.Ordinal);
        }

        public int GetHashCode(AuditLog obj)
        {
            if (obj is null) return 0;

            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (obj.SourceName?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.RecordId?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.AffectedField?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.OriginalValue?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.NewValue?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.Module?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.Reason?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
