using AutoMapper;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using SoftnetManager.Modules.Identity.Api.Response;
using SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister;
using SoftnetManager.Modules.Identity.Application.DTOs.User;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Application.Validators;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Database;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SoftnetManager.Modules.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO,[FromServices]IValidator<RegisterDTO>validator,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserDTO>.Fail(ModelState,"Invalid request body"));
            }

            var validationResult = await validator.ValidateAsync(registerDTO);
            if (!validationResult.IsValid)
            {
                var validationResponse = validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                });

                return BadRequest(ApiResponse<UserDTO>.Fail(validationResponse));
            }

            var result = await userService.RegisterUserAsync(registerDTO,cancellationToken);

            if (!result.IsSuccess)
            {
                return Unauthorized(ApiResponse<UserDTO>.Fail(result.Error!,"User not registered"));
            }

            return Ok(ApiResponse<UserDTO>.Success(result.Data!,"User registered successfully"));


            //return CreatedAtAction(nameof(GetProfile), new { id = newUser.Id }, new { message = "User registered successfully." });
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await userService.GetUsersAsync(cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<IEnumerable<UserDTO>>.Fail(result!, result.Error!));
            }

            return Ok(ApiResponse<IEnumerable<UserDTO>>.Success(result.Data!, "Users are retrieved"));
        }

        //[HttpGet("Profile")]
        //[Authorize]
        //public async Task<IActionResult> GetProfile()
        //{
        //    var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

        //    if (emailClaim == null)
        //    {
        //        return Unauthorized(new { Message = "Invalid token: Email claim missing." });
        //    }

        //    var user = await _context.Users
        //        .Include(u => u.UserRoles)
        //            .ThenInclude(ur => ur.Role)
        //        .FirstOrDefaultAsync(u => u.Email.ToLower() == emailClaim.Value.ToLower());

        //    if (user == null)
        //    {
        //        return NotFound(new { message = "User not found." });
        //    }

        //    var profileDto = _mapper.Map<ProfileDTO>(user);

        //    return Ok(profileDto);
        //}

        
        //[HttpPatch("{userId}")]
        //public async Task<IActionResult> UpdateProfile(int userId, [FromForm]UpdateProfileDTO updateProfileDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ApiResponse<object>.Fail(ModelState,"Invalid Request body"));
        //    }

        //    var result = await userService.UpdateUserProfileAsync(userId, updateProfileDTO);

        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(ApiResponse<object>.Fail(result.Error!, "User not updated"));
        //    }


        //    //var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        //    //var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);


        //    //if (emailClaim == null)
        //    //{
        //    //    return Unauthorized(ApiResponse<object>.Fail("Invalid token: Email claim missing."));
        //    //}

        //    //emailClaim.Value.ToString()

        //    //var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        //    //if (userIdClaim == null)
        //    //{
        //    //    return Unauthorized(ApiResponse<UserDTO>.Fail("User Unauthorized"));
        //    //}

        //    //var userId = Convert.ToInt32(userIdClaim.Value);
        //    return Ok(ApiResponse<object>.Success(result.Data, "User profile updated successfully"));
            
        
        //}

        [HttpPut("toggleActiveStatus")]
        public async Task<IActionResult> ToggleActiveStatus([FromBody] ToggleActiveStatusDTO toggleActiveStatusDTO,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid request body"));
            }

            var result = await userService.ToggleUserActiveStatusAsync(toggleActiveStatusDTO,cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "Failed to toggle user active status"));
            }

            return Ok(ApiResponse<object>.Success(result.Data!, "User active status toggled successfully"));
        }


        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromForm]CreateUserDTO createUserDTO, IValidator<CreateUserDTO> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(createUserDTO);
            if (!validationResult.IsValid)
            {
                var errorResponse = validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage,

                });
                return BadRequest(ApiResponse<object>.Fail(new {Errors=errorResponse}));
            }
            var result = await userService.CreateUserAsync(createUserDTO,cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "User not created"));
            }
            return Ok(ApiResponse<UserDTO>.Success(result.Data!, "User created successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute]int id,CancellationToken cancellationToken)
        {
            
            var result = await userService.DeleteUserAsync(id,cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Error!, "User not Deleted"));
            }
            return Ok(ApiResponse<object>.Success(result.Data!, "User deleted successfully"));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id,JsonPatchDocument<UpdateUserProfileDTO>jsonPatchDoc,CancellationToken cancellationToken)
        {
            var result = await userService.UpdateUserProfileAsync(id, jsonPatchDoc, cancellationToken);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<UserDTO>.Fail(result.Error!,"Fail to update User"));
            }

            return Ok(ApiResponse<UserDTO>.Success(result.Data, "User Updated Successfully."));
        }

    }
}
