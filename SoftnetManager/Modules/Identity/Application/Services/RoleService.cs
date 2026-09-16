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

        public async Task<Result<RoleResponseDto>> CreateRoleAsync(CreateRoleDto createRoleDto,CancellationToken cancellationToken)
        {

            var distinctPermissions = createRoleDto.Permissions.Distinct().ToList();

            // Create the role
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var role = new Role
                {
                    Name = createRoleDto.Name,
                    Description = createRoleDto.Description,
                    RolePermissions = distinctPermissions.Select(p=>new RolePermission { PermissionId = p }).ToList(),
                };

                unitOfWork.Roles.Add(role);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                //get role with permissions

                var rolePermissions = await roleRepository.GetRolePermissionAsync(role.Id,cancellationToken);
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
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }

                return Result<RoleResponseDto>.Fail($"An error occurred while creating the role: {ex.Message}");
            }



        }

        public async Task<Result<object>> DeleteRoleAsync(int roleId, CancellationToken cancellationToken)
        {
            
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var existingRole = await unitOfWork.Roles.GetRoleByIdAsync(roleId,cancellationToken);
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

                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<object>.Success($"Role deleted SuccessFully");


            }
            catch (Exception ex)
            {

                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }

                return Result<object>.Fail($"An error occurred while updating the role: {ex.Message}");
            }


        }

        public async Task<Result<IEnumerable<RoleResponseDto>>> GetAllRolesAsync(CancellationToken cancellationToken)//try catch
        {
            var roles = await roleRepository.GetAllAsync(cancellationToken);
            if (roles == null)
            {
                return Result<IEnumerable<RoleResponseDto>>.Fail("No roles found");
            }

            var listOfRoles = roles.Select(r => new RoleResponseDto
            {
                RoleName = r.Name,
                Description = r.Description,
            });

            return Result<IEnumerable<RoleResponseDto>>.Success(listOfRoles);
        }

        public async Task<Result<RoleResponseDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto,CancellationToken cancellationToken)
        {

            // check if the permissions are valid
            var distinctPermissions = updateRoleDto.Permissions.Distinct().ToList();
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var existingRole = await unitOfWork.Roles.GetRoleByIdAsync(updateRoleDto.RoleId,cancellationToken);

                if (existingRole != null)
                {
                    return Result<RoleResponseDto>.Fail("Role not found.");
                }

                var existingRolePermissions = existingRole!.RolePermissions.Select(rp => rp.PermissionId).ToList();

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

                        //delete at once
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
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var rolePermissions = await roleRepository.GetRolePermissionAsync(existingRole.Id!, cancellationToken);

                //get role with permissions


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
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }

                return Result<RoleResponseDto>.Fail($"An error occurred while updating the role: {ex.Message}");
               
            }



        }
    }
}
