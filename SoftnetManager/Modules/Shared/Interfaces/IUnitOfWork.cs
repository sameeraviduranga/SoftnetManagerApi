using Microsoft.EntityFrameworkCore.Storage;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Shared.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        //Define repositories for each entity
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        //Iproductrepository Products { get; }
        //Icustomerrepository Customers { get; }

        bool HasActiveTransaction { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitTransactionAsync(CancellationToken cancellationToken);
        Task RollbackTransactionAsync(CancellationToken cancellationToken);

    }
}
