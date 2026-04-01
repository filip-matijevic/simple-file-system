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
        private readonly ILogger<FileSystemController> _logger;
        public FileSystemController(IFileSystemService fileSystemService, ILogger<FileSystemController> logger)
        {
            _fileSystemService = fileSystemService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetFileSystem()
        {
            var paths = await _fileSystemService.SearchNodesAsync(null, null);
            return Ok(paths);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNode(long id)
        {
            var node = await _fileSystemService.GetNodeAsync(id);
            if (node is null)
                return NotFound();
            return Ok(new NodeResponseDTO(node.Id, node.Name, node.Type, node.ParentId));
        }

        [HttpPost("file")]
        public async Task<IActionResult> CreateFile([FromBody] CreateFileDTO dto)
        {
            _logger.LogInformation("Creating file with name '{Name}' under parent ID {ParentId}", dto.Name, dto.ParentId);
            Node createdFile = await _fileSystemService.CreateFileAsync(dto.Name, dto.ParentId);
            return CreatedAtAction(nameof(GetNode), new { id = createdFile.Id }, new NodeResponseDTO(createdFile.Id, createdFile.Name, createdFile.Type, createdFile.ParentId));
        }
        [HttpPost("directory")]
        public async Task<IActionResult> CreateDirectory([FromBody] CreateDirectoryDTO dto)
        {
            _logger.LogInformation("Creating directory with name '{Name}' under parent ID {ParentId}", dto.Name, dto.ParentId);
            Node createdDirectory = await _fileSystemService.CreateDirectoryAsync(dto.Name, dto.ParentId);
            return CreatedAtAction(nameof(GetNode), new { id = createdDirectory.Id }, new NodeResponseDTO(createdDirectory.Id, createdDirectory.Name, createdDirectory.Type, createdDirectory.ParentId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(long id)
        {
            _logger.LogInformation("Deleting node with ID {Id}", id);
            await _fileSystemService.DeleteNodeAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchNodes([FromQuery] string? query, [FromQuery] long? parentId)
        {
            _logger.LogInformation("Searching nodes with query '{Query}' under parent ID {ParentId}", query, parentId);
            var results = await _fileSystemService.SearchNodesAsync(query, parentId);
            return Ok(results);
        }
    }
}
