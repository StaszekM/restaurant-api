using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantApi.Controllers;

[Route("file")]
[Authorize]
public class FileController : ControllerBase
{
    [HttpGet]
    public ActionResult GetFile([FromQuery] string fileName) {
        var rootPath = Directory.GetCurrentDirectory();

        var filePath = $"{rootPath}/PrivateFiles/{fileName}";

        var fileExists = System.IO.File.Exists(filePath);

        if (fileExists) {
            var fileContents = System.IO.File.ReadAllBytes(filePath);
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);
            return File(fileContents, contentType, fileName);
        }

        return NotFound();
    }
}