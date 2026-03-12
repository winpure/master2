
namespace WinPure.Configuration.Repository
{
    internal class FavoritesRepository : BaseRepository, IFavoritesRepository
    {
        public FavoritesRepository(WinPureConfigurationContext context) : base(context)
        {
        }

        public async Task Add(FavouriteSourceEntity favourite)
        {
            await _context.FavouriteSources.AddAsync(favourite);
            await _context.SaveChangesAsync();
        }

        public Task<List<FavouriteSourceEntity>> GetAll()
        {
            return _context.FavouriteSources.ToListAsync();
        }

        public async Task Remove(FavouriteSourceEntity favourite)
        {
            if (favourite != null)
            {
                _context.FavouriteSources.Remove(favourite);
                await _context.SaveChangesAsync();
            }
        }
    }
}
