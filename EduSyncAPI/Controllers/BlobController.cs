using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class BlobController : ControllerBase
{
    private readonly IBlobService _blobService;

    public BlobController(IBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] FileUploadDto dto)
    {
        var url = await _blobService.UploadFileAsync(dto.File);
        return Ok(new { FileUrl = url });
    }


    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] string fileUrl)
    {
        var result = await _blobService.DeleteFileAsync(fileUrl);
        return result ? Ok("Deleted successfully") : NotFound("File not found");
    }
}
