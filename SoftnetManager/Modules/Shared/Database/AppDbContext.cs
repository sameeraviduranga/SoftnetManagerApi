using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Enums;

namespace SoftnetManager.Modules.Shared.Database
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne()
                .HasForeignKey<User>(u => u.UserProfileID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.Nic)
                .IsUnique()
                .HasFilter("[Nic] IS NOT NULL");

            modelBuilder.Entity<UserProfile>()
                .HasOne(u=>u.Branch)
                .WithMany(b=>b.users)
                .HasForeignKey(u=>u.BranchID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(u => u.Address)
                .WithMany()
                .HasForeignKey(u => u.AddressID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(u => u.Salutation)
                .WithMany()
                .HasForeignKey(u => u.SalutationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(u => u.Gender)
                .WithMany()
                .HasForeignKey(u => u.GenderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(u => u.MaritialStatus)
                .WithMany()
                .HasForeignKey(u => u.MaritialStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProfile>()
                .HasOne(u => u.Designation)
                .WithMany()
                .HasForeignKey(u => u.DesignationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => new {rt.ClientId,rt.UserId});

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserID, ur.RoleID });

            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.ClientId)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Admin Role" },
                new Role { Id = 2, Name = "Editor", Description = " Editor Role" },
                new Role { Id = 3, Name = "User", Description = "User Role" }
            );

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Client)
                .WithMany(c => c.RefreshTokens)
                .HasForeignKey(rt => rt.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            //add permissons
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new
                {
                    rp.RoleId,
                    rp.PermissionId
                });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp=>rp.Permission)
                .WithMany(p=>p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    Id = 1,
                    ClientId = "Client1",
                    Name = "Client Application 1",
                    ClientURL = "https://client1.com"
                },
                new Client
                {
                    Id = 2,
                    ClientId = "Client2",
                    Name = "Client Application 2",
                    ClientURL = "https://client2.com"
                }
            );

            modelBuilder.Entity<Permission>().HasData(

                new Permission
                {
                    Id = 1,
                    Name = "Product.Read",
                    Description = "View products"
                },

                new Permission
                {
                    Id = 2,
                    Name = "Product.Create",
                    Description = "Create products"
                },

                new Permission
                {
                    Id = 3,
                    Name = "Product.Update",
                    Description = "Update products"
                },

                new Permission
                {
                    Id = 4,
                    Name = "Product.Delete",
                    Description = "Delete products"
                },

                new Permission
                {
                    Id = 5,
                    Name = "User.Read",
                    Description = "View users"
                },

                new Permission
                {
                    Id = 6,
                    Name = "User.Update",
                    Description = "Update users"
                },
                new Permission
                {
                    Id = 7,
                    Name = "User.Delete",
                    Description = "Delete users"
                }
            );

            modelBuilder.Entity<City>()
                .HasOne(c=>c.Province)
                .WithMany(p=>p.Cities)
                .HasForeignKey(c=>c.ProvinceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zone>()
                .HasOne(z => z.City)
                .WithMany(c => c.Zones)
                .HasForeignKey(z => z.CityID)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<Gender>().HasData(
                new Gender {ID=1,GenderName="Male"},
                new Gender { ID = 2, GenderName ="FeMale"},
                new Gender { ID = 3, GenderName ="Other"}
             );

            modelBuilder.Entity<Salutation>().HasData(
                new Salutation { ID = 1, SalutationName = "Mr." },
                new Salutation { ID = 2, SalutationName = "Mrs." },
                new Salutation { ID = 3, SalutationName = "Ms." },
                new Salutation { ID = 4, SalutationName = "Doc." },
                new Salutation { ID = 5, SalutationName = "Ven." },
                new Salutation { ID = 6, SalutationName = "Prof." }
             );

            // 1. Seed Provinces
            modelBuilder.Entity<Province>().HasData(
                new Province { ID = 1, ProvinceName = "Western" },
                new Province { ID = 2, ProvinceName = "Central" },
                new Province { ID = 3, ProvinceName = "Southern" }
            );

            // 2. Seed Cities
            modelBuilder.Entity<City>().HasData(
                // Western Province Cities (ProvinceID = 1)
                new City { Id = 1, ProvinceID = 1, CityName = "Colombo" },
                new City { Id = 2, ProvinceID = 1, CityName = "Gampaha" },
                new City { Id = 3, ProvinceID = 1, CityName = "Kalutara" },

                // Central Province Cities (ProvinceID = 2)
                new City { Id = 4, ProvinceID = 2, CityName = "Kandy" },
                new City { Id = 5, ProvinceID = 2, CityName = "Nuwara Eliya" },

                // Southern Province Cities (ProvinceID = 3)
                new City { Id = 6, ProvinceID = 3, CityName = "Galle" },
                new City { Id = 7, ProvinceID = 3, CityName = "Matara" }
            );

            // 3. Seed Zones
            modelBuilder.Entity<Zone>().HasData(
                // Colombo Zones (CityID = 1)
                new Zone { ID = 1, CityID = 1, ZoneName = "Colombo 01 (Fort)" },
                new Zone { ID = 2, CityID = 1, ZoneName = "Colombo 03 (Kollupitiya)" },
                new Zone { ID = 3, CityID = 1, ZoneName = "Colombo 07 (Cinnamon Gardens)" },
                

                // Gampaha Zones (CityID = 2)
                new Zone { ID = 4, CityID = 2, ZoneName = "Ja-Ela" },
                new Zone { ID = 5, CityID = 2, ZoneName = "Negombo" },

                // Kandy Zones (CityID = 4)
                new Zone { ID = 6, CityID = 4, ZoneName = "Peradeniya" },
                new Zone { ID = 7, CityID = 4, ZoneName = "Katugastota" },

                // Galle Zones (CityID = 6)
                new Zone { ID = 8, CityID = 6, ZoneName = "Galle Fort" },
                new Zone { ID = 9, CityID = 6, ZoneName = "Karapitiya" },

                new Zone { ID = 10, CityID = 1, ZoneName = "Sitawaka" }
            );

            modelBuilder.Entity<Address>().HasData(

               new Address { ID = 1,ZoneID=10, Line1 = "126 GANEGODA",Line2="ARUKWATTA PADUKKA",LocationStatus = LocationStatus.Branch }
               );

            modelBuilder.Entity<Branch>().HasData(
                
                new Branch { ID = 1,AddressID = 1,Name = "Padukka Branch"}
             );

            modelBuilder.Entity<Designation>().HasData(

               new Designation { ID = 1,DesignationName="Feild Development Officer",ShortName = "FDO" },
               new Designation { ID = 2,DesignationName="Customer Relationship Officer",ShortName = "CRO" },
               new Designation { ID = 3, DesignationName = "Branch Manager", ShortName = "BM" }
            );

            modelBuilder.Entity<MaritialStatus>().HasData(

               new MaritialStatus { ID = 1,Status="Married" },
               new MaritialStatus { ID = 2,Status="UnMarried" },
               new MaritialStatus { ID = 3,Status="Widow" }
               
            );


        }

        

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<SigningKey> SigningKeys { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<MaritialStatus> MaritialStatuses { get; set; }
        public DbSet<Salutation> Salutations { get; set; }
        public DbSet<Address> Address { get; set; }

    }
}
