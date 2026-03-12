using System.Globalization;

namespace WinPure.Configuration.Service
{
    internal interface ILanguageService
    {
        void Initiate(ProgramType programType);
        LanguageEnum ProgramLanguage { get; }
        Dictionary<string, LanguageEnum> GetLanguageList();
        void SetProgramLanguage(CultureInfo currentCulture, LanguageEnum? lang = null);
    }
}