using System.Globalization;
using System.Threading;
using WinPure.Licensing.Services;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class LanguageService : ILanguageService
    {
        private readonly IValueStore _valueStore;
        private string _currentProgramRegistryKey;

        public LanguageService(IValueStore valueStore)
        {
            _valueStore = valueStore;
        }

        public void Initiate(ProgramType programType)
        {
            _currentProgramRegistryKey = programType.ToString().ToUpper();
        }

        public LanguageEnum ProgramLanguage
        {
            get
            {
                var regValue = _valueStore.GetValue(_currentProgramRegistryKey, "Language", "en");

                return Enum.TryParse(regValue, out LanguageEnum language) ? language : LanguageEnum.en;
            }

        }

        public Dictionary<string, LanguageEnum> GetLanguageList()
        {
            return EnumExtension.GetDisplayNameDictionary<LanguageEnum>();
        }

        public void SetProgramLanguage(CultureInfo currentCulture, LanguageEnum? lang = null)
        {
            // https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
            if (lang == null)
            {
                lang = ProgramLanguage;
            }
            else
            {
                _valueStore.SetValue(_currentProgramRegistryKey, "Language", lang.ToString());
            }

            string ciName;
            switch (lang)
            {
                case LanguageEnum.en:
                    ciName = "en-US";
                    break;
                case LanguageEnum.bp:
                    ciName = "pt-BR";
                    break;
                case LanguageEnum.nl:
                    ciName = "nl-NL";
                    break;
                case LanguageEnum.fr:
                    ciName = "fr-FR";
                    break;
                case LanguageEnum.de:
                    ciName = "de-DE";
                    break;
                case LanguageEnum.it:
                    ciName = "it-IT";
                    break;
                case LanguageEnum.pt:
                    ciName = "pt-PT";
                    break;
                case LanguageEnum.ru:
                    ciName = "ru-RU";
                    break;
                case LanguageEnum.sp:
                    ciName = "es-ES";
                    break;
                case null:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Thread.CurrentThread.CurrentCulture = new CultureInfo(ciName);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ciName);
            Thread.CurrentThread.CurrentUICulture.DateTimeFormat = currentCulture.DateTimeFormat;
            Thread.CurrentThread.CurrentUICulture.NumberFormat = currentCulture.NumberFormat;
        }
    }
}