using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IUserRepository:IRepository<User>
    {
        Task<Client?> GetClientByIdAsync(string clientId,CancellationToken cancellationToken);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<SigningKey?> GetActiveSigningKeyAsync(CancellationToken cancellationToken);



        void CreateUser(User user);
        void CreateUserProfile(UserProfile userProfile);
        void UpdateUser(User existingUser);
        //Task<UserProfile?> GetUserProfileByUserIdAsync(int userId);
        void UpdateUserProfile(UserProfile userProfile);

        Task<Role?> GetRole(string roleName, CancellationToken cancellationToken);
        //Task AssignUserRoleAsync(UserRole userRole);
        



        Task<bool> IsEmailExistAsync(string email,CancellationToken cancellationToken);
        Task<bool> IsNicExistAsync(string Nic, CancellationToken cancellationToken);

        Task<bool> CheckRegisteredEmailAsync(User user,string email, CancellationToken cancellationToken);
        Task<bool> CheckRegisteredNicAsync(int userId,string nic, CancellationToken cancellationToken);
        

        Task<bool> IsSalutationExistAsync(int salutationId, CancellationToken cancellationToken);
        Task<bool> IsGenderExistAsync(int genderId, CancellationToken cancellationToken);
        Task<bool> IsMaritialStatusExistAsync(int maritialStatusId,CancellationToken cancellationToken);
        Task<bool> IsAddressExistAsync(int? addressId, CancellationToken cancellationToken);
        Task<bool> IsBranchExistAsync(int branchId, CancellationToken cancellationToken);
        Task<bool> IsDesignationExistAsync(int designationId, CancellationToken cancellationToken);
        

        
        
    }
}
