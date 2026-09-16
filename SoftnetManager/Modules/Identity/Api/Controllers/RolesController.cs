using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoftnetManager.Modules.Identity.Api.Response;
using SoftnetManager.Modules.Identity.Application.DTOs.Role;
using SoftnetManager.Modules.Identity.Application.Interfaces;

namespace SoftnetManager.Modules.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            var result = await roleService.GetAllRolesAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!,"role retrieval not success."));
            }

            return Ok(ApiResponse<IEnumerable<RoleResponseDto>>.Success(result.Data, "Roles Retreived Successfully."));

        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleDto createRoleDto,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid model state"));
            }

            var result = await roleService.CreateRoleAsync(createRoleDto,cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "Failed to create role"));

            }

            return Ok(ApiResponse<object>.Success(result, "Role created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] UpdateRoleDto updateRoleDto,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid model state"));
            }

            var result = await roleService.UpdateRoleAsync(updateRoleDto,cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "Failed to Update role"));
            }
            

            return Ok(ApiResponse<object>.Success(result, "Role Updated successfully"));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute]int id,CancellationToken cancellationToken)
        {
            var result = await roleService.DeleteRoleAsync(id,cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!,"Failed to delete Role"));
            }

            return Ok(ApiResponse<object>.Success(result.Data,"Role Deleted Successfully."));
        }
    }
}
