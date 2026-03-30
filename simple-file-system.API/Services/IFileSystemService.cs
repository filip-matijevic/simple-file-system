using System;
using simple_file_system.API.DTOs;
using simple_file_system.API.Models;

namespace simple_file_system.API.Services;

public interface IFileSystemService
{
    Task<Node> CreateFileAsync(CreateFileDTO fileDto);
    Task<Node> CreateDirectoryAsync(CreateDirectoryDTO directoryDto);
    Task<Node?> GetNodeAsync(long id);
    Task DeleteNodeAsync(long id);
}
