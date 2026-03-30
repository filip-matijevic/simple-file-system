using Microsoft.EntityFrameworkCore;
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
        if (string.IsNullOrWhiteSpace(directoryDto.Name))
            throw new Exceptions.ValidationException("Name cannot be empty.");

        if (directoryDto.ParentId.HasValue)
        {
            var parent = await _context.Nodes.FindAsync(directoryDto.ParentId.Value);
            if (parent is null)
            {
                throw new Exceptions.NotFoundException($"Parent node with id {directoryDto.ParentId} does not exist.");
            }

            if (parent.Type != NodeType.Directory)
            {
                throw new Exceptions.InvalidOperationException($"Parent node with id {directoryDto.ParentId} is not a directory.");
            }
        }

        if (await NodeExistsAsync(directoryDto.Name, directoryDto.ParentId, NodeType.Directory))
        {
            throw new Exceptions.ConflictException($"A directory named '{directoryDto.Name}' already exists in this location.");
        }

        try
        {
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
        catch (DbUpdateException)
        {
            throw new Exceptions.ConflictException($"A directory named '{directoryDto.Name}' already exists in this location.");
        }
    }

    public async Task<Node> CreateFileAsync(CreateFileDTO fileDto)
    {
        if (string.IsNullOrWhiteSpace(fileDto.Name))
            throw new Exceptions.ValidationException("Name cannot be empty.");

        if (fileDto.ParentId.HasValue)
        {
            var parent = await _context.Nodes.FindAsync(fileDto.ParentId.Value);
            if (parent is null)
            {
                throw new Exceptions.NotFoundException($"Parent node with id {fileDto.ParentId} does not exist.");
            }

            if (parent.Type != NodeType.Directory)
            {
                throw new Exceptions.InvalidOperationException($"Node {fileDto.ParentId} is a file and cannot contain children.");
            }
        }

        if (await NodeExistsAsync(fileDto.Name, fileDto.ParentId, NodeType.File))
        {
            throw new Exceptions.ConflictException($"A file named '{fileDto.Name}' already exists in this location.");
        }

        try
        {
            Node newNode = new Node { Name = fileDto.Name, Type = NodeType.File, ParentId = fileDto.ParentId };
            _context.Nodes.Add(newNode);
            await _context.SaveChangesAsync();
            return newNode;
        }
        catch (DbUpdateException)
        {
            throw new Exceptions.ConflictException($"A file named '{fileDto.Name}' already exists in this location.");
        }
    }

    public async Task DeleteNodeAsync(long id)
    {
        Node? deleteNode = await _context.Nodes.FindAsync(id);
        if (deleteNode is null)
            throw new Exceptions.NotFoundException($"Node with id {id} does not exist.");

        _context.Nodes.Remove(deleteNode);
        await _context.SaveChangesAsync();
    }

    public async Task<Node?> GetNodeAsync(long id)
    {
        return await _context.Nodes.FindAsync(id);
    }

    private async Task<bool> NodeExistsAsync(string name, long? parentId, NodeType type)
    {
        return await _context.Nodes.AnyAsync(n => n.Name == name && n.ParentId == parentId && n.Type == type);
    }
}
