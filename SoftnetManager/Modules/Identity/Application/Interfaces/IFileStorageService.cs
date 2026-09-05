namespace SoftnetManager.Modules.Identity.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadProfileImageAsync(IFormFile file);
    }
}
