using WinPure.Configuration.Infrastructure.EntityConfiguration;

namespace WinPure.Configuration.Infrastructure
{
    public class WinPureConfigurationContext : DbContext
    {
        public WinPureConfigurationContext()
        {
        }

        public WinPureConfigurationContext(DbContextOptions<WinPureConfigurationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AutomationConfigurationStepEntity> AutomationConfigurationSteps { get; set; }
        public virtual DbSet<AutomationHeaderEntity> AutomationHeaders { get; set; }
        public virtual DbSet<AutomationLogEntity> AutomationLogs { get; set; }
        public virtual DbSet<AutomationScheduleEntity> AutomationSchedules { get; set; }
        public virtual DbSet<AutomationStepDictionaryEntity> AutomationStepsDictionary { get; set; }
        public virtual DbSet<DictionaryDataEntity> DictionaryData { get; set; }
        public virtual DbSet<DictionaryNameEntity> DictionaryName { get; set; }
        public virtual DbSet<RecentProjectEntity> RecentProjects { get; set; }
        public virtual DbSet<ProperCaseSettingEntity> ProperCaseSettings { get; set; }
        public virtual DbSet<ConnectionSettingEntity> ConnectionSettings { get; set; }
        public virtual DbSet<EntityResolutionMappingEntity> EntityResolutionMapping { get; set; }
        public virtual DbSet<StatisticPatternEntity> StatisticPatterns { get; set; }
        public virtual DbSet<CleansingAiConfigurationEntity> CleansingAiConfigurations { get; set; }
        public virtual DbSet<FavouriteSourceEntity> FavouriteSources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=C:\\ProgramData\\WinPure\\Clean & Match v11\\DB\\SysDict.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AutomationHeaderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AutomationLogEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AutomationScheduleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AutomationStepDictionaryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AutomationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AutomationConfigurationStepEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DictionaryNameEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DictionaryDataEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RecentProjectEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProperCaseSettingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ConnectionSettingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EntityResolutionMappingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StatisticPatternEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CleansingAiConfigurationEntityTypeConfiguration());
        }
    }
}
