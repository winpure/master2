namespace WinPure.Configuration.Service
{
    internal class ExternalSourceService : IExternalSourceService
    {
        public static List<ExternalSourceTypes> FileSources = new List<ExternalSourceTypes>
        {
            ExternalSourceTypes.TextFile,
            ExternalSourceTypes.Excel,
            ExternalSourceTypes.Json,
            ExternalSourceTypes.Xml,
            ExternalSourceTypes.JSONL,
            ExternalSourceTypes.Senzing,
        };

        public static List<ExternalSourceTypes> DatabaseSources = new List<ExternalSourceTypes>
        {
            ExternalSourceTypes.SqlServer,
            ExternalSourceTypes.MySqlServer,
            ExternalSourceTypes.Access,
            ExternalSourceTypes.Oracle,
            ExternalSourceTypes.SqLite,
            ExternalSourceTypes.Db2,
            ExternalSourceTypes.AzureDb,
            ExternalSourceTypes.Postgres,
            ExternalSourceTypes.Snowflake,
        };

        public static List<ExternalSourceTypes> CrmSources = new List<ExternalSourceTypes>
        {
            ExternalSourceTypes.ZohoCrm,
            ExternalSourceTypes.Salesforce,
        };

        private const int MaxFavorites = 8;
        private readonly IFavoritesRepository _repository;

        public ExternalSourceService(IFavoritesRepository repository)
        {
            _repository = repository;
        }

        public List<ExternalSource> GetSources()
        {
            var favourites = GetFavoriteEntities();

            return Enum.GetValues(typeof(ExternalSourceTypes))
                .Cast<ExternalSourceTypes>()
                .Select(source =>
                {
                    var group = GetSourceGroup(source);
                    return new ExternalSource
                    {
                        Source = source,
                        Group = group,
                        Favorite = favourites.Any(fav => fav.Source == source),
                        CanImport = group != ExternalSourceGroup.Unsupported,
                        CanExport = group == ExternalSourceGroup.File || group == ExternalSourceGroup.Database,
                    };
                }).Where(x => x.Group != ExternalSourceGroup.Unsupported)
                .ToList();
        }

        public List<ExternalSource> GetFavorites()
        {
            var favourites = GetFavoriteEntities();
            return favourites.Select(favourite =>
            {
                var group = GetSourceGroup(favourite.Source);
                return new ExternalSource
                {
                    Source = favourite.Source,
                    Group = group,
                    Favorite = true,
                    CanImport = group != ExternalSourceGroup.Unsupported,
                    CanExport = group == ExternalSourceGroup.File || group == ExternalSourceGroup.Database,
                };
            }).ToList();
        }

        public void AddFavorite(ExternalSourceTypes source)
        {
            var favourites = GetFavoriteEntities();

            if (favourites.Any(x => x.Source == source))
                return;

            if (favourites.Count == MaxFavorites)
                throw new Exception($"You can add up to {MaxFavorites} favorites only");

            var newFavorite = new FavouriteSourceEntity { Source = source };
            AsyncHelpers.RunSync(() => _repository.Add(newFavorite));
        }

        public void RemoveFavorite(ExternalSourceTypes source)
        {
            var favourites = GetFavoriteEntities();
            var favoriteToRemove = favourites.FirstOrDefault(x => x.Source == source);
            if (favoriteToRemove != null)
            {
                AsyncHelpers.RunSync(() => _repository.Remove(favoriteToRemove));
            }   
        }

        private List<FavouriteSourceEntity> GetFavoriteEntities() => AsyncHelpers.RunSync(_repository.GetAll);

        private static ExternalSourceGroup GetSourceGroup(ExternalSourceTypes sourceType)
        {
            if (FileSources.Contains(sourceType))
            {
                return ExternalSourceGroup.File;
            }

            if (DatabaseSources.Contains(sourceType))
            {
                return ExternalSourceGroup.Database;
            }

            if (CrmSources.Contains(sourceType))
            {
                return ExternalSourceGroup.CRM;
            }

            return ExternalSourceGroup.Unsupported;
        }
    }
}
