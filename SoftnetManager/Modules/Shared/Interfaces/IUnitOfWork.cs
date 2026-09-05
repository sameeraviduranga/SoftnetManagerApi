using Microsoft.EntityFrameworkCore.Storage;
using SoftnetManager.Modules.Identity.Domain.Interfaces;

namespace SoftnetManager.Modules.Shared.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        //Define repositories for each entity
        IUserRepository Users { get; }
        //Iproductrepository Products { get; }
        //Icustomerrepository Customers { get; }


        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}
