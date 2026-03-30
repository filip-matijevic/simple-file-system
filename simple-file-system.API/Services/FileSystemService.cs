using simple_file_system.API.Data;
using simple_file_system.API.DTOs;
using simple_file_system.API.Models;

namespace simple_file_system.API.Services;

public class FileSystemService : IFileSystemService
{
    private readonly FileSystemDbContext _context;
    public FileSystemService(FileSystemDbContext context)
    {
        _context = context;
    }
    public async Task<Node> CreateDirectoryAsync(CreateDirectoryDTO directoryDto)
    {
        if(directoryDto.ParentId.HasValue)
        {
            var parent = await _context.Nodes.FindAsync(directoryDto.ParentId.Value);
            if (parent is null)
            {
                throw new Exception("Parent directory not found.");
            }

            if (parent.Type != NodeType.Directory)
            {
                throw new Exception("Parent node must be a directory.");
            }
        }

        Node newNode = new Node
        {
            Name = directoryDto.Name,
            Type = NodeType.Directory,
            ParentId = directoryDto.ParentId
        };

        _context.Nodes.Add(newNode);
        await _context.SaveChangesAsync();
        return newNode;

    }

    public async Task<Node> CreateFileAsync(CreateFileDTO fileDto)
    {
        if (fileDto.ParentId.HasValue)
        {
            var parent = await _context.Nodes.FindAsync(fileDto.ParentId.Value);
            if (parent is null)
            {
                throw new Exception("Parent directory not found.");
            }

            if (parent.Type != NodeType.Directory)
            {
                throw new Exception("Parent node must be a directory.");
            }
        }

        Node newNode = new Node
        {
            Name = fileDto.Name,
            Type = NodeType.File,
            ParentId = fileDto.ParentId
        };

        _context.Nodes.Add(newNode);
        await _context.SaveChangesAsync();
        return newNode;
    }

    public async Task<bool> DeleteNodeAsync(long id)
    {
        Node? deleteNode = await _context.Nodes.FindAsync(id);
        if (deleteNode is null)
            return false;

        _context.Nodes.Remove(deleteNode);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Node?> GetNodeAsync(long id)
    {
        return await _context.Nodes.FindAsync(id);
    }
}
