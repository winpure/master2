using WinPure.Configuration.Models.Dictionary;

namespace WinPure.Configuration.Service
{
    internal interface IDictionaryService
    {
        Task<List<DictionaryHeader>> GetDictionaryList();
        Task<List<DictionaryData>> GetDictionaryData(string dictionaryName);
        Task<List<DictionaryData>> GetDictionaryData(int dictionaryId);
        Task<List<DictionaryHeader>> AddNewDictionary(string name);
        Task<List<DictionaryHeader>> DeleteDictionary(string name);
        Task<List<DictionaryData>> AddNewDictionaryData(string search, string replace, int dictId);
        Task<List<DictionaryData>> AddNewDictionaryData(List<DictionaryData> data, int dictId);
        Task<List<DictionaryData>> DeleteDictionaryData(int dataId, int dictId);
        Task UpdateDictionary(string newName, int dictId);
        Task UpdateDictionaryData(string newSearch, string newReplace, int dataId, int dictId);
        Task<Dictionary<string, string>> GetDictionary(string dictionaryName);
    }
}