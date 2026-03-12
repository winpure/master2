using System.Collections.Generic;

namespace WinPure.Common.Helpers
{
    internal static class ProgramTypeHelper
    {
        public static List<ProgramType> EnterpriseTypes = new() { ProgramType.CamEnt, ProgramType.CamEntLite, ProgramType.CamEntAd };
        public static List<ProgramType> CamTypes = new() { ProgramType.CamFree, ProgramType.CamLte };
        public static List<ProgramType> AutomationPrograms = new() { ProgramType.CamEnt, ProgramType.CamEntLite, ProgramType.CamEntAd };

        internal static string GetProgramName(ProgramType programType)
        {
            switch (programType)
            {
                case ProgramType.Api:
                    return "API";
                case ProgramType.CamEnt:
                case ProgramType.CamEntLite:
                case ProgramType.CamEntAd:
                    return "Enterprise";
                case ProgramType.CamLte:
                    return "Lite";
                case ProgramType.CamBiz:
                    return "Business";
                case ProgramType.CamFree:
                    return "Community";
                default:
                    return "";
            }
        }
    }
}