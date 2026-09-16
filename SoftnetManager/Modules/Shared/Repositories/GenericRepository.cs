using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Shared.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbset;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbset = _dbContext.Set<T>();
        }
        public void Add(T entity)
        {
            _dbset.Add(entity);//add entity to change tracker
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await GetByIdAsync(id,cancellationToken) != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbset.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(int id,CancellationToken cancellationToken)
        {
            return await _dbset.FindAsync(id,cancellationToken);
        }

        //not used in this project, but can be used in other projects that use this generic repository
        public async Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _dbset.FirstOrDefaultAsync(e => EF.Property<string>(e, "Name") == name,cancellationToken);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)//not used in this project, but can be used in other projects that use this generic repository
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);  
        }
    }
}
