using WinPure.Common.Abstractions;
using WinPure.Integration.Enums;

namespace WinPure.Integration.Abstractions;

internal interface IImportExportProvider : IWinPureNotification
{
    OperationType OperationType { get; }
    ExternalSourceTypes SourceType { get; }
    string DisplayName { get; set; }
    void Initialize(object parameters);
}