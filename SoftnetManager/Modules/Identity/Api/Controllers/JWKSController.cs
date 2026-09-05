using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SoftnetManager.Modules.Shared.Database;
using System.Security.Cryptography;

namespace SoftnetManager.Modules.Identity.Api.Controllers
{
    [Route(".well-known")]
    [ApiController]
    public class JWKSController : ControllerBase
    {
        private readonly AppDbContext context;

        public JWKSController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("jwks.json")]
        public IActionResult GetJWKS()
        {
            var keys =  context.SigningKeys.Where(k=>k.IsActive).ToList();

            var jwks = new
            {
                keys = keys.Select(k => new
                {
                    kty = "RSA",
                    use = "sig",
                    kid = k.KeyId,
                    n = Base64UrlEncoder.Encode(GetModulus(k.PublicKey)),
                    e = Base64UrlEncoder.Encode(GetExponent(k.PublicKey)),
                })
            };

            return Ok(jwks);
        }

        private byte[] GetModulus(string publicKey)
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

            var parameters = rsa.ExportParameters(false);

            if (parameters.Modulus == null)
            {
                throw new InvalidOperationException("RSA parameters are not valid.");
            }

            return parameters.Modulus;
        }

        private byte[] GetExponent(string publicKey)
        {
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

            var parameters = rsa.ExportParameters(false);

            if (parameters.Exponent == null)
            {
                throw new InvalidOperationException("RSA parameters are not valid.");
            }

            return parameters.Exponent;
        }
    }
}
