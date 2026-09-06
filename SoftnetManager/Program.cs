using JWTAuthenticationServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using softnetmanager.modules.identity.application.services;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Application.MappingProfiles;
using SoftnetManager.Modules.Identity.Application.Services;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Identity.Infrastructure.Repositories;
using SoftnetManager.Modules.Identity.Infrastructure.Services;
using SoftnetManager.Modules.Shared.Authorization;
using SoftnetManager.Modules.Shared.Database;
using SoftnetManager.Modules.Shared.Interfaces;
using SoftnetManager.Modules.Shared.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   // Preserve property names as defined in the C# models (disable camelCase naming)
                   options.JsonSerializerOptions.PropertyNamingPolicy = null;
               })
               .AddNewtonsoftJson(options =>
               {
                   // Configure Newtonsoft.Json settings if needed
                   options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
               });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));

builder.Services.AddHostedService<KeyRotationService>();

builder.Services.AddAutoMapper(cfg=>cfg.AddMaps(typeof(MyMappingProfile)));



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeyResolver = (token,securityToken,kid,parameters) =>
        {

            Console.WriteLine($"Received Token: {token}");
            Console.WriteLine($"Token Issuer: {securityToken.Issuer}");
            Console.WriteLine($"Key ID: {kid}");
            Console.WriteLine($"Validate Lifetime: {parameters.ValidateLifetime}");


            var httpClient = new HttpClient();
            var jwks = httpClient.GetStringAsync($"{builder.Configuration["Jwt:Issuer"]}/.well-known/jwks.json").Result;
            var keys = new JsonWebKeySet(jwks);
            return keys.Keys;
        }
       
    };
});

//add plicy

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider,PermissionPolicyProvider>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ITokenService,Tokenservice>();
builder.Services.AddScoped<IRefreshTokenRepository,RefreshTokenRepository>();
builder.Services.AddScoped<IFileStorageService,FileStorageService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


//add policies
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Product.Read", policy =>
//    {
//        policy.Requirements.Add(new PermissionRequirement("Product.Read"));
//    });
//    options.AddPolicy("Product.Create", policy =>
//    {
//        policy.RequireClaim("permission", "Product.Create");
//    });
//    options.AddPolicy(
//        "Product.Update",
//        policy =>
//        {
//            policy.RequireClaim(
//                "permission",
//                "Product.Update"
//            );
//        }
//    );

//    options.AddPolicy(
//        "Product.Delete",
//        policy =>
//        {
//            policy.RequireClaim(
//                "permission",
//                "Product.Delete"
//            );
//        }
//    );

//    options.AddPolicy("User.Read", policy =>
//    {
//        policy.RequireClaim("permission", "User.Read");
//    });
//    options.AddPolicy("User.Create", policy =>
//    {
//        policy.RequireClaim("permission", "User.Create");
//    });

//    options.AddPolicy(
//        "User.Update",
//        policy =>
//        {
//            policy.RequireClaim(
//                "permission",
//                "User.Update"
//            );
//        }
//    );

//    options.AddPolicy(
//        "User.Delete",
//        policy =>
//        {
//            policy.RequireClaim(
//                "permission",
//                "User.Delete"
//            );
//        }
//    );
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                    
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
