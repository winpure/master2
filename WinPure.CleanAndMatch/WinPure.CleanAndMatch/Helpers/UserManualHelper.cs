namespace WinPure.CleanAndMatch.Helpers;

public class UserManualHelper
{
    public static void OpenHelpPage(HelpPageChapter chapter)
    {
        string url = "";
        switch (chapter)
        {
            case HelpPageChapter.MatchingRecordsOptions:
                url = "https://winpure.com/HelpManuals/CAMv11/MatchingRecordsOptions.html";
                break;
            case HelpPageChapter.Cam:
                url = "https://winpure.com/HelpManuals/CAMv11/CAM.html";
                break;
            case HelpPageChapter.Schedule:
                url = "https://winpure.acuityscheduling.com/schedule.php";
                break;
            case HelpPageChapter.CamStarted:
                url = "https://www.youtube.com/playlist?list=PLVgYe7zT_iCiEppoLU61CZftKuFm2A-KN";
                break;
            case HelpPageChapter.WordManager:
                url = "https://winpure.com/HelpManuals/CAMv11/WordManager.html";
                break;
            case HelpPageChapter.Clean:
                url = "https://winpure.com/HelpManuals/CAMv11/Clean.html";
                break;
            case HelpPageChapter.DataProfilingStatistics:
                url = "https://winpure.com/HelpManuals/CAMv11/DataProfilingStatistics.html";
                break;
            case HelpPageChapter.DataPreview:
                url = "https://winpure.com/HelpManuals/CAMv11/DataPreview.html";
                break;
            case HelpPageChapter.CamStep1:
                url = "https://winpure.com/HelpManuals/CAMv11/Step1.html";
                break;
            case HelpPageChapter.CamStep2:
                url = "https://winpure.com/HelpManuals/CAMv11/Step2.html";
                break;
            case HelpPageChapter.CamStep3:
                url = "https://winpure.com/HelpManuals/CAMv11/Step3.html";
                break;
            case HelpPageChapter.MatchAcrossTablesOnly:
                url = "https://winpure.com/HelpManuals/CAMv11/MatchAcrossTablesonly.html";
                break;
            case HelpPageChapter.CamReport:
                url = "https://winpure.com/HelpManuals/CAMv11/Report.html";
                break;
            case HelpPageChapter.AdvancedOptions:
                url = "https://winpure.com/HelpManuals/CAMv11/AdvancedOptions.html";
                break;
            case HelpPageChapter.Verify:
                url = "https://winpure.com/HelpManuals/CAMv11/VerifyOptions.html";
                break;
            case HelpPageChapter.CamEntStarted:
                url = "https://www.youtube.com/playlist?list=PLVgYe7zT_iCiEppoLU61CZftKuFm2A-KN";
                break;
            case HelpPageChapter.GeoCodes:
                url = "https://winpure.com/HelpManuals/CAMv11/GeoCode.html";
                break;
            case HelpPageChapter.Verification:
                url = "https://winpure.com/HelpManuals/CAMv11/VerifyInternationalAddressVerifi.html";
                break;
            case HelpPageChapter.MergeRecords:
                url = "https://winpure.com/HelpManuals/CAMv11/MergeRecords.html";
                break;
            case HelpPageChapter.SetMasterRecords:
                url = "https://winpure.com/HelpManuals/CAMv11/SetMasterRecords.html";
                break;
            case HelpPageChapter.UpdateOverwrite:
                url = "https://winpure.com/HelpManuals/CAMv11/UpdateOverwrite.html";
                break;
            case HelpPageChapter.Delete:
                url = "https://winpure.com/HelpManuals/CAMv11/Delete.html";
                break;

            case HelpPageChapter.Thoroughfare:
                url = "https://winpure.com/HelpManuals/CAMv11/ThoroughfareLevelAddressTags.html";
                break;

            case HelpPageChapter.PremiseLevel:
                url = "https://winpure.com/HelpManuals/CAMv11/PremiseLevelTags.html";
                break;

            case HelpPageChapter.GeoData:
                url = "https://winpure.com/HelpManuals/CAMv11/GeoDataTags.html";
                break;


            case HelpPageChapter.Mapping:
                url = "https://winpure.com/HelpManuals/CAMv11/MappingTags.html";
                break;


            case HelpPageChapter.CorrectionsCodes:
                url = "https://winpure.com/HelpManuals/CAMv11/CorrectionsCodes.html";
                break;

            case HelpPageChapter.UKVerify:
                url = "https://winpure.com/HelpManuals/CAMv11/VerifyUKAddressVerification.html";
                break;


            case HelpPageChapter.SelectFieldsToImport:
                url = "https://winpure.com/HelpManuals/CAMv11/SelectFieldstoImport.html";
                break;

            case HelpPageChapter.Automation:
                url = "https://winpure.com/HelpManuals/CAMv11/Automation.html";
                break;

            case HelpPageChapter.ViewingOptions:
                url = "https://winpure.com/HelpManuals/CAMv11/ViewingOptions.html";
                break;

            case HelpPageChapter.AVDemo:
                url = "https://winpure.com/HelpManuals/CAMv11/AVDemo.html";
                break;

            case HelpPageChapter.AVOverview:
                url = "https://winpure.com/HelpManuals/CAMv11/VerifyInternationalAddressVerifi.html";
                break;

            case HelpPageChapter.AVCodes:
                url = "https://winpure.com/HelpManuals/CAMv11/AddressVerificationCodes.html";
                break;

            case HelpPageChapter.AVGeocoding:
                url = "https://winpure.com/HelpManuals/CAMv11/GeoCode.html";
                break;

            case HelpPageChapter.AVCountryUpdates:
                url = "https://winpure.com/HelpManuals/CAMv11/CountryDataUpdatesFrequencySched.html";
                break;

            case HelpPageChapter.AVCountry:
                url = "https://winpure.com/HelpManuals/CAMv11/Country.html";
                break;

            case HelpPageChapter.CASS:
                url = "https://winpure.com/HelpManuals/CAMv11/CASSFields.html";
                break;

            case HelpPageChapter.AMAS:
                url = "https://winpure.com/HelpManuals/CAMv11/AMASFields.html";
                break;

            case HelpPageChapter.SERP:
                url = "https://winpure.com/HelpManuals/CAMv11/SERPFields.html";
                break;

            case HelpPageChapter.NewDataSources:
                url = "https://winpure.com/request_new_data_connector/";
                break;

            case HelpPageChapter.Settings:
                url = "https://winpure.com/HelpManuals/CAMv11/Settings.html";
                break;

            case HelpPageChapter.KnowledgeBase:
                url = "https://winpure.com/HelpManuals/CAMv11/KnowledgeBaseLibrary.html";
                break;

            case HelpPageChapter.ProperCaseSettings:
                url = "https://winpure.com/HelpManuals/CAMv11/ProperCaseSettings.html";
                break;

            case HelpPageChapter.ImportSnowflake:
                url = "https://winpure.com/HelpManuals/CAMv11/ImportfromSnowflake.html";
                break;

            case HelpPageChapter.ExportSnowflake:
                url = "https://winpure.com/HelpManuals/CAMv11/ToSnowflake.html";
                break;

            case HelpPageChapter.ImportExcel:
                url = "https://winpure.com/HelpManuals/CAMv11/FromMSExcel.html";
                break;

            case HelpPageChapter.ERSelectTables:
                url = "https://winpure.com/HelpManuals/CAMv11/SelectTables.html";
                break;

            case HelpPageChapter.ERDefineColumnTypes:
                url = "https://winpure.com/HelpManuals/CAMv11/DefineColumnTypes.html";
                break;

            case HelpPageChapter.ERAutoMapColumns:
                url = "https://winpure.com/HelpManuals/CAMv11/AutoMapColumn.html";
                break;

            case HelpPageChapter.ERReviewAnalyse:
                url = "https://winpure.com/HelpManuals/CAMv11/ReviewAnalyse.html";
                break;

            case HelpPageChapter.ERSystemStatus:
                url = "https://winpure.com/HelpManuals/CAMv11/SystemStatus.html";
                break;

            case HelpPageChapter.ERStep1:
                url = "https://winpure.com/HelpManuals/CAMv11/SelectTables.html";
                break;

            case HelpPageChapter.ERStep2:
                url = "https://winpure.com/HelpManuals/CAMv11/DefineColumnTypes.html";
                break;

            case HelpPageChapter.PatternManager:
                url = "https://winpure.com/HelpManuals/CAMv11/PatternManager.html";
                break;

            case HelpPageChapter.ERAutoMapConfig:
                url = "https://winpure.com/HelpManuals/CAMv11/AutoMapConfiguration.html";
                break;

            case HelpPageChapter.AuditLogs:
                url = "https://winpure.com/HelpManuals/CAMv11/AuditLogs.html";
                break;

            case HelpPageChapter.RegexManager:
                url = "https://winpure.com/HelpManuals/CAMv11/RegexManager.html";
                break;


            case HelpPageChapter.ERConfiguration:
                url = "https://winpure.com/HelpManuals/CAMv10/ERConfiguration.html";
                break;

            case HelpPageChapter.ERResults:
                url = "https://winpure.com/HelpManuals/CAMv10/ERResults.html";
                break;

            case HelpPageChapter.ReverseGeocode:
                url = "";
                break;
            default:
                url = "";
                break;
        }

        if (!string.IsNullOrEmpty(url)) System.Diagnostics.Process.Start(url);
    }

    internal static void OpenGetStartedHelp(ProgramType pType)
    {
        if (ProgramTypeHelper.EnterpriseTypes.Contains(pType))
        {
            OpenHelpPage(HelpPageChapter.CamEntStarted);
        }
        else if (ProgramTypeHelper.CamTypes.Contains(pType))
        {
            OpenHelpPage(HelpPageChapter.CamStarted);
        }
    }
}