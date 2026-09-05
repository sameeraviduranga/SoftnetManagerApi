using Microsoft.IdentityModel.Tokens;
using SoftnetManager.Modules.Identity.Application.Interfaces;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace softnetmanager.modules.identity.application.services
{
    public class Tokenservice:ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public Tokenservice(IUserRepository userRepository,IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }


        public string GenerateJwtToken(User user, Client client)
        {
            var signingkey = _userRepository.GetActiveSigningKey();
            if (signingkey == null)
            {
                throw new Exception("no active signing key found.");
            }

            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(signingkey.PrivateKey), out _);

                var parameters = rsa.ExportParameters(true);

                var rsasecuritykey = new RsaSecurityKey(parameters)
                {
                    KeyId = signingkey.KeyId
                };

                var creds = new SigningCredentials(rsasecuritykey, SecurityAlgorithms.RsaSha256);

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserProfile.FirstName),
                new Claim("nameidentifier", user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),


            };

                foreach (var userrole in user.UserRoles)
                {
                    claims.Add(new Claim("role", userrole.Role.Name));

                    //add permissions
                    foreach (var rolepermission in userrole.Role.RolePermissions)
                    {
                        claims.Add(new Claim("permission", rolepermission.Permission.Name));
                    }
                }

                var tokendescriptor = new JwtSecurityToken(
                    issuer: _configuration["jwt:issuer"],
                    audience: client.ClientURL,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                    );

                var tokenhandler = new JwtSecurityTokenHandler(); // create a jwt token handler to serialize the token

                var token = tokenhandler.WriteToken(tokendescriptor);// serialize the token to a string

                return token;
            }

        }

        public string GenerateRefreshToken()
        {
            var randomnumbers = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomnumbers);
            }
            return Convert.ToBase64String(randomnumbers);
        }

        public string HashToken(string refreshToken)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
