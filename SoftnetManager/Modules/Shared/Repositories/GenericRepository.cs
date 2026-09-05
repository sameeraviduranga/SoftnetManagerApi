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
        public async Task AddAsync(T entity)
        {
            await _dbset.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await GetByIdAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbset.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbset.FindAsync(id);
        }

        //not used in this project, but can be used in other projects that use this generic repository
        public async Task<T?> GetByNameAsync(string name)
        {
            return await _dbset.FirstOrDefaultAsync(e => EF.Property<string>(e, "Name") == name);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);  
        }
    }
}
