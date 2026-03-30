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
            Node createdFile = await _fileSystemService.CreateFileAsync(dto);
            return CreatedAtAction(nameof(GetFileSystem), new { id = createdFile.Id }, dto);
        }
        [HttpPost("directory")]
        public async Task<IActionResult> CreateDirectory([FromBody] CreateDirectoryDTO dto)
        {
            Node createdDirectory = await _fileSystemService.CreateDirectoryAsync(dto);
            return CreatedAtAction(nameof(GetFileSystem), new { id = createdDirectory.Id }, dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(long id)
        {
            await _fileSystemService.DeleteNodeAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchNodes([FromQuery] string query, [FromQuery] long? parentId)
        {
            var results = await _fileSystemService.SearchNodesAsync(query, parentId);
            return Ok(results);
        }
    }
}
