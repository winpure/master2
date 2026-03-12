using System.Collections.Concurrent;
using System.Threading;
using WinPure.Configuration.Models.Dictionary;

namespace WinPure.Configuration.DependencyInjection;

internal partial class WinPureConfigurationDependency
{
    private class DictionaryRepository : IDictionaryRepository
    {
        private readonly WinPureConfigurationContext _configurationContext;
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> CashDictionary;
        private ReaderWriterLockSlim _locker;

        public DictionaryRepository(WinPureConfigurationContext configurationContext)
        {
            _configurationContext = configurationContext;
            _locker = new ReaderWriterLockSlim();
            CashDictionary = new ConcurrentDictionary<string, Dictionary<string, string>>();
        }

        public async Task<List<DictionaryHeader>> GetDictionaryList()
        {
            return await _configurationContext.DictionaryName.Select(x => new DictionaryHeader
            {
                Id = x.Id,
                Name = x.DictionaryName
            }).ToListAsync();
        }

        public async Task<List<DictionaryData>> GetDictionaryData(string dictionaryName)
        {
            return await _configurationContext.DictionaryData.Where(x => x.Dictionary.DictionaryName == dictionaryName)
                .Select(x => x.ToDictionaryData()).ToListAsync();
        }

        public async Task<List<DictionaryData>> GetDictionaryData(int dictionaryId)
        {
            return await _configurationContext.DictionaryData.Where(x => x.DictionaryId == dictionaryId)
                .Select(x => x.ToDictionaryData()).ToListAsync();
        }

        public async Task<List<DictionaryHeader>> AddNewDictionary(DictionaryNameEntity dictionaryEntity)
        {
            await _configurationContext.DictionaryName.AddAsync(dictionaryEntity);
            await _configurationContext.SaveChangesAsync();

            return await GetDictionaryList();
        }

        public async Task DeleteDictionary(string name)
        {
            var dictionaryEntity = await _configurationContext.DictionaryName.FirstOrDefaultAsync(x => x.DictionaryName == name);

            if (dictionaryEntity != null)
            {
                _configurationContext.Remove(dictionaryEntity);
                await _configurationContext.SaveChangesAsync();
                ClearCache();
            }
        }

        public async Task<DictionaryData> GetDictionaryDataValue(string search, int dictId)
        {
            return await _configurationContext.DictionaryData
                .Where(x => x.DictionaryId == dictId && x.SearchValue == search)
                .Select(x => x.ToDictionaryData())
                .FirstOrDefaultAsync();
        }

        public async Task AddNewDictionaryData(string search, string replace, int dictId)
        {
            var dictionaryData = new DictionaryDataEntity
            {
                DictionaryId = dictId,
                ReplaceValue = replace,
                SearchValue = search
            };

            await _configurationContext.DictionaryData.AddAsync(dictionaryData);
            await _configurationContext.SaveChangesAsync();
            ClearCache();
        }

        public async Task<List<DictionaryData>> DeleteDictionaryData(int dataId, int dictId)
        {
            var dictionaryData = await _configurationContext.DictionaryData.FirstOrDefaultAsync(x => x.Id == dataId);
            if (dictionaryData != null)
            {
                _configurationContext.Remove(dictionaryData);
                await _configurationContext.SaveChangesAsync();
                ClearCache();
            }

            return await GetDictionaryData(dictId);
        }

        public async Task UpdateDictionary(string newName, int dictId)
        {
            var dictionary = await _configurationContext.DictionaryName.FirstOrDefaultAsync(x => x.Id == dictId);

            if (dictionary != null)
            {
                dictionary.DictionaryName = newName;
                await _configurationContext.SaveChangesAsync();
                ClearCache();
            }
        }

        public async Task UpdateDictionaryData(string newSearch, string newReplace, int dataId, int dictId)
        {
            var dictionaryData = await _configurationContext.DictionaryData.FirstOrDefaultAsync(x => x.Id == dataId);
            if (dictionaryData != null)
            {
                dictionaryData.SearchValue = newSearch;
                dictionaryData.ReplaceValue = newReplace;
                await _configurationContext.SaveChangesAsync();
                ClearCache();
            }
        }

        public async Task<Dictionary<string, string>> GetDictionary(string dictionaryName)
        {
            var result = CheckInCache(dictionaryName);
            if (result != null)
            {
                return result;
            }

            _locker.EnterWriteLock();
            try
            {
                result = await _configurationContext.DictionaryData
                    .Where(x => x.Dictionary.DictionaryName == dictionaryName)
                    .ToDictionaryAsync(x => x.SearchValue.ToLower(), y => y.ReplaceValue.ToLower());
                AddInCache(dictionaryName, result);
                return result;
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        private Dictionary<string, string> CheckInCache(string dictName)
        {
            _locker.EnterReadLock();
            try
            {
                if (CashDictionary.TryGetValue(dictName, out var dictionaryValue))
                {
                    return dictionaryValue;
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }

            return null;
        }

        private void AddInCache(string dictName, Dictionary<string, string> dict)
        {
            CashDictionary.TryAdd(dictName, dict);
        }

        public void ClearCache()
        {
            CashDictionary.Clear();
        }
    }
}