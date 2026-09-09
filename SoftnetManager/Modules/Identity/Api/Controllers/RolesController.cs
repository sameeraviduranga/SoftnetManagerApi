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

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid model state"));
            }

            var result = await roleService.CreateRoleAsync(createRoleDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "Failed to create role"));

            }

            return Ok(ApiResponse<object>.Success(result, "Role created successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid model state"));
            }

            var result = await roleService.UpdateRoleAsync(updateRoleDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "Failed to Update role"));
            }
            

            return Ok(ApiResponse<object>.Success(result, "Role Updated successfully"));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute]int id)
        {
            var result = await roleService.DeleteRoleAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!,"Failed to delete Role"));
            }

            return Ok(ApiResponse<object>.Success(result.Data,"Role Deleted Successfully."));
        }
    }
}
