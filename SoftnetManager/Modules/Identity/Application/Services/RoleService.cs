using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Application.Result;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Interfaces;
using SoftnetManager.Modules.Shared.Repositories;
using System.Data;
using System.Runtime.InteropServices;

namespace SoftnetManager.Modules.Identity.Application.Services
{
    public class RoleService:IRoleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRoleRepository roleRepository;
        private IRepository<Permission> permissionRepository;

        public RoleService(IUnitOfWork unitOfWork,IRoleRepository roleRepository,IRepository<Permission> permissionRepository)
        {
            this.unitOfWork = unitOfWork;
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;
        }

        public async Task<Result<RoleResponseDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            if (string.IsNullOrEmpty(createRoleDto.Name))
            {
                return Result<RoleResponseDto>.Fail("Role name is required.");
            }

            // Check if the role already exists
            if (await roleRepository.IsRoleExistsAsync(createRoleDto.Name))
            {
                return Result<RoleResponseDto>.Fail("Role already exists.");
            }

            // check if the permissions are valid
            if (createRoleDto.Permissions == null || !createRoleDto.Permissions.Any())
            {
                return Result<RoleResponseDto>.Fail("At least one permission is required.");
            }

            var distinctPermissions = createRoleDto.Permissions.Distinct().ToList();

            //check if the permissions are valid
            List<string> permissionErrors = new List<string>();

            foreach (var permission in distinctPermissions)
            {
                if (!await permissionRepository.ExistsAsync(permission))
                {
                    permissionErrors.Add($"Permission : {permission}\n");
                }
            }

            if (permissionErrors.Any())
            {
                return Result<RoleResponseDto>.Fail($"Invalid permissions:\n{string.Join("", permissionErrors)}");
            }

            // Create the role
            try
            {
                await unitOfWork.BeginTransactionAsync();

                var role = new Role
                {
                    Name = createRoleDto.Name,
                    Description = createRoleDto.Description,
                    RolePermissions = distinctPermissions.Select(p=>new RolePermission { PermissionId = p }).ToList(),
                };

                await unitOfWork.Roles.AddAsync(role);
                await unitOfWork.CommitTransactionAsync();

                //get role with permissions

                var rolePermissions = await roleRepository.GetRolePermissionAsync(role.Id);
                if (rolePermissions == null)
                {
                    return Result<RoleResponseDto>.Fail("Failed to retrieve role permissions after creation.");
                }

                var roleResponseDto = new RoleResponseDto
                {
                    RoleName = role.Name,
                    Description= role.Description,
                    RolePermissions = rolePermissions.Select(rp=>rp.Permission.Name).ToList()
                };

                return Result<RoleResponseDto>.Success(roleResponseDto);


            }
            catch (Exception ex)
            {

                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync();
                }

                return Result<RoleResponseDto>.Fail($"An error occurred while creating the role: {ex.Message}");
            }



        }

        public async Task<Result<object>> DeleteRoleAsync(int roleId)
        {
            
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var existingRole = await unitOfWork.Roles.GetRoleById(roleId);
                if (existingRole == null)
                {
                    return Result<object>.Fail($"Role doesn't found");
                }

                if (existingRole.UserRoles != null && existingRole.UserRoles.Any())
                {
                    return Result<object>.Fail($"Please remove users from this role before delete the role");
                }

                //delete all permisison from rolepermission table

                if (existingRole.RolePermissions != null && existingRole.RolePermissions.Any())
                {
                    existingRole.RolePermissions.Clear();
                }

                unitOfWork.Roles.Delete(existingRole);

                await unitOfWork.CommitTransactionAsync();

                return Result<object>.Success($"Role deleted SuccessFully");


            }
            catch (Exception ex)
            {

                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync();
                }

                return Result<object>.Fail($"An error occurred while updating the role: {ex.Message}");
            }


        }

        public async Task<Result<RoleResponseDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            if (string.IsNullOrEmpty(updateRoleDto.Name))
            {
                return Result<RoleResponseDto>.Fail("Role name is required.");
            }

            //check if the role exists
            if (!await roleRepository.ExistsAsync(updateRoleDto.RoleId))
            {
                return Result<RoleResponseDto>.Fail("Role not found.");
            }

            //check newrole exists by name
            if (await roleRepository.IsRegisteredRole(updateRoleDto.RoleId,updateRoleDto.Name))
            {
                return Result<RoleResponseDto>.Fail("Role name already exists.");
            }

            // check if the permissions are valid
            var distinctPermissions = updateRoleDto.Permissions.Distinct().ToList();
            List<string> permissionErrors = new List<string>();

            foreach (var permission in distinctPermissions)
            {
                if (!await permissionRepository.ExistsAsync(permission))
                {
                    permissionErrors.Add($"Permission : {permission}\n");
                }
            }

            if (permissionErrors.Any())
            {
                return Result<RoleResponseDto>.Fail($"Invalid permissions:\n{string.Join("", permissionErrors)}");
            }

            //new permissions

            try
            {
                await unitOfWork.BeginTransactionAsync();

                var existingRole = await unitOfWork.Roles.GetRoleById(updateRoleDto.RoleId);

                if (existingRole != null)
                {

                    var existingRolePermissions = existingRole.RolePermissions.Select(rp => rp.PermissionId).ToList();

                    // existing වල නැති අලුත් ඒවා (Add කිරීමට)
                    var addNewPermisions = distinctPermissions.Except(existingRolePermissions).ToList();

                    var removeOldPermission = existingRolePermissions.Except(distinctPermissions).ToList();


                    existingRole.Name = updateRoleDto.Name;
                    existingRole.Description = updateRoleDto.Description;

                    // Remove old permissions
                    if (removeOldPermission != null && removeOldPermission.Any())
                    {
                        foreach (var permission in removeOldPermission)
                        {
                            //existingRole.RolePermissions.Remove(new RolePermission { PermissionId = permission });this doenst work
                            var rolePermission = existingRole.RolePermissions.FirstOrDefault(rp => rp.PermissionId == permission);
                            existingRole.RolePermissions.Remove(rolePermission!);
                        }
                    }

                    // Add new permissions
                    if (addNewPermisions.Any())
                    {
                        foreach (var permission in addNewPermisions)
                        {
                            existingRole.RolePermissions.Add(new RolePermission { PermissionId = permission });
                        }
                    }

                    unitOfWork.Roles.Update(existingRole);
                    await unitOfWork.CommitTransactionAsync();

                }

                //get role with permissions

                var rolePermissions = await roleRepository.GetRolePermissionAsync(existingRole.Id);
                if (rolePermissions == null)
                {
                    return Result<RoleResponseDto>.Fail("Failed to retrieve role permissions after creation.");
                }

                var roleResponseDto = new RoleResponseDto
                {
                    RoleName = existingRole.Name,
                    Description = existingRole.Description,
                    RolePermissions = rolePermissions.Select(rp => rp.Permission.Name).ToList()
                };

                return Result<RoleResponseDto>.Success(roleResponseDto);



            }
            catch (Exception ex)
            {
                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync();
                }

                return Result<RoleResponseDto>.Fail($"An error occurred while updating the role: {ex.Message}");
               
            }



        }
    }
}
