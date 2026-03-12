using IntelliLock.Licensing;
using System;
using System.Collections.Generic;
using System.IO;
using WinPure.Common.Constants;
using WinPure.Common.Enums;
using WinPure.Common.Helpers;
using WinPure.Common.Services;
using WinPure.Licensing.Enums;

namespace WinPure.Licensing.DependencyInjection;

internal partial class WinPureLicensingDependency
{
    private class LicenseService : ILicenseService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IValueStore _valueStore;
        private readonly string _programTypeRegistryKey;
        private readonly string _purchaseDateRegistryKey;

        private string _registryInstallPath;

        private string _currentProgramRegistryKey = "CAMENT";
        private string _currentApiRegistryKey = "API";

        public ProgramType ProgramType { get; private set; } = ProgramType.CamEnt;

        public event Action LicenseLoaded;
        
#if DEBUG
        public bool IsDemo => false;
#else
        public bool IsDemo => ProgramType != ProgramType.CamFree && !CheckSaas() &&
                              (EvaluationMonitor.CurrentLicense.LicenseStatus == LicenseStatus.EvaluationMode);
#endif

        public LicenseService(IEncryptionService encryptionService, IValueStore valueStore)
        {
            _valueStore = valueStore;
            _encryptionService = encryptionService;

            _programTypeRegistryKey = _encryptionService.Hash("Product");
            _purchaseDateRegistryKey = _encryptionService.Hash("PurchaseDate");
        }

        public void Initiate()
        {
            _registryInstallPath = _currentProgramRegistryKey + @"\Install";
            var currentProgram = _encryptionService.Decrypt(_valueStore.GetValue(_registryInstallPath, _programTypeRegistryKey));
            if (string.IsNullOrWhiteSpace(currentProgram)) // first start
            {
                RegisterProduct();
            }
        }

        public void InitiateApi()
        {
            ProgramType = ProgramType.Api;
            _registryInstallPath = _currentApiRegistryKey + @"\Install";
            var currentProgram = _encryptionService.Decrypt(_valueStore.GetValue(_registryInstallPath, _programTypeRegistryKey));
            if (string.IsNullOrWhiteSpace(currentProgram)) // first start
            {
                RegisterProduct();
            }
        }

        public int LicenseExpiredAtDays
        {
            get
            {

#if DEBUG
                return 30;
#endif

                if (ProgramType == ProgramType.CamFree || CheckSaas())
                {
                    return 5000;
                }

                if (EvaluationMonitor.CurrentLicense.ExpirationDays_Enabled)
                {
                    return EvaluationMonitor.CurrentLicense.ExpirationDays - EvaluationMonitor.CurrentLicense.ExpirationDays_Current;
                }

                if (EvaluationMonitor.CurrentLicense.ExpirationDate_Enabled)
                {
                    return Convert.ToInt32((EvaluationMonitor.CurrentLicense.ExpirationDate - DateTime.Now).TotalDays);
                }

                return 0;
            }
        }

        public int ExpirationDays
        {
            get
            {
                if (ProgramType == ProgramType.CamFree || CheckSaas())
                {
                    return 5000;
                }

                if (EvaluationMonitor.CurrentLicense.ExpirationDays_Enabled)
                {
                    return EvaluationMonitor.CurrentLicense.ExpirationDays;
                }

                if (EvaluationMonitor.CurrentLicense.ExpirationDate_Enabled)
                {
                    var purchaseDateString = _valueStore.GetValue(_registryInstallPath, _purchaseDateRegistryKey);
                    if (!string.IsNullOrEmpty(purchaseDateString))
                    {
                        var purchaseDate = Convert.ToDateTime(_encryptionService.Decrypt(purchaseDateString)).ToLocalTime();
                        return Convert.ToInt32((EvaluationMonitor.CurrentLicense.ExpirationDate - purchaseDate).TotalDays);
                    }
                }

                return 0;
            }
        }

        public string GetLocalRegistrationKey()
        {
            return HardwareID.GetHardwareID(true, true, false, true, false, false);
        }

        public LicenseState LoadLicense(string licensePath, string fileName)
        {
            var licenseCodeState = GetLicenseState();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return licenseCodeState;
            }

            var licenseFile = Path.Combine(licensePath, fileName);

            try
            {
                EvaluationMonitor.LoadLicense(licenseFile);

                licenseCodeState = GetLicenseState();

                _valueStore.SetValue(_registryInstallPath, _purchaseDateRegistryKey, _encryptionService.Encrypt(DateTime.UtcNow.ToString("s")));
            }
            catch
            {
                // ignored
            }

            ProgramType = GetProgramType();

