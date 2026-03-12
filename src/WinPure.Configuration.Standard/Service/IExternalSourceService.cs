namespace WinPure.Configuration.Service
{
    internal interface IExternalSourceService
    {
        List<ExternalSource> GetSources();
        List<ExternalSource> GetFavorites();
        void AddFavorite(ExternalSourceTypes source);
        void RemoveFavorite(ExternalSourceTypes source);
    }
}
