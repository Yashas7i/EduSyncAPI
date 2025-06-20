using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<bool> DeleteFileAsync(string fileUrl);
}
