namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserID { get;}
        string? Username { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
