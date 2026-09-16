namespace SoftnetManager.Modules.Identity.Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<bool> IsZoneExistAsync(int? id, CancellationToken cancellationToken);
    }
}
