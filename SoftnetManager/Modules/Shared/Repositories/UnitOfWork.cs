using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Identity.Infrastructure.Repositories;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Interfaces;
using System.Data;

namespace SoftnetManager.Modules.Shared.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext context;
        private IDbContextTransaction? _transaction;// Define repositories for each entity

        public IUserRepository Users { get; }
        

        public UnitOfWork(AppDbContext context)
        {
            this.context = context;

            Users = new UserRepository(context);
            
           
            
        }
        public bool HasActiveTransaction =>_transaction != null;
        //IUserRepository IUnitOfWork.Users => throw new NotImplementedException();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
                return _transaction;

            _transaction = await context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if ( _transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            await context.SaveChangesAsync();
            await _transaction.CommitAsync();
            await DisposeTransactionAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            context.Dispose();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to rollback.");

            await _transaction.RollbackAsync();

            await DisposeTransactionAsync();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
        

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            
        }

        
    }
}
