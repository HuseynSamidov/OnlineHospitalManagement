using Microsoft.AspNetCore.Http;

namespace Application.Abstracts.Services;

public interface IFileUploadServices
{
    Task<string> UploadAsync(IFormFile file);
}
