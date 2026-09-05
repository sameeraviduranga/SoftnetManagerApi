using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Repositories;

namespace SoftnetManager.Modules.Identity.Infrastructure.Repositories
{

    public class UserRespository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRespository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task AssignRoleAsync(User user, Role role)
        {
            var userRole = new UserRole
            {
                UserID = user.ID,
                RoleID = role.Id,
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

        }

        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
        }

        public async Task CreateUserProfileAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
            
            
        }

        public SigningKey? GetActiveSigningKey()
        {
            return _context.SigningKeys.FirstOrDefault(k=>k.IsActive);
        }

        public async Task<Client?> GetClientByIdAsync(string clientId)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u=>u.UserProfile)
                .Include(u=>u.UserRoles)
                    .ThenInclude(ur=>ur.Role)
                        .ThenInclude(r=>r.RolePermissions)
                            .ThenInclude(p=>p.Permission)
                .FirstOrDefaultAsync(u=>u.Email == email);

        }

        public async Task<Role?> GetRole(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u=>u.Email == email);
        }

        public async Task<bool> CheckRegisteredEmail(User user, string email)
        {
            return await _context.Users.AnyAsync(u=>u.Email ==  email && u.ID != user.ID);
        }

        public async Task<bool> CheckRegisteredNic(User user, string nic)
        {
            return await _context.Users.AnyAsync(u=>u.UserProfile.Nic == nic && u.ID != user.ID);
        }

        public async Task<bool> UpdateUserAsync(User existingUser)
        {
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSalutationExists(int? salutationId)
        {
            return await _context.Salutations.AnyAsync(s=>s.ID == salutationId);
        }

        public async Task<bool> IsGenderExists(int? genderId)
        {
            return await _context.Genders.AnyAsync(g => g.ID == genderId);
        }

        public async Task<bool> IsMaritialStatusExists(int? maritialStatusId)
        {
            return await _context.MaritialStatuses.AnyAsync(m=>m.ID == maritialStatusId);
        }

        public async Task<bool> IsAddressExists(int? addressId)
        {
            return await _context.Address.AnyAsync(a=>a.ID == addressId);
        }

        public async Task<bool> IsBranchExists(int? branchId)
        {
            return await _context.Branches.AnyAsync(b=>b.ID == branchId);
        }

        public async Task<bool> IsDesignationExists(int? designationId)
        {
            return await _context.Designations.AnyAsync(d=>d.ID == designationId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking()
                .Include(u => u.UserProfile)    
                    .ThenInclude(up=>up.Salutation)
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
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
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
               .FirstOrDefaultAsync(u => u.ID == id);
        }

        public async Task AssignUserRoleAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsNicExists(string Nic)
        {
           return await _context.Users.AnyAsync(u=>u.UserProfile.Nic == Nic);
        }

        public async Task<bool> IsRoleExists(int roleId)
        {
            return await _context.Roles.AnyAsync(r=>r.Id == roleId);
        }
    }
}
