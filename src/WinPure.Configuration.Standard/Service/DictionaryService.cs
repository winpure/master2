using WinPure.Common.Exceptions;
using WinPure.Configuration.Models.Dictionary;
using WinPure.Configuration.Properties;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class DictionaryService : IDictionaryService
    {
        private readonly IDictionaryRepository _repository;

        public DictionaryService(IDictionaryRepository repository)
        {
            _repository = repository;
        }

        public Task<List<DictionaryHeader>> GetDictionaryList() => _repository.GetDictionaryList();

        public Task<List<DictionaryData>> GetDictionaryData(string dictionaryName) => _repository.GetDictionaryData(dictionaryName);

        public Task<List<DictionaryData>> GetDictionaryData(int dictionaryId) => _repository.GetDictionaryData(dictionaryId);
        
        public Task<Dictionary<string, string>> GetDictionary(string dictionaryName) => _repository.GetDictionary(dictionaryName);

        public async Task<List<DictionaryHeader>> AddNewDictionary(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new WinPureArgumentException("Library name cannot be empty. Please enter a name.");
            }

            var dictionaries = await _repository.GetDictionaryList();
            if (dictionaries.Any(x => x.Name == name))
            {
                throw new WinPureArgumentException("Dictionary with the same name exists in database. Please add another name.");
            }

            var dictionaryEntity = new DictionaryNameEntity { DictionaryName = name };
            return await _repository.AddNewDictionary(dictionaryEntity);
        }

        public async Task<List<DictionaryHeader>> DeleteDictionary(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new WinPureArgumentException("Dictionary name cannot be empty. Please enter a name.");
            }

            await _repository.DeleteDictionary(name);
            return await _repository.GetDictionaryList();
        }

        public async Task<List<DictionaryData>> AddNewDictionaryData(string search, string replace, int dictId)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_FILL_SEARCH_VALUE);
            }

            if (dictId <= 0)
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_SELECT_DICTIONARY);
            }

            var dictionaryData = await _repository.GetDictionaryDataValue(search, dictId);
            if (dictionaryData != null)
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_SEARCH_VALUE_EXISTS);
            }

            await _repository.AddNewDictionaryData(search, replace, dictId);

            return await GetDictionaryData(dictId);
        }

        public async Task<List<DictionaryData>> AddNewDictionaryData(List<DictionaryData> data, int dictId)
        {
            if (data.Any( x => string.IsNullOrEmpty(x.SearchValue)))
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_FILL_SEARCH_VALUE);
            }

            if (dictId <= 0)
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_SELECT_DICTIONARY);
            }

            var dictionaryData = await GetDictionaryData(dictId);

            foreach (var newValue in data)
            {
                var existingValue = dictionaryData.FirstOrDefault(x => string.Equals(x.SearchValue, newValue.SearchValue));
                if (existingValue != null)
                {
                    await _repository.UpdateDictionaryData(newValue.SearchValue, newValue.ReplaceValue, existingValue.Id, dictId);
                }
                else
                {
                    await _repository.AddNewDictionaryData(newValue.SearchValue, newValue.ReplaceValue, dictId);
                }
            }

            return await GetDictionaryData(dictId);
        }

        public Task<List<DictionaryData>> DeleteDictionaryData(int dataId, int dictId) => _repository.DeleteDictionaryData(dataId, dictId);

        public async Task UpdateDictionary(string newName, int dictId)
        {
            if (dictId < 8)
            {
                throw new WinPureAccessException(Resources.DICTIONARY_EXCEPTION_CANNOT_EDIT_SYSTEM_DICTIONARY);
            }

            var dictionaries = await _repository.GetDictionaryList();

            if (dictionaries.Any(x => x.Id != dictId && x.Name == newName))
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_SELECT_ANOTHER_LIBRARY_NAME);
            }

            await _repository.UpdateDictionary(newName, dictId);
        }

        public async Task UpdateDictionaryData(string newSearch, string newReplace, int dataId, int dictId)
        {
            var dictionaryData = await _repository.GetDictionaryData(dictId);

            if (dictionaryData.Any(x =>
                x.DictionaryId == dictId && x.Id != dataId && x.SearchValue == newSearch))
            {
                throw new WinPureArgumentException(Resources.DICTIONARY_EXCEPTION_SELECT_ANOTHER_SEARCH_VALUE);
            }

            await _repository.UpdateDictionaryData(newSearch, newReplace, dataId, dictId);
        }
    }
}