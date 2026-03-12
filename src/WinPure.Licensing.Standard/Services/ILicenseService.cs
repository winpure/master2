using System;
using System.Collections.Generic;
using WinPure.Common.Enums;
using WinPure.Licensing.Enums;

namespace WinPure.Licensing.Services
{
    internal interface ILicenseService
    {
        event Action LicenseLoaded;
        bool IsDemo { get; }
        ProgramType ProgramType { get; }
        void Initiate();
        void InitiateApi();

        int LicenseExpiredAtDays { get; }
        int ExpirationDays { get; }

        /// <summary>
        /// Gets the generation key and copies to clipboard.
        /// </summary>
        string GetLocalRegistrationKey();

        LicenseState LoadLicense(string licensePath, string fileName);
        LicenseState Register(string licenseFile);

        LicenseState GetLicenseState();

        Dictionary<string, string> GetFullLicenseInfo();
        int GetErRecordLimit();
        bool IsAuditLogEnabled();
    }
}