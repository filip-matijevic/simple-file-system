using Microsoft.AspNetCore.Mvc;
using simple_file_system.API.DTOs;
using simple_file_system.API.Models;
using simple_file_system.API.Services;

namespace simple_file_system.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileSystemController : ControllerBase
    {
        private readonly IFileSystemService _fileSystemService;
        public FileSystemController(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }
        [HttpGet]
        public async Task<IActionResult> GetFileSystem()
        {
            // Implementation for getting file system
            return Ok();
        }

        [HttpPost("file")]
        public async Task<IActionResult> CreateFile([FromBody] CreateFileDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Name cannot be empty.");
            }
            Node createdFile = await _fileSystemService.CreateFileAsync(dto);
            return CreatedAtAction(nameof(GetFileSystem), new { id = createdFile.Id }, dto);
        }
        [HttpPost("directory")]
        public async Task<IActionResult> CreateDirectory([FromBody] CreateDirectoryDTO dto){
            // Implementation for creating a directory
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest("Name cannot be empty.");
                }
            Node createdDirectory = await _fileSystemService.CreateDirectoryAsync(dto);
            return CreatedAtAction(nameof(GetFileSystem), new { id = createdDirectory.Id }, dto);
        }

        [HttpDelete("/filesystem/{id}")]
        public async Task<IActionResult> DeleteNode(long id)
        {
            bool deletedNode = await _fileSystemService.DeleteNodeAsync(id);
            if (!deletedNode)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
