using NLog;
using NLog.Config;
using NLog.Targets;
using System.Collections.Generic;
using System.IO;

namespace WinPure.Common.DependencyInjection;

internal partial class WinPureCommonDependency
{
    private class WpNLogger : IWpLogger
    {
        private readonly NLog.Logger _logger;

        public string ReportPath
        {
            get
            {
                var target = LogManager.Configuration.AllTargets.Where(t => t is FileTarget)
                    .Where(x => x.Name.Contains("InfoFile")).Cast<FileTarget>().FirstOrDefault();

                return target != null
                    ? Path.GetDirectoryName(target.FileName.ToString())
                    : string.Empty;
            }
        }

        public WpNLogger()
        {
            _logger = LogManager.GetLogger("WinPure");
            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }
        }

        public void SetReportPath(string newPath)
        {
            var path = Path.GetDirectoryName(newPath);
            var fileName = Path.GetFileNameWithoutExtension(newPath);
            foreach (FileTarget target3 in LogManager.Configuration.AllTargets.Where(t => t is FileTarget)
                         .Where(x => x.Name.Contains("InfoFile")).Cast<FileTarget>())
            {
                target3.FileName = newPath;
                target3.ArchiveFileName = $"{path}\\archive\\{fileName}_{{#}}.log";
            }

            LogManager.ReconfigExistingLoggers();
        }

        public void Trace(string message, params object[] args)
        {
            args = AddTrace(args);
            _logger.Trace(Format(message, args));
        }
        
        public void Debug(string message, params object[] args)
        {
            args = AddTrace(args);
            _logger.Debug(Format(message, args));
        }

        public void Information(string message, params object[] args)
        {
            _logger.Info(Format(message, args));
        }
        
        public void Warning(string message, params object[] args)
        {
            _logger.Warn(Format(message, args));
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(Format(message, args));
        }

        public void Fatal(string message, params object[] args)
        {
            args = AddTrace(args);
            _logger.Fatal(Format(message, args));
        }

        private static string Format(string message, object[] args)
        {
            if (args == null || !args.Any())
                return message;

            //return message != null
            //    ? String.Format(message, args)
            //    : null;
            var param = new List<object>() { message ?? "" };
            param.AddRange(args);
            return string.Join(" | ", param);

        }

        private static object[] AddTrace(object[] args)
        {
            if (args == null)
            {
                return new object[] { Environment.StackTrace };
            }

            var t = args.ToList();
            t.Add(Environment.StackTrace);
            return t.ToArray();
        }
    }
}