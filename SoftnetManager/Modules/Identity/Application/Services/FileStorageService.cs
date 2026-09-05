using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SoftnetManager.Modules.Identity.Application.Interfaces;

namespace SoftnetManager.Modules.Identity.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public FileStorageService(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
        public async Task<string> UploadProfileImageAsync(IFormFile file)
        {
            string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "uploads", "profileImages");
            string uniqueFilename = Guid.NewGuid().ToString() + "_" + file.FileName;
            string uploadFilePath = Path.Combine(uploadFolder, uniqueFilename);

            using (var stream = new FileStream(uploadFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return uniqueFilename;

        }
    }
}
