using WinPure.Configuration.Models.Dictionary;

namespace WinPure.Configuration.Repository
{
    internal interface IDictionaryRepository
    {
        Task<List<DictionaryHeader>> GetDictionaryList();
        Task<List<DictionaryData>> GetDictionaryData(string dictionaryName);
        Task<List<DictionaryData>> GetDictionaryData(int dictionaryId);
        Task<List<DictionaryHeader>> AddNewDictionary(DictionaryNameEntity dictionaryEntity);
        Task DeleteDictionary(string name);
        Task AddNewDictionaryData(string search, string replace, int dictId);
        Task<DictionaryData> GetDictionaryDataValue(string search, int dictId);
        Task<List<DictionaryData>> DeleteDictionaryData(int dataId, int dictId);
        Task UpdateDictionary(string newName, int dictId);
        Task UpdateDictionaryData(string newSearch, string newReplace, int dataId, int dictId);
        Task<Dictionary<string, string>> GetDictionary(string dictionaryName);
    }
}