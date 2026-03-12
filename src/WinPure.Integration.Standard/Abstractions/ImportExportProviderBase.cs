using WinPure.Common.Abstractions;
using WinPure.Configuration.DependencyInjection;
using WinPure.Integration.Enums;

namespace WinPure.Integration.Abstractions;

internal abstract class ImportExportProviderBase : WinPureNotification, IImportExportProvider, IDisposable
{
    internal IWpLogger _logger;

    public OperationType OperationType { get; private set; }
    public ExternalSourceTypes SourceType { get; private set; }

    public string DisplayName { get; set; }

    protected ImportExportProviderBase(OperationType operation, ExternalSourceTypes source)
    {
        OperationType = operation;
        SourceType = source;
        _logger = WinPureConfigurationDependency.Resolve<IWpLogger>();
    }

    public abstract void Initialize(object parameters);

    public virtual void Dispose()
    {
    }
}