namespace WinPure.Configuration.Models.Database
{
    public class DictionaryNameEntity
    {
        public int Id { get; set; }
        public string DictionaryName { get; set; }
        public List<DictionaryDataEntity> DictionaryData { get; set; }
    }
}