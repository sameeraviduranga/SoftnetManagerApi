using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Identity.Infrastructure.Repositories;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Interfaces;
using System.Data;
using static Azure.Core.HttpHeader;

namespace SoftnetManager.Modules.Shared.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext context;
        private IDbContextTransaction? _transaction;// Define repositories for each entity

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get;}


        public UnitOfWork(AppDbContext context)
        {
            this.context = context;

            Users = new UserRepository(context);
            Roles = new RoleRepository(context);
           
            
        }
        public bool HasActiveTransaction =>_transaction != null;
        //IUserRepository IUnitOfWork.Users => throw new NotImplementedException();

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
                return _transaction;

            _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            return _transaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if ( _transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            await context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(CancellationToken.None); // Commit is cleanup/finalization → don't cancel it
            await DisposeTransactionAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to rollback.");

            await _transaction.RollbackAsync(CancellationToken.None);

            await DisposeTransactionAsync();
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
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
