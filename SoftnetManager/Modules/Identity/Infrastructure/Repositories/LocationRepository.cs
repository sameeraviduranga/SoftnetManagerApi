using Microsoft.EntityFrameworkCore;
using SoftnetManager.Modules.Identity.Domain.Interfaces;
using SoftnetManager.Modules.Shared.Database;

namespace SoftnetManager.Modules.Identity.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext context;

        public LocationRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<bool> IsZoneExistAsync(int? id,CancellationToken cancellationToken)
        {
            return await context.Zones.AnyAsync(z => z.ID == id, cancellationToken);
        }
    }
}
