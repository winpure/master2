namespace WinPure.Configuration.Helper;

    internal static partial class DatabaseInitiator
    {
        static ProperCaseSettingEntity GetProperCaseDelimitersSettings => new ProperCaseSettingEntity
        {
            Name = ProperCaseNameHelper.Delimiters,
            Value = " '-."
        };
        static ProperCaseSettingEntity GetProperCaseExceptionsSettings => new ProperCaseSettingEntity
        {
            Name = ProperCaseNameHelper.Exceptions,
            Value = "[\"de\",\"la\",\"le\",\"von\"]"
        };
        static ProperCaseSettingEntity GetProperCasePrefixSettings => new ProperCaseSettingEntity
        {
            Name = ProperCaseNameHelper.Prefix,
            Value = "[\"Mc\",\"Mac\",\"O'\",\"Di\"]"
        };
}