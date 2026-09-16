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
        Task<Result<TokenResponseDTO>> LoginAsync(LoginDTO loginDTO,CancellationToken cancellationToken);
        Task<Result<TokenResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken);

        Task<Result<UserDTO>> RegisterUserAsync(RegisterDTO registerDTO, CancellationToken cancellationToken);
        Task<Result<UserDTO>> CreateUserAsync(CreateUserDTO userDTO, CancellationToken cancellationToken);
        Task<Result<UserDTO>> UpdateUserProfileAsync(int userId,JsonPatchDocument<UpdateUserProfileDTO>jsonPatchDoc,CancellationToken cancellationToken);

        Task<Result<object>> DeleteUserAsync(int userId, CancellationToken cancellationToken);

        Task<Result<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken);

        Task<Result<bool>> ToggleUserActiveStatusAsync(ToggleActiveStatusDTO toggleActiveStatusDTO, CancellationToken cancellationToken);



    }
}
