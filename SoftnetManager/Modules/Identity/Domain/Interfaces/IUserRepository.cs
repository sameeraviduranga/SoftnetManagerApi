using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Interfaces;

namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface IUserRepository:IRepository<User>
    {
        Task<Client?> GetClientByIdAsync(string clientId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        SigningKey? GetActiveSigningKey();




        Task CreateUserProfileAsync(UserProfile userProfile);
        Task CreateUserAsync(User user);
        Task AssignRoleAsync(User user, Role role);

        Task<Role?> GetRole(string roleName);
        Task AssignUserRoleAsync(UserRole userRole);
        Task<bool> IsRoleExists(int roleId);



        Task<bool> IsEmailExists(string email);
        Task<bool> IsNicExists(string Nic);

        Task<bool> CheckRegisteredEmail(User user,string email);
        Task<bool> CheckRegisteredNic(User user,string nic);
        Task<bool> UpdateUserAsync(User existingUser);

        Task<bool> IsSalutationExists(int? salutationId);
        Task<bool> IsGenderExists(int? genderId);
        Task<bool> IsMaritialStatusExists(int? maritialStatusId);
        Task<bool> IsAddressExists(int? addressId);
        Task<bool> IsBranchExists(int? branchId);
        Task<bool> IsDesignationExists(int? designationId);
        

        
        
    }
}
