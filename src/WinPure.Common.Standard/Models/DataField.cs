namespace WinPure.Common.Models
{
    [Serializable]
    public class DataField
    {
        public int Id { get; set; }
        public string DatabaseName { get; set; }
        public string DisplayName { get; set; }
        public string FieldType { get; set; }
        public string Pattern { get; set; }
        public string AiType { get; set; }
    }
}