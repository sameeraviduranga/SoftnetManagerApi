using AutoMapper;
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

        public UserService(IUserRepository userRepository,ITokenService tokenService,IRefreshTokenRepository refreshTokenRepository,IFileStorageService fileStorageService,IMapper mapper,IHttpContextAccessor httpContextAccessor,IUnitOfWork unitOfWork,ICurrentUserService currentUser)
        {
            this.userRepository = userRepository;
            this.tokenService = tokenService;
            this.refreshTokenRepository = refreshTokenRepository;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetUsersAsync()
        {
            var users = await userRepository.GetAllUsersAsync();
            var userDTOs = mapper.Map<IEnumerable<UserDTO>>(users);

            if (userDTOs == null)
            {
                return Result<IEnumerable<UserDTO>>.Fail("Users are not found");
            }

            return Result<IEnumerable<UserDTO>>.Success(userDTOs);
        }

        public async Task<Result<TokenResponseDTO>> Login(LoginDTO loginDTO)
        {
           var client  = await userRepository.GetClientByIdAsync(loginDTO.ClientId);
            if (client == null)
            {
                return Result<TokenResponseDTO>.Fail("Invalid Client Credentials");
            }

            var user = await userRepository.GetUserByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return Result<TokenResponseDTO>.Fail("Invalid credentials.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);

            if (!isPasswordValid)
            {
                return Result<TokenResponseDTO>.Fail("Invalid credentials.");
            }

            var token = tokenService.GenerateJwtToken(user,client);

            var refreshToken = tokenService.GenerateRefreshToken();
            var hashedRefreshToken = tokenService.HashToken(refreshToken);

            var newRefreshToken = new RefreshToken
            {
                Token = hashedRefreshToken,
                UserId = user.ID,
                ClientId=client.Id,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),

            };

            await refreshTokenRepository.AddAsync(newRefreshToken);

            return Result<TokenResponseDTO>.Success(new TokenResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
            });

        }

        public async Task<Result<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            var refreshToken = refreshTokenRequestDTO.RefreshToken;
            var clientId = refreshTokenRequestDTO.ClientId;


            var storedRefreshToken = await refreshTokenRepository.GetStoredRefreshTokenAsync(tokenService.HashToken(refreshToken), clientId);

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

            await refreshTokenRepository.RevokedRefreshTokenAsync(storedRefreshToken);

            var user = storedRefreshToken.User;
            var client = storedRefreshToken.Client;

            var newAccessToken =  tokenService.GenerateJwtToken(user,client);
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

            await refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            return Result<TokenResponseDTO>.Success(new TokenResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,

            });


        }

        public async Task<Result<UserDTO>> RegisterUser(RegisterDTO registerDTO)
        {
            try
            {
                var emailExists = await userRepository.IsEmailExists(registerDTO.Email);
                if (emailExists)
                {
                    return Result<UserDTO>.Fail("Email already registered.");
                }

                var user = mapper.Map<User>(registerDTO);

                await unitOfWork.BeginTransactionAsync();

                //unitOfWork.Users.CreateUserProfileAsync(user.UserProfile);

                //await userRepository.CreateUserProfileAsync(user.UserProfile);

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

                user.Password = hashedPassword;
                user.UserProfile.CreatedBy = 1;


                
                //unitOfWork.Users.CreateUserAsync(user);
                await unitOfWork.Users.AddAsync(user);

                var role = await unitOfWork.Users.GetRole("User");
                if (role == null)
                {
                    return Result<UserDTO>.Fail("Role Not Found");
                }
                await unitOfWork.Users.AssignRoleAsync(user, role);

                //another solution to save userrole
                //user.UserRoles.Add(new UserRole
                //{
                //    User = user,
                //    Role = role
                //});

                

                await unitOfWork.CommitTransactionAsync();//commit transaction

                var userDto = mapper.Map<UserDTO>(user);
                //need mapping
                return Result<UserDTO>.Success(userDto);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result<UserDTO>.Fail(ex.Message);
            }


        }

        public async Task<Result<bool>> ToggleUserActiveStatusAsync(ToggleActiveStatusDTO toggleActiveStatusDTO)
        {
            try
            {
                var userId = currentUser.UserID;

                await unitOfWork.BeginTransactionAsync();
                var existingUser = await unitOfWork.Users.GetUserByIdAsync(toggleActiveStatusDTO.UserId);


                if (existingUser == null || existingUser.UserProfile == null)
                {
                    return Result<bool>.Fail("User not found");
                }
                

                existingUser.UserProfile.IsActive = toggleActiveStatusDTO.IsActive;
                existingUser.UserProfile.UpdatedBy = 1;//replace with userId from current 
                existingUser.UserProfile.UpdatedAt = DateTime.UtcNow;

                unitOfWork.Users.Update(existingUser);
                await unitOfWork.CommitTransactionAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync();
                return Result<bool>.Fail(ex.Message);
            }
        }

        //=======================================================toggle user 


        //==============================================update user ==================
        public async Task<Result<UserDTO>> UpdateUser(int userId, JsonPatchDocument<UpdateProfileDTO> patchDocument)
        {
            var existingUser = await userRepository.GetUserByIdAsync(userId);
            if (existingUser == null || existingUser.UserProfile== null)
            {
                return Result<UserDTO>.Fail("User not  found");
            }

            var userProfileDto = mapper.Map<UpdateProfileDTO>(existingUser.UserProfile);

            //validation patch operation
            foreach (var operation in patchDocument.Operations)
            {
                if (operation.path.Equals("/SalutationID",StringComparison.OrdinalIgnoreCase) && operation.value != null)
                {
                    int salutionId = Convert.ToInt32(operation.value);
                    if (!await userRepository.IsSalutationExists(salutionId))
                    {
                       return  Result<UserDTO>.Fail("Salution not exists.");
                    }
                }
                if (operation.path.Equals("/GenderID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                {
                    int genderID = Convert.ToInt32(operation.value);
                    if (!await userRepository.IsGenderExists(genderID))
                    {
                        return Result<UserDTO>.Fail("Gender not exists.");
                    }
                }
                if (operation.path.Equals("/MaritialStatusID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                {
                    int maritialStatusID = Convert.ToInt32(operation.value);
                    if (!await userRepository.IsMaritialStatusExists(maritialStatusID))
                    {
                        return Result<UserDTO>.Fail("MaritialStatus not exists.");
                    }
                }
                if (operation.path.Equals("/BranchID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                {
                    int branchID = Convert.ToInt32(operation.value);
                    if (!await userRepository.IsBranchExists(branchID))
                    {
                        return Result<UserDTO>.Fail("Branch not exists.");
                    }
                }
                if (operation.path.Equals("/DesignationID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                {
                    int designationID = Convert.ToInt32(operation.value);
                    if (!await userRepository.IsDesignationExists(designationID))
                    {
                        return Result<UserDTO>.Fail("designation not exists.");
                    }
                }

            }

            //apply patch
            patchDocument.ApplyTo(userProfileDto);

            //validate dto
            ValidationContext validationContext = new ValidationContext(userProfileDto);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(userProfileDto,validationContext,validationResults,true))
            {
                return Result<UserDTO>.Fail(string.Join(", ", validationResults.Select(x => x.ErrorMessage)));
            }

            //map back to entity

            mapper.Map(userProfileDto, existingUser.UserProfile);

            userRepository.UpdateUserAsync(existingUser);

            
            //if (!isUpdate)
            //{
            //    return Result<UserDTO>.Fail("Failed to update user.");
            //}
            
            var userDto = mapper.Map<UserDTO>(existingUser);

            return Result<UserDTO>.Success(userDto);

        }


        public async Task<Result<UserDTO>> UpdateUserProfileAsync(int userId,JsonPatchDocument<UpdateProfileDTO> patchDocument)
        {
            var curretUserId = currentUser.UserID;
            try
            {
                await unitOfWork.BeginTransactionAsync();

                var existingUser = await unitOfWork.Users.GetUserByIdAsync(userId);
                if (existingUser == null || existingUser.UserProfile == null)
                {
                    return Result<UserDTO>.Fail("User not found");
                }

                var userProfleDto = mapper.Map<UpdateProfileDTO>(existingUser.UserProfile);

                //validation
                foreach (var operation in patchDocument.Operations)
                {
                    if (operation.path.Equals("/SalutationID",StringComparison.OrdinalIgnoreCase) && operation.value != null)
                    {
                        var salutationId = Convert.ToInt32(operation.value);

                        if (!await userRepository.IsSalutationExists(salutationId))
                        {
                            return Result<UserDTO>.Fail("Salution not exists.");
                        }
                    }

                    if (operation.path.Equals("/GenderID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                    {
                        int genderID = Convert.ToInt32(operation.value);
                        if (!await userRepository.IsGenderExists(genderID))
                        {
                            return Result<UserDTO>.Fail("Gender not exists.");
                        }
                    }
                    if (operation.path.Equals("/MaritialStatusID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                    {
                        int maritialStatusID = Convert.ToInt32(operation.value);
                        if (!await userRepository.IsMaritialStatusExists(maritialStatusID))
                        {
                            return Result<UserDTO>.Fail("MaritialStatus not exists.");
                        }
                    }
                    if (operation.path.Equals("/BranchID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                    {
                        int branchID = Convert.ToInt32(operation.value);
                        if (!await userRepository.IsBranchExists(branchID))
                        {
                            return Result<UserDTO>.Fail("Branch not exists.");
                        }
                    }
                    if (operation.path.Equals("/DesignationID", StringComparison.OrdinalIgnoreCase) && operation.value != null)
                    {
                        int designationID = Convert.ToInt32(operation.value);
                        if (!await userRepository.IsDesignationExists(designationID))
                        {
                            return Result<UserDTO>.Fail("designation not exists.");
                        }
                    }

                }

                //apply patch
                patchDocument.ApplyTo(userProfleDto);

                //validate dto

                ValidationContext validationContext = new ValidationContext(userProfleDto);
                List<ValidationResult> validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(userProfleDto, validationContext, validationResults, true))
                {
                    return Result<UserDTO>.Fail(string.Join(",", validationResults.Select(x => x.ErrorMessage)));
                }

                //map back to entity
                mapper.Map(userProfleDto, existingUser.UserProfile);

                existingUser.UserProfile.UpdatedBy = 1;//replace with current user id
                existingUser.UserProfile.UpdatedAt = DateTime.UtcNow;

                unitOfWork.Users.Update(existingUser);

                await unitOfWork.CommitTransactionAsync();

                var userDto = mapper.Map<UserDTO>(existingUser);

                return Result<UserDTO>.Success(userDto);


            }
            catch (Exception ex)
            {

                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync(); 
                }
                return Result<UserDTO>.Fail(ex.Message);
            }
        }



        public async Task<Result<UserDTO>> CreateUser(CreateUserDTO userDTO)
        {
            string uploadedImagePath = string.Empty;
            try
            {
                if (userDTO == null || userDTO.ProfileDTO == null)
                {
                    return Result<UserDTO>.Fail("Not valid dto");
                }

                ValidationContext validationContext = new ValidationContext(userDTO);
                List<ValidationResult> validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(userDTO, validationContext, validationResults, true))
                {
                    return Result<UserDTO>.Fail(string.Join(",", validationResults.Select(x => x.ErrorMessage)));
                }

                if (await userRepository.IsEmailExists(userDTO.Email))
                {
                    return Result<UserDTO>.Fail("Email aleady registerd.");
                }

                if (await userRepository.IsNicExists(userDTO.ProfileDTO.Nic))
                {
                    return Result<UserDTO>.Fail("Nic aleady registerd.");
                }

                if (!await userRepository.IsSalutationExists(userDTO.ProfileDTO.SalutationID))
                {
                    return Result<UserDTO>.Fail("Salutation doesn't exists");
                }

                if (!await userRepository.IsGenderExists(userDTO.ProfileDTO.GenderID))
                {
                    return Result<UserDTO>.Fail("Gender doesn't exists");
                }

                if (!await userRepository.IsBranchExists(userDTO.ProfileDTO.BranchID))
                {
                    return Result<UserDTO>.Fail("Branch doesn't exists");
                }

                if (!await userRepository.IsDesignationExists(userDTO.ProfileDTO.DesignationID))
                {
                    return Result<UserDTO>.Fail("Designation doesn't exists");
                }



                var user = mapper.Map<User>(userDTO);

                if (userDTO.ProfileDTO.UserPhoto != null)
                {
                    uploadedImagePath = await fileStorageService.UploadProfileImageAsync(userDTO.ProfileDTO.UserPhoto);
                    user.UserProfile.ProfileImageUrl = uploadedImagePath;
                }
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                user.Password = hashedPassword;
                user.UserProfile.CreatedBy = 1;
                user.UserProfile.CreatedAt = DateTime.UtcNow;

                //map
                await unitOfWork.BeginTransactionAsync();


                unitOfWork.Users.CreateUser(user);

                //need save profile

                //asignRole

                var distinctRoles = userDTO.roles.Distinct().ToList();
                foreach (var role in distinctRoles)
                {
                    if (!await userRepository.IsRoleExists(role))
                    {
                        return Result<UserDTO>.Fail($"Role '{role}' does not exist.");
                    }
                }

                foreach (var role in distinctRoles)
                {
                    
                     user.UserRoles.Add(new UserRole {RoleID = role });

                }
                

                await unitOfWork.CommitTransactionAsync();

                var newUser = await userRepository.GetUserByIdAsync(user.ID);

                var createdUserDto = mapper.Map<UserDTO>(newUser);

                return Result<UserDTO>.Success(createdUserDto);
            }
            catch (Exception ex)
            {
                if (unitOfWork.HasActiveTransaction)
                {
                    await unitOfWork.RollbackTransactionAsync();
                }

                if (!string.IsNullOrEmpty(uploadedImagePath))
                {
                    await fileStorageService.DeleteProfileImageAsync(uploadedImagePath);
                }
                return Result<UserDTO>.Fail(ex.Message);
            }

        }


    }
}
