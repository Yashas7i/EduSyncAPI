using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

public class BlobService : IBlobService
{
    private readonly string _connectionString;
    private readonly string _containerName = "coursemedia";

    public BlobService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("AzureBlobStorage");
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var containerClient = new BlobContainerClient(_connectionString, _containerName);
        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString(); // 🔗 URL to store in DB
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        var containerClient = new BlobContainerClient(_connectionString, _containerName);
        var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
        var blobClient = containerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }
}
