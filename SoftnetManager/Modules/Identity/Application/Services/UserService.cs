using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using softnetmanager.modules.identity.application.services;
using SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister;
using SoftnetManager.Modules.Identity.Application.DTOs.User;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Application.Result;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SoftnetManager.Modules.Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUser;
        private readonly IRoleRepository roleRepository;
        private readonly IServiceProvider serviceProvider;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IFileStorageService fileStorageService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, ICurrentUserService currentUser,IRoleRepository roleRepository,IServiceProvider serviceProvider)
        {
            this.userRepository = userRepository;
            this.tokenService = tokenService;
            this.refreshTokenRepository = refreshTokenRepository;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
            this.roleRepository = roleRepository;
            this.serviceProvider = serviceProvider;
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var users = await userRepository.GetAllUsersAsync(cancellationToken);
            var userDTOs = mapper.Map<IEnumerable<UserDTO>>(users);

            if (userDTOs == null)
            {
                return Result<IEnumerable<UserDTO>>.Fail("Users are not found");
            }

            return Result<IEnumerable<UserDTO>>.Success(userDTOs);
        }

        public async Task<Result<TokenResponseDTO>> LoginAsync(LoginDTO loginDTO,CancellationToken cancellationToken)
        {
            var client = await userRepository.GetClientByIdAsync(loginDTO.ClientId,cancellationToken);
            if (client == null)
            {
                return Result<TokenResponseDTO>.Fail("Invalid Client Credentials");
            }

            var user = await userRepository.GetUserByEmailAsync(loginDTO.Email,cancellationToken);
            if (user == null)
            {
                return Result<TokenResponseDTO>.Fail("Invalid credentials.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);

            if (!isPasswordValid)
            {
                return Result<TokenResponseDTO>.Fail("Invalid credentials.");
            }

            var token = await tokenService.GenerateJwtTokenAsync(user, client,cancellationToken);

            var refreshToken = tokenService.GenerateRefreshToken();
            var hashedRefreshToken = tokenService.HashToken(refreshToken);

            var newRefreshToken = new RefreshToken
            {
                Token = hashedRefreshToken,
                UserId = user.ID,
                ClientId = client.Id,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),

            };

            await refreshTokenRepository.AddAsync(newRefreshToken,cancellationToken);

            return Result<TokenResponseDTO>.Success(new TokenResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
            });

        }

        public async Task<Result<TokenResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequestDTO, CancellationToken cancellationToken)
        {
            var refreshToken = refreshTokenRequestDTO.RefreshToken;
            var clientId = refreshTokenRequestDTO.ClientId;


            var storedRefreshToken = await refreshTokenRepository.GetStoredRefreshTokenAsync(tokenService.HashToken(refreshToken), clientId,cancellationToken);

            if (storedRefreshToken == null)
            {
                return Result<TokenResponseDTO>.Fail("Invalid refresh token.");

            }

            if (storedRefreshToken.IsRevoked)
            {
                return Result<TokenResponseDTO>.Fail("Refresh token has been revoked.");
            }

            if (storedRefreshToken.ExpiresAt < DateTime.Now)
            {
                return Result<TokenResponseDTO>.Fail("Refresh token has expired.");
            }

            //remove old token

            await refreshTokenRepository.RevokedRefreshTokenAsync(storedRefreshToken,cancellationToken);

            var user = storedRefreshToken.User;
            var client = storedRefreshToken.Client;

            var newAccessToken = await tokenService.GenerateJwtTokenAsync(user, client,cancellationToken);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            var hashedRefreshToken = tokenService.HashToken(newRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = hashedRefreshToken,
                UserId = user.ID,
                ClientId = client.Id,
                IsRevoked = false,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(7),

            };

            await refreshTokenRepository.AddAsync(newRefreshTokenEntity,cancellationToken);

            return Result<TokenResponseDTO>.Success(new TokenResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,

            });


        }

        public async Task<Result<UserDTO>> RegisterUserAsync(RegisterDTO registerDTO,CancellationToken cancellationToken)
        {

            var user = mapper.Map<User>(registerDTO);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            user.Password = hashedPassword;
            //user.UserProfile.CreatedBy = 1; null


            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                unitOfWork.Users.Add(user);
                user.UserRoles.Add(new UserRole { RoleID = 1 });

                await unitOfWork.CommitTransactionAsync(cancellationToken);
                
                //get user
                var createdUser = await userRepository.GetUserByIdAsync(user.ID,cancellationToken);
                if (createdUser == null)
                {
                    return Result<UserDTO>.Fail("Created User Not found");
                }
                var userDto = mapper.Map<UserDTO>(createdUser);

                return Result<UserDTO>.Success(userDto);

            }
            catch (Exception ex)
            {

                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }

                return Result<UserDTO>.Fail(ex.Message);
            }

        }

        public async Task<Result<bool>> ToggleUserActiveStatusAsync(ToggleActiveStatusDTO toggleActiveStatusDTO, CancellationToken cancellationToken)
        {
            try
            {
                var userId = currentUser.UserID;

                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var existingUser = await unitOfWork.Users.GetUserByIdAsync(toggleActiveStatusDTO.UserId,cancellationToken);


                if (existingUser == null || existingUser.UserProfile == null)
                {
                    return Result<bool>.Fail("User not found");
                }


                existingUser.UserProfile.IsActive = toggleActiveStatusDTO.IsActive;
                existingUser.UserProfile.UpdatedBy = 1;//replace with userId from current 
                existingUser.UserProfile.UpdatedAt = DateTime.UtcNow;

                unitOfWork.Users.Update(existingUser);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }
                return Result<bool>.Fail(ex.Message);
            }
        }


        //public async Task<Result<UserDTO>> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO)
        //{

        //}



        public async Task<Result<UserDTO>> CreateUserAsync(CreateUserDTO userDTO, CancellationToken cancellationToken)
        {
            var user = mapper.Map<User>(userDTO);
            var photopath = string.Empty;
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var hashPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                user.Password = hashPassword;

                if (userDTO.UserProfileDto.UserPhoto != null)
                {
                    photopath = await fileStorageService.UploadProfileImageAsync(userDTO.UserProfileDto.UserPhoto);
                    user.UserProfile.ProfileImageUrl = photopath;
                }

                //add roles
                foreach (var role in userDTO.roles.Distinct().ToList())
                {
                    user.UserRoles.Add(new UserRole { RoleID = role });
                }

                user.UserProfile.CreatedBy = 1;
                

                unitOfWork.Users.Add(user);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                var createdUser = await userRepository.GetUserByIdAsync(user.ID,cancellationToken);
                var dto = mapper.Map<UserDTO>(createdUser);
                return Result<UserDTO>.Success(dto);

            }
            catch (Exception ex)
            {
                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }
                if (!string.IsNullOrEmpty(photopath))
                {
                    await fileStorageService.DeleteProfileImageAsync(photopath);
                }
                return Result<UserDTO>.Fail(ex.Message);

            }
        }

        public async Task<Result<object>> DeleteUserAsync(int userId, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetByIdAsync(userId,cancellationToken);

            if (existingUser == null)
            {
                return Result<object>.Fail("User not found");
            }

            userRepository.Delete(existingUser);
            await userRepository.SaveAsync(cancellationToken);

            return Result<object>.Success($"User : {existingUser.Email} deleted successfully.");
        }

        public async Task<Result<UserDTO>> UpdateUserProfileAsync(int userId, JsonPatchDocument<UpdateUserProfileDTO> jsonPatchDoc, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await unitOfWork.Users.GetUserByIdAsync(userId, cancellationToken);
                if (existingUser == null || existingUser.UserProfile == null)
                {
                    return Result<UserDTO>.Fail("User not found");
                }
                //map
                var updateDto = mapper.Map<UpdateUserProfileDTO>(existingUser.UserProfile);

                //apply patch
                jsonPatchDoc.ApplyTo(updateDto);

                //validation
                var validator = serviceProvider.GetRequiredService<IValidator<UpdateUserProfileDTO>>();
                var validationResult = await validator.ValidateAsync(updateDto, cancellationToken);

                if (!validationResult.IsValid)
                {
                    var errorResponse = validationResult.Errors.Select(e => new
                    {
                        Property = e.PropertyName,
                        Error = e.ErrorMessage
                    });

                    return Result<UserDTO>.Fail(string.Join("/n", errorResponse));
                }

                //again map
                mapper.Map(updateDto, existingUser.UserProfile);
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                unitOfWork.Users.Update(existingUser);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                var updatedUser = await userRepository.GetUserByIdAsync(existingUser.ID,cancellationToken);
                var updatedUserDto = mapper.Map<UserDTO>(updatedUser);
                return Result<UserDTO>.Success(updatedUserDto);

            }
            catch (Exception ex)
            {

                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                }

                return Result<UserDTO>.Fail(ex.Message);
            }
        }
    }
}
