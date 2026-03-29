using Microsoft.AspNetCore.Mvc;

namespace simple_file_system.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSystemController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFileSystem()
        {
            // Implementation for getting file system
            return Ok();
        }
    }
}
