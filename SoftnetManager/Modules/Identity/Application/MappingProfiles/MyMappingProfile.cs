using AutoMapper;
using SoftnetManager.Modules.Identity.Application.DTOs.Address;
using SoftnetManager.Modules.Identity.Application.DTOs.LoginAndRegister;
using SoftnetManager.Modules.Identity.Application.DTOs.User;
using SoftnetManager.Modules.Identity.Application.DTOs.UserProfile;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Domain.Entities;

namespace SoftnetManager.Modules.Identity.Application.MappingProfiles
{
    public class MyMappingProfile:Profile 
    {


        public MyMappingProfile()
        {
            //Create address to addressDto

            CreateMap<CreateAddressDTO, Address>();
            CreateMap<Address, AddressDTO>()
                .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Zone != null? src.Zone.ZoneName:null))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LocationStatus.ToString()));

            CreateMap<User, UserDTO>()
                .ForMember(dest=>dest.UserProfile,opt=>opt.MapFrom(src=>src.UserProfile))
                .ForMember(dest=>dest.UserRoles, opt => opt.MapFrom(src=>src.UserRoles != null?src.UserRoles.Where(ur=>ur.Role != null).Select(ur=>ur.Role.Name).ToList():new List<string>()));

            CreateMap<User, UserProfileDTO>();

            CreateMap<UserProfile, UserProfileDTO>()
                .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.Salutation != null? src.Salutation.SalutationName:null))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender != null? src.Gender.GenderName:null))
                .ForMember(dest => dest.MaritialStatus, opt => opt.MapFrom(src => src.MaritialStatus != null?src.MaritialStatus.Status:null))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch != null? src.Branch.Name:null))
                .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation != null? src.Designation.DesignationName:null))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.HasValue? src.Dob.Value.ToString("yyyy-MM-dd"):null));

            

            CreateMap<RegisterDTO, UserProfile>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src));



            CreateMap<UpdateProfileDTO, UserProfile>()
                .ForMember(dest=>dest.Address,opt=>opt.MapFrom(src => src.Address))
                .ForMember(dest=>dest.UpdatedAt,opt=>opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            //creatMap to  address

            CreateMap<CreateAddressDTO, Address>()
                .ReverseMap();


            //create user
            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.ProfileDTO));

            CreateMap<CreateUserProfileDTO,UserProfile>()
                .ForMember(dest=>dest.Address,opt=>opt.MapFrom(src=>src.AddressDTO))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));

        }
    }
}
