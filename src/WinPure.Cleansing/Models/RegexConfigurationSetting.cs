namespace WinPure.Cleansing.Models
{

    /// <summary>
    /// Configure regular expression cleansing settings 
    /// </summary>
    [Serializable]
    public class RegexConfigurationSetting
    {
        public int Id { get; set; }
        public string Expression { get; set; }
        public string Replacement { get; set; }
        public string Description { get; set; }
    }
}
