namespace WinPure.Configuration.Models.Configuration
{
    [Serializable]
    internal class DatabaseSetting
    {
        public bool UseRelativePath { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
    }
}