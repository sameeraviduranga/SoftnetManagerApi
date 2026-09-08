using Azure;
using Microsoft.AspNetCore.JsonPatch;
using SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister;
using SoftnetManager.Modules.Identity.Application.DTOs.User;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Application.Result;

namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<TokenResponseDTO>> Login(LoginDTO loginDTO);
        Task<Result<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO);

        Task<Result<UserDTO>> RegisterUser(RegisterDTO registerDTO);
        Task<Result<UserDTO>> CreateUser(CreateUserDTO userDTO);
        Task<Result<UserDTO>> UpdateUser(int userId, JsonPatchDocument<UpdateProfileDTO>patchDocument);
        Task<Result<UserDTO>> UpdateUserProfileAsync(int userId, JsonPatchDocument<UpdateProfileDTO> patchDocument);

        Task<Result<IEnumerable<UserDTO>>> GetUsersAsync();

        Task<Result<bool>> ToggleUserActiveStatusAsync(ToggleActiveStatusDTO toggleActiveStatusDTO);



    }
}
