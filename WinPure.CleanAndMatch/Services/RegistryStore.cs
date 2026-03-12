using Microsoft.Win32;

namespace WinPure.CleanAndMatch.Services;

/// <summary>
/// This is for accessing registry values
/// </summary>
internal class RegistryStore : IValueStore
{
    /// <summary>
    /// The reg path
    /// </summary>
    private const string REGISTRY_BASE_PATH = @"Software\WinPure";

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
        var registryPath = $@"{REGISTRY_BASE_PATH}\{subPath}";
        try
        {
            var currentRegistryBranch = Registry.CurrentUser;
            //HKEY_CURRENT_USER
            currentRegistryBranch = currentRegistryBranch.CreateSubKey(registryPath);
            currentRegistryBranch.SetValue(key, keyVal);

            if (duplicateToLocalMachine)
            {
                currentRegistryBranch = Registry.LocalMachine;
                //HKEY_CURRENT_USER
                currentRegistryBranch = currentRegistryBranch.CreateSubKey(registryPath);
                currentRegistryBranch.SetValue(key, keyVal);
            }
        }
        catch (Exception)
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
        var registryPath = $@"{REGISTRY_BASE_PATH}\{subPath}";
        try
        {
            var currentRegistryBranch = Registry.CurrentUser;
            key = key.Trim();
            currentRegistryBranch = currentRegistryBranch.CreateSubKey(registryPath);
            registryValue = Convert.ToString(currentRegistryBranch.GetValue(key)).Trim();
            if (string.IsNullOrEmpty(registryValue))
            {
                currentRegistryBranch = Registry.LocalMachine;
                currentRegistryBranch = currentRegistryBranch.CreateSubKey(registryPath);
                registryValue = Convert.ToString(currentRegistryBranch.GetValue(key, defaultValue)).Trim();
            }
        }
        catch
        {
            return defaultValue;
        }

        return registryValue;
    }

    #endregion
}
