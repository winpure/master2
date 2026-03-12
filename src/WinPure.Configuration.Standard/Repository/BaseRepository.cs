namespace WinPure.Configuration.Repository;

internal  abstract class BaseRepository : IBaseRepository
{
    internal WinPureConfigurationContext _context { get; }

    public BaseRepository(WinPureConfigurationContext context)
    {
        _context = context;
    }


    public void Add(object entity)
    {
        _context.Add(entity);
    }

    public void Delete(object entity)
    {
        _context.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}