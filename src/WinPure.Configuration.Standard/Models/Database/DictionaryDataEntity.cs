using WinPure.Configuration.Models.Dictionary;

namespace WinPure.Configuration.Models.Database
{
    public class DictionaryDataEntity
    {
        public int Id { get; set; }
        public int DictionaryId { get; set; }
        public string SearchValue { get; set; }
        public string ReplaceValue { get; set; }
        public DictionaryNameEntity Dictionary { get; set; }

        public DictionaryData ToDictionaryData()
        {
            return new DictionaryData
            {
                Id = Id,
                DictionaryId = DictionaryId,
                ReplaceValue = ReplaceValue,
                SearchValue = SearchValue
            };
        }
    }
}
