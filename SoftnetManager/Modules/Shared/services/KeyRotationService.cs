
using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Entities;
using SoftnetManager.Modules.Shared.Database;
using System.Security.Cryptography;

namespace JWTAuthenticationServer.Services
{
    public class KeyRotationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _rotationInterval = TimeSpan.FromDays(7);

        public KeyRotationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RotateKeyAsync();
                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
            }
        }

        private async Task RotateKeyAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
           // var context = _serviceProvider.GetRequiredService<AppDbContext>();

            var activeKey = await context.SigningKeys.FirstOrDefaultAsync(k=>k.IsActive);

            if (activeKey == null || activeKey.ExpiresAt <= DateTime.UtcNow.AddDays(10))
            {
                if (activeKey != null)
                {
                    activeKey.IsActive = false;
                    context.SigningKeys.Update(activeKey);
                }

                using var rsa = RSA.Create(2048);

                string privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                string publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

                var newSigningKey = new SigningKey
                {
                    KeyId =  Guid.NewGuid().ToString(),
                    PrivateKey = privateKey,
                    PublicKey = publicKey,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddYears(1),
                    IsActive = true
                };
                await context.SigningKeys.AddAsync(newSigningKey);
                await context.SaveChangesAsync();
            }
        }


    }
}
