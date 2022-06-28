using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantApi.Controllers;

[Route("file")]
// [Authorize]
public class FileController : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 1200, VaryByQueryKeys = new[] { "fileName" })]
    public ActionResult GetFile([FromQuery] string fileName)
    {
        var rootPath = Directory.GetCurrentDirectory();

        var filePath = $"{rootPath}/PrivateFiles/{fileName}";

        var fileExists = System.IO.File.Exists(filePath);

        if (fileExists)
        {
            var fileContents = System.IO.File.ReadAllBytes(filePath);
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);
            return File(fileContents, contentType, fileName);
        }

        return NotFound();
    }

    [HttpPost]
    public ActionResult Upload([FromForm] IFormFile file)
    {
        if (file is not null && file.Length > 0)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/PrivateFiles/{file.FileName}";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok();
        }

        return BadRequest();
    }
}