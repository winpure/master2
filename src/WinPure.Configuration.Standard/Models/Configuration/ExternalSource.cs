namespace WinPure.Configuration.Models.Configuration
{
    internal class ExternalSource
    {
        public ExternalSourceTypes Source { get; set; }
        public ExternalSourceGroup Group { get; set; }
        public bool CanImport { get; set; }
        public bool CanExport { get; set; }
        public bool Favorite { get; set; }
    }
}
