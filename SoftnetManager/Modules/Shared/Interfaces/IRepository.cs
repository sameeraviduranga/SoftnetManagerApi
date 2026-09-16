namespace SoftnetManager.Modules.Shared.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
        Task SaveAsync(CancellationToken cancellationToken);//no need
    }
}
