using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WinPure.Cleansing.Models;
using WinPure.Cleansing.Services;
using WinPure.Common.Helpers;
using WinPure.Configuration.DependencyInjection;
using WinPure.Configuration.Models.Database;
using WinPure.Project.Models;

namespace WinPure.Project.DependencyInjection;

internal static partial class WinPureProjectDependencyExtension
{
    private class RecentProjectService : IRecentProjectService
    {
        private const int RECENT_PROJECT_LIST_COUNT = 20;
        private readonly WinPureConfigurationContext _context;
        public event Action OnRecentListChanged; //TODO

        public RecentProjectService(WinPureConfigurationContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateProjectAsync(ProjectSettings project)
        {

            var recentProject = await _context.RecentProjects.FirstOrDefaultAsync(x => x.Id == project.Id);
            if (recentProject == null)
            {
                recentProject = new RecentProjectEntity()
                {
                    Id = project.Id,
                    ProjectName = project.ProjectName,
                    ProjectPath = project.ProjectPath,
                    CreateDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                };
                await _context.RecentProjects.AddAsync(recentProject);
            }
            else
            {
                recentProject.ProjectName = project.ProjectName;
                recentProject.ProjectPath = project.ProjectPath;
                recentProject.LastUpdateDate = DateTime.UtcNow;
            }
            SetEnvironmentSettings(recentProject);

            await _context.SaveChangesAsync();
            OnRecentListChanged?.Invoke();
        }

        public bool CheckProjectIfExist(RecentProject project)
        {
            return File.Exists(project.Path);
        }

        public async Task<List<RecentProject>> GetRecentProjectsAsync()
        {
            return await _context.RecentProjects.OrderByDescending(x => x.LastUpdateDate).Take(RECENT_PROJECT_LIST_COUNT)
                .Select(x => new RecentProject
                {
                    Id = x.Id,
                    Name = x.ProjectName,
                    Path = x.ProjectPath,
                    ModifiedBy = x.ModifiedBy,
                    CreateDate = x.CreateDate.ToLocalTime(),
                    EditDate = x.LastUpdateDate.ToLocalTime(),
                    NumberOfDataset = x.NumberOfDataset,
                    NumberOfRecords = x.NumberOfRecords,
                    IsCleansing = x.IsCleansing,
                    IsMatch = x.IsMatch,
                    IsMatchAi = x.IsMatchAi,
                    IsAddressVerification = x.IsAddressVerification,
                    IsAutomation = x.IsAutomation,
                    IsAuditLog = x.IsAuditLog
                }).ToListAsync();
        }

        public async Task<RecentProject> GetRecentProjectByPath(string projectPath)
        {
            var recentProjectEntity = await _context.RecentProjects.FirstOrDefaultAsync(x => x.ProjectPath == projectPath);
            if (recentProjectEntity == null)
            {
                return null;
            }

            return new RecentProject
            {
                Id = recentProjectEntity.Id,
                Name = recentProjectEntity.ProjectName,
                Path = recentProjectEntity.ProjectPath,
                ModifiedBy = recentProjectEntity.ModifiedBy,
                CreateDate = recentProjectEntity.CreateDate.ToLocalTime(),
                EditDate = recentProjectEntity.LastUpdateDate.ToLocalTime(),
                NumberOfDataset = recentProjectEntity.NumberOfDataset,
                NumberOfRecords = recentProjectEntity.NumberOfRecords,
                IsCleansing = recentProjectEntity.IsCleansing,
                IsMatch = recentProjectEntity.IsMatch,
                IsMatchAi = recentProjectEntity.IsMatchAi,
                IsAddressVerification = recentProjectEntity.IsAddressVerification,
                IsAutomation = recentProjectEntity.IsAutomation,
                IsAuditLog = recentProjectEntity.IsAuditLog
            };
        }

        public async Task RemoveProjectAsync(RecentProject project)
        {
            var recentProject = await _context.RecentProjects.FirstOrDefaultAsync(x => x.Id == project.Id);
            if (recentProject != null)
            {
                _context.RecentProjects.Remove(recentProject);
                await _context.SaveChangesAsync();
                OnRecentListChanged?.Invoke();
            }
        }

        public void RemoveProject(RecentProject project)
        {
            var recentProject = _context.RecentProjects.FirstOrDefault(x => x.Id == project.Id);
            if (recentProject != null)
            {
                _context.RecentProjects.Remove(recentProject);
                _context.SaveChanges();
                OnRecentListChanged?.Invoke();
            }
        }

        public async Task RenameProjectAsync(RecentProject project)
        {
            var recentProject = await _context.RecentProjects.FirstOrDefaultAsync(x => x.Id == project.Id);
            if (recentProject != null)
            {
                recentProject.ProjectName = project.Name;
                await _context.SaveChangesAsync();
                OnRecentListChanged?.Invoke();
            }
        }

        public void RenameProject(RecentProject project)
        {
            var recentProject = _context.RecentProjects.FirstOrDefault(x => x.Id == project.Id);
            if (recentProject != null)
            {
                recentProject.ProjectName = project.Name;
                _context.SaveChanges();
                OnRecentListChanged?.Invoke();
            }
        }

        private void SetEnvironmentSettings(RecentProjectEntity projectEntity)
        {
            var dataManagerService = WinPureConfigurationDependency.Resolve<IDataManagerService>();
            var configuration = WinPureConfigurationDependency.Resolve<IConfigurationService>();
            var cleansingConfigurationService = WinPureConfigurationDependency.Resolve<ICleansingConfigurationService>();

            projectEntity.NumberOfDataset = dataManagerService.TableList.Count;
            projectEntity.NumberOfRecords = dataManagerService.TableList.Sum(x => x.RowCount);
            projectEntity.IsAuditLog = configuration.Configuration.EnableAuditLogs;
            projectEntity.IsAutomation = configuration.Configuration.EnableAutomation;
            projectEntity.IsMatch = dataManagerService.MatchSettings != null && dataManagerService.MatchSettings.MatchParameters != null && dataManagerService.MatchSettings.MatchParameters.Any();
            projectEntity.IsMatchAi = dataManagerService.TableList.Any(t => t.IsResolutionSelected);
            projectEntity.IsCleansing = false;
            projectEntity.IsAddressVerification = false;
            projectEntity.ModifiedBy = SystemInfoHelper.GetCurrentUserQualified();

            foreach (var table in dataManagerService.TableList)
            {
                var cleansingConfiguration = cleansingConfigurationService.GetWinPureCleanSettings(table.TableName);
                projectEntity.IsCleansing = projectEntity.IsCleansing || HasCleansingSettings(cleansingConfiguration);

                var afSettings = dataManagerService.GetDataTableAddressVerificationSetting(table.TableName);
                projectEntity.IsAddressVerification = projectEntity.IsAddressVerification || HasAddressVerificationSettings(afSettings);
            }
        }

        private bool HasCleansingSettings(WinPureCleanSettings cleansingSettings)
        {
            return cleansingSettings.TextCleanerSettings.Any() || cleansingSettings.CaseConverterSettings.Any() || cleansingSettings.WordManagerSettings.Any() || cleansingSettings.ColumnSplitSettings.Any() || cleansingSettings.ColumnMergeSettings.Any() || cleansingSettings.ColumnShiftSettings.Any() || cleansingSettings.ColumnCheckSettings.Any();
        }

        private bool HasAddressVerificationSettings(DataTable afSettings)
        {
            return afSettings.AsEnumerable().Any(x => x.Field<long>("AF_Address") == 1 || x.Field<long>("AF_Zip") == 1 || x.Field<long>("AF_City") == 1 || x.Field<long>("AF_State") == 1 || x.Field<long>("RG_Latitude") == 1 || x.Field<long>("RG_Longitude") == 1);
        }
    }
}