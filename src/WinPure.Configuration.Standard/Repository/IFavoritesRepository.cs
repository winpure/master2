namespace WinPure.Configuration.Repository
{
    internal interface IFavoritesRepository
    {
        Task<List<FavouriteSourceEntity>> GetAll();
        Task Add(FavouriteSourceEntity favourite);
        Task Remove(FavouriteSourceEntity favourite);
    }
}
