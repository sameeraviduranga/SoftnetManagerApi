using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Api.Response;
using SoftnetManager.Modules.Identity.Application.Result;
using SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister;


namespace SoftnetManager.Modules.Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
           
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid request body"));
            }

            var result = await userService.Login(loginDTO);

            if (!result.IsSuccess)
            {
                return Unauthorized(ApiResponse<object>.Fail(result.Error!,"Login Failed")); 
            }

            return Ok(ApiResponse<TokenResponseDTO>.Success(result.Data!, "Login successful"));
            

        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO requestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail(ModelState, "Invalid request body"));
            }

            var result = await userService.RefreshToken(requestDTO);

            if (!result.IsSuccess)
            {
                return Unauthorized(ApiResponse<object>.Fail(result.Error ?? "Session expired. Please log in again.", "Authentication failed."));
            }

            return Ok(ApiResponse<TokenResponseDTO>.Success(result.Data!, "Token refreshed successfully."));

        }

    }
}
