namespace WinPure.Configuration.Repository;

internal interface IBaseRepository
{
    void Add(object entity);
    void Delete(object entity);
    Task SaveChangesAsync();
}