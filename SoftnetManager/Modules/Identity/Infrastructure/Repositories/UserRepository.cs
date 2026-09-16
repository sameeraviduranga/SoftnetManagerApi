using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Repositories;

namespace SoftnetManager.Modules.Identity.Infrastructure.Repositories
{

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
           // await _context.SaveChangesAsync();
            
        }

        public void CreateUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            //await _context.SaveChangesAsync();
            
            
        }

        public async Task<SigningKey?> GetActiveSigningKeyAsync(CancellationToken cancellationToken)
        {
            return await _context.SigningKeys.FirstOrDefaultAsync(k=>k.IsActive,cancellationToken);
        }

        public async Task<Client?> GetClientByIdAsync(string clientId, CancellationToken cancellationToken)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId,cancellationToken);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u=>u.UserProfile)
                .Include(u=>u.UserRoles)
                    .ThenInclude(ur=>ur.Role)
                        .ThenInclude(r=>r.RolePermissions)
                            .ThenInclude(p=>p.Permission)
                .FirstOrDefaultAsync(u=>u.Email == email,cancellationToken);

        }

        public async Task<Role?> GetRole(string roleName, CancellationToken cancellationToken)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName,cancellationToken);
        }

        public async Task<bool> IsEmailExistAsync(string email,CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u=>u.Email == email,cancellationToken);
        }

        public async Task<bool> CheckRegisteredEmailAsync(User user, string email,CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u=>u.Email ==  email && u.ID != user.ID,cancellationToken);
        }

        public async Task<bool> CheckRegisteredNicAsync(int userId, string nic, CancellationToken cancellationToken)
        {
            return await _context.Users.AnyAsync(u=>u.UserProfile.Nic == nic && u.ID != userId,cancellationToken);
        }

        public void UpdateUser(User existingUser)
        {
            _context.Users.Update(existingUser);
            //await _context.SaveChangesAsync();
            //return true;
        }

        public async Task<bool> IsSalutationExistAsync(int salutationId, CancellationToken cancellationToken)
        {
            return await _context.Salutations.AnyAsync(s=>s.ID == salutationId,cancellationToken);
        }

        public async Task<bool> IsGenderExistAsync(int genderId, CancellationToken cancellationToken)
        {
            return await _context.Genders.AnyAsync(g => g.ID == genderId,cancellationToken);
        }

        public async Task<bool> IsMaritialStatusExistAsync(int maritialStatusId, CancellationToken cancellationToken)
        {
            return await _context.MaritialStatuses.AnyAsync(m=>m.ID == maritialStatusId,cancellationToken);
        }

        public async Task<bool> IsAddressExistAsync(int? addressId,CancellationToken cancellationToken)
        {
            return await _context.Addresses.AnyAsync(a=>a.ID == addressId,cancellationToken);
        }

        public async Task<bool> IsBranchExistAsync(int branchId, CancellationToken cancellationToken)
        {
            return await _context.Branches.AnyAsync(b=>b.ID == branchId,cancellationToken);
        }

        public async Task<bool> IsDesignationExistAsync(int designationId, CancellationToken cancellationToken)
        {
            return await _context.Designations.AnyAsync(d=>d.ID == designationId,cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.AsNoTracking()
                .Include(u => u.UserProfile)    
                    .ThenInclude(up=>up.Salutation)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Gender)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.MaritialStatus)
                .Include(u => u.UserProfile)
                    //.ThenInclude(up => up.Address)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Branch)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Designation)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Salutation)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Gender)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.MaritialStatus)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Address)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Branch)
                .Include(u => u.UserProfile)
                    .ThenInclude(up => up.Designation)
               .Include(u => u.UserRoles)
                   .ThenInclude(ur => ur.Role)
                       .ThenInclude(r => r.RolePermissions)
                           .ThenInclude(p => p.Permission)
               .FirstOrDefaultAsync(u => u.ID == id,cancellationToken);
        }

        
        public async Task<bool> IsNicExistAsync(string Nic, CancellationToken cancellationToken)
        {
           return await _context.Users.AnyAsync(u=>u.UserProfile.Nic == Nic,cancellationToken);
        }
        public void UpdateUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
        }
    }
}
