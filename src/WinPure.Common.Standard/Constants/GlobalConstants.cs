using WinPure.Common.Exceptions;

namespace WinPure.Common.Constants;

internal static class GlobalConstants
{
    internal static int MatchRecordsForDemoVersion = 500000;
    internal static int ErRecordsForDemoVersion = 500000;
    internal static int AddressVerificationRecordsForDemoVersion = 100;

    private static int DemoExportRecordCount = 100;

    private const int IMPORT_API_DEMO_LIMIT = 1000;
    private const int IMPORT_EXPORT_API_LIMIT = 0;

    private const int IMPORT_ENTERPRISE_DEMO_LIMIT = 100000;
    private const int IMPORT_EXPORT_ENTERPRISE_LIMIT = 0;
    private const int IMPORT_EXPORT_ENTERPRISE_LITE_LIMIT = 500000;

    private const int IMPORT_EXPORT_USADDRESSENTERPRISE_LIMIT = 0;

    private const int IMPORT_EXPORT_LITE_LIMIT = 100000;

    private const int IMPORT_EXPORT_BIZ_LIMIT = 250000;

    private const int IMPORT_EXPORT_FREE_LIMIT = 10000;

    internal static int ImportRowLimitForProgram(ProgramType programType, bool isDemo)
    {
        switch (programType)
        {
            case ProgramType.Api:
                return isDemo ? IMPORT_API_DEMO_LIMIT : IMPORT_EXPORT_API_LIMIT;
            case ProgramType.CamEnt:
                return isDemo ? IMPORT_ENTERPRISE_DEMO_LIMIT : IMPORT_EXPORT_ENTERPRISE_LIMIT;
            case ProgramType.CamEntLite:
                return isDemo ? IMPORT_ENTERPRISE_DEMO_LIMIT : IMPORT_EXPORT_ENTERPRISE_LITE_LIMIT;
            case ProgramType.CamEntAd:
                return isDemo ? IMPORT_ENTERPRISE_DEMO_LIMIT : IMPORT_EXPORT_USADDRESSENTERPRISE_LIMIT;
            case ProgramType.CamLte:
                return isDemo ? IMPORT_ENTERPRISE_DEMO_LIMIT : IMPORT_EXPORT_LITE_LIMIT;
            case ProgramType.CamBiz:
                return isDemo ? IMPORT_ENTERPRISE_DEMO_LIMIT : IMPORT_EXPORT_BIZ_LIMIT;
            case ProgramType.CamFree:
                return IMPORT_EXPORT_FREE_LIMIT;
            default:
                throw new WinPureArgumentException($"Program type {programType} not found");
        }
    }

    internal static int MatchResultExportLimitForProgram(ProgramType programType, bool isDemo)
    {
        var importLimit = ImportRowLimitForProgram(programType, isDemo);
        var tableCount = AllowedTableCount(programType, isDemo);

        return tableCount * importLimit;
    }

    internal static int ExportRowLimitForProgram(ProgramType programType, bool isDemo)
    {
        if (isDemo && programType != ProgramType.CamFree)
        {
            return DemoExportRecordCount;
        }

        switch (programType)
        {
            case ProgramType.Api:
                return IMPORT_EXPORT_API_LIMIT;
            case ProgramType.CamEnt:
                return IMPORT_EXPORT_ENTERPRISE_LIMIT;
            case ProgramType.CamEntLite:
                return IMPORT_EXPORT_ENTERPRISE_LITE_LIMIT;
            case ProgramType.CamEntAd:
                return IMPORT_EXPORT_USADDRESSENTERPRISE_LIMIT;
            case ProgramType.CamLte:
                return IMPORT_EXPORT_LITE_LIMIT;
            case ProgramType.CamBiz:
                return IMPORT_EXPORT_BIZ_LIMIT;
            case ProgramType.CamFree:
                return IMPORT_EXPORT_FREE_LIMIT;
            default:
                throw new WinPureArgumentException($"Program type {programType} not found");
        }
    }

    internal static int AllowedTableCount(ProgramType programType, bool isDemo)
    {
        if (isDemo)
        {
            return 5;
        }

        switch (programType)
        {
            case ProgramType.CamFree:
            case ProgramType.CamLte:
                return 2;
            case ProgramType.CamBiz:
                return 4;
            case ProgramType.CamEntLite:
                return 6;
            case ProgramType.CamEnt:
            case ProgramType.CamEntAd:
                return 0;
            default:
                return 1;
        }
    }
}