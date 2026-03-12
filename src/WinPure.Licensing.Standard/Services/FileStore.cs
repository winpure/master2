using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;

namespace WinPure.Licensing.DependencyInjection;

internal partial class WinPureLicensingDependency
{
    private class FileStore : IValueStore
    {
        private const string REGISTRY_FILE = "WinPureValues.dat";

        #region PublicMethods
        /// <summary>
        /// Sets the value to Registry Editor.
        /// </summary>
        /// <param name="rootPath">The root path for registry value.</param>
        /// <param name="subPath">The sub path for registry value.</param>
        /// <param name="key">The key for registry value.</param>
        /// <param name="keyVal">The value for registry key.</param>
        /// <param name="duplicateToLocalMachine">Duplicate value in LocalMachine registry branch</param>
        public void SetValue(string subPath, string key, string keyVal, bool duplicateToLocalMachine = false)
        {
            try
            {

                var values = GetValuesFromFile();
                var valueKey = $@"{subPath}\{key}";
                if (values.ContainsKey(valueKey))
                {
                    values[valueKey] = keyVal;
                }
                else
                {
                    values.Add(valueKey, keyVal);
                }
                WriteValuesToFile(values);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the value to Registry Editor.
        /// </summary>
        /// <param name="rootPath">The root path for registry value.</param>
        /// <param name="subPath">The sub path for registry value.</param>
        /// <param name="key">The key for registry value.</param>
        /// <param name="defaultValue">The default value for registry key if there is no value for provided registry key.</param>
        /// <returns>The value for provided registry key.</returns>
        public string GetValue(string subPath, string key, string defaultValue = "")
        {
            string registryValue = defaultValue;
            try
            {
                var values = GetValuesFromFile();
                var valueKey = $@"{subPath}\{key}";
                if (values.ContainsKey(valueKey))
                {
                    registryValue = values[valueKey];
                }
            }
            catch
            {
                return defaultValue;
            }

            return registryValue;
        }

        #endregion

        private Dictionary<string, string> GetValuesFromFile()
        {
            var values = new Dictionary<string, string>();

            if (File.Exists(REGISTRY_FILE))
            {
                var fileData = File.ReadAllBytes(REGISTRY_FILE);
                var ms = new MemoryStream(fileData);
                using (var reader = new BsonDataReader(ms))
                {
                    var serializer = new JsonSerializer();

                    values = serializer.Deserialize<Dictionary<string, string>>(reader);
                }
            }

            return values;
        }

        private void WriteValuesToFile(Dictionary<string, string> values)
        {
            var ms = new MemoryStream();
            using (var writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, values);
                File.WriteAllBytes(REGISTRY_FILE, ms.ToArray());
            }
        }
    }
}