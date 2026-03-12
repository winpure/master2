namespace WinPure.Licensing.Services
{
    internal interface IValueStore
    {
        /// <summary>
        /// Sets the value to Registry Editor.
        /// </summary>
        /// <param name="rootPath">The root path for registry value.</param>
        /// <param name="subPath">The sub path for registry value.</param>
        /// <param name="key">The key for registry value.</param>
        /// <param name="keyVal">The value for registry key.</param>
        /// <param name="duplicateToLocalMachine">Duplicate value in LocalMachine registry branch</param>
        void SetValue(string subPath, string key, string keyVal, bool duplicateToLocalMachine = false);

        /// <summary>
        /// Gets the value to Registry Editor.
        /// </summary>
        /// <param name="rootPath">The root path for registry value.</param>
        /// <param name="subPath">The sub path for registry value.</param>
        /// <param name="key">The key for registry value.</param>
        /// <param name="defaultValue">The default value for registry key if there is no value for provided registry key.</param>
        /// <returns>The value for provided registry key.</returns>
        string GetValue(string subPath, string key, string defaultValue = "");
    }
}