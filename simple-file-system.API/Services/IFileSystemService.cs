using simple_file_system.API.Models;

namespace simple_file_system.API.Services;

public interface IFileSystemService
{
    Task<Node> CreateFileAsync(string name, long? parentId);
    Task<Node> CreateDirectoryAsync(string name, long? parentId);
    Task<Node?> GetNodeAsync(long id);
    Task DeleteNodeAsync(long id);
    Task<IEnumerable<string>> SearchNodesAsync(string? query, long? parentId);
}