            return licenseCodeState;
        }

        public LicenseState Register(string licenseFile)
        {
            LicenseState licenseCodeState = LicenseState.Invalid;

            if (string.IsNullOrWhiteSpace(licenseFile))
            {
                return licenseCodeState;
            }

            try
            {
                EvaluationMonitor.LoadLicense(licenseFile);

                licenseCodeState = GetLicenseState();

                _valueStore.SetValue(_registryInstallPath, _purchaseDateRegistryKey, _encryptionService.Encrypt(DateTime.UtcNow.ToString("s")));

                LicenseLoaded?.Invoke();
            }
            catch
            {
                // ignored
            }

            ProgramType = GetProgramType();

            if (ProgramType == ProgramType.CamFree)
            {
                return LicenseState.Free;
            }

            return licenseCodeState;
        }

        public LicenseState GetLicenseState()
        {
            if (ProgramType == ProgramType.CamFree)
            {
                return LicenseState.Free;
            }
#if DEBUG
            return LicenseState.Valid;
#endif
            if (CheckSaas())
            {
                return LicenseState.Valid;
            }

            var currentLicense = EvaluationMonitor.CurrentLicense;

            if (currentLicense.LicenseStatus == LicenseStatus.NotChecked || !currentLicense.HardwareLock_Enabled)
            {
                return LicenseState.Demo;
            }

            switch (currentLicense.LicenseStatus)
            {
                case LicenseStatus.Licensed:
                case LicenseStatus.Reactivated: return LicenseState.Valid;

                case LicenseStatus.LicenseFileNotFound:
                case LicenseStatus.EvaluationMode: return LicenseState.Demo;
                case LicenseStatus.EvaluationExpired: return LicenseState.DemoExpire;

                case LicenseStatus.FullVersionExpired:
                case LicenseStatus.Deactivated: return LicenseState.LicenseExpire;

                case LicenseStatus.HardwareNotMatched:
                case LicenseStatus.InvalidSignature:
                case LicenseStatus.ServerValidationFailed:
                case LicenseStatus.FloatingLicenseUserExceeded:
                case LicenseStatus.FloatingLicenseServerError:
                case LicenseStatus.FloatingLicenseServerTimeout: return LicenseState.Invalid;
                default:
                    return LicenseState.Invalid;
            }
        }

        public Dictionary<string, string> GetFullLicenseInfo()
        {
            var currentLicense = EvaluationMonitor.CurrentLicense;
            var info = new Dictionary<string, string>();

            info.Add("Registration key", GetLocalRegistrationKey());
            info.Add("Current hardware ID", currentLicense.HardwareID);
            info.Add("License location", currentLicense.LicenseLocation.ToString());
            info.Add("WinPure license state", GetLicenseState().ToString());
            info.Add("Internal license state", currentLicense.LicenseStatus.ToString());
            info.Add("Expiration days enabled", currentLicense.ExpirationDays_Enabled.ToString());
            info.Add("Current expiration day", currentLicense.ExpirationDays_Current.ToString());
            info.Add("Expiration days", currentLicense.ExpirationDays.ToString());
            info.Add("Expiration date enabled", currentLicense.ExpirationDate_Enabled.ToString());
            info.Add("Expiration date", currentLicense.ExpirationDate.ToString());

            if (currentLicense.LicenseInformation != null)
            {
                for (int i = 0; i < currentLicense.LicenseInformation.Count; i++)
                {
                    string key = currentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = currentLicense.LicenseInformation.GetByIndex(i).ToString();

                    info.Add($"Info_{key}", value);
                }
            }

            return info;
        }

        public int GetErRecordLimit()
        {
#if DEBUG
            return 10000000;
#endif
            var currentLicense = EvaluationMonitor.CurrentLicense;
            if (currentLicense.LicenseInformation != null)
            {
                for (int i = 0; i < currentLicense.LicenseInformation.Count; i++)
                {
                    string key = currentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = currentLicense.LicenseInformation.GetByIndex(i).ToString();

                    if (key == "ER" && Int32.TryParse(value, out var erLimit))
                    {
                        return erLimit;
                    }
                }
            }
            //define default limit for registered version
            return GlobalConstants.ErRecordsForDemoVersion;
        }

        public bool IsAuditLogEnabled()
        {
#if DEBUG
            return true;
#endif
            var currentLicense = EvaluationMonitor.CurrentLicense;
            if (currentLicense.LicenseInformation != null)
            {
                for (int i = 0; i < currentLicense.LicenseInformation.Count; i++)
                {
                    string key = currentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = currentLicense.LicenseInformation.GetByIndex(i).ToString();

                    if (key == "LOG" && bool.TryParse(value, out var isEnabled))
                    {
                        return isEnabled;
                    }
                }
            }
            return false;
        }
        private ProgramType GetProgramType()
        {
#if DEBUG
            return ProgramType;
#endif
            if (IsDemo)
            {
                return ProgramType;
            }

            var currentLicense = EvaluationMonitor.CurrentLicense;
            if (currentLicense.LicenseInformation != null)
            {
                for (int i = 0; i < currentLicense.LicenseInformation.Count; i++)
                {
                    string key = currentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = currentLicense.LicenseInformation.GetByIndex(i).ToString();
                    if (key == "Product" && Enum.TryParse(value, out ProgramType programType))
                    {
                        ProgramType = programType;
                        return programType;
                    }
                }
            }
            
            throw new ArgumentOutOfRangeException("Program type not defined");
        }

        private bool CheckSaas()
        {
            var currentLicense = EvaluationMonitor.CurrentLicense;
            var index = currentLicense.LicenseInformation.IndexOfKey("Solution");
            if (index > -1)
            {
                var solution = currentLicense.LicenseInformation.GetByIndex(index);
                return solution.ToString().ToLower() == "winpure saas";
            }

            return false;
        }

        private void RegisterProduct()
        {
            _valueStore.SetValue(_registryInstallPath, _programTypeRegistryKey, _encryptionService.Encrypt(ProgramType.ToString().ToUpper()), ProgramTypeHelper.AutomationPrograms.Contains(ProgramType));
            _valueStore.SetValue(_registryInstallPath, _purchaseDateRegistryKey, _encryptionService.Encrypt(DateTime.UtcNow.ToString("s")));
        }
    }
}