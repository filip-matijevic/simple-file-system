using Microsoft.EntityFrameworkCore;
using simple_file_system.API.Data;
using simple_file_system.API.Models;

namespace simple_file_system.API.Services;

public class FileSystemService : IFileSystemService
{
    private readonly FileSystemDbContext _context;
    public FileSystemService(FileSystemDbContext context)
    {
        _context = context;
    }
    public async Task<Node> CreateDirectoryAsync(string name, long? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.ValidationException("Name cannot be empty.");

        await ValidateParentAsync(parentId);

        if (await NodeExistsAsync(name, parentId))
            throw new Exceptions.ConflictException($"A node named '{name}' already exists in this location.");

        Node newNode = new() { Name = name, Type = NodeType.Directory, ParentId = parentId };
        _context.Nodes.Add(newNode);
        await _context.SaveChangesAsync();
        return newNode;
    }

    public async Task<Node> CreateFileAsync(string name, long? parentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exceptions.ValidationException("Name cannot be empty.");

        await ValidateParentAsync(parentId);

        if (await NodeExistsAsync(name, parentId))
            throw new Exceptions.ConflictException($"A node named '{name}' already exists in this location.");

        Node newNode = new() { Name = name, Type = NodeType.File, ParentId = parentId };
        _context.Nodes.Add(newNode);
        await _context.SaveChangesAsync();
        return newNode;
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

    public async Task<IEnumerable<string>> SearchNodesAsync(string? query, long? parentId)
    {
        if (query is not null && string.IsNullOrWhiteSpace(query))
            throw new Exceptions.ValidationException("Query cannot be empty.");
            
        if (parentId.HasValue)
        {
            Node? parent = await _context.Nodes.FindAsync(parentId.Value);
            if (parent is null)
                throw new Exceptions.NotFoundException($"Parent node with id {parentId} does not exist.");
        }

        var sql = parentId.HasValue
            ? @"WITH RECURSIVE paths(id, name, path) AS (
               SELECT id, name, name FROM Nodes WHERE id = {1}
               UNION ALL
               SELECT n.id, n.name, p.path || '/' || n.name
               FROM Nodes n JOIN paths p ON n.ParentId = p.id
           )
           SELECT path FROM paths WHERE name LIKE {0} ORDER BY path"
            :
            @"WITH RECURSIVE paths(id, name, path) AS (
               SELECT id, name, name FROM Nodes WHERE ParentId IS NULL
               UNION ALL
               SELECT n.id, n.name, p.path || '/' || n.name
               FROM Nodes n JOIN paths p ON n.ParentId = p.id
           )
           SELECT path FROM paths WHERE name LIKE {0} ORDER BY path";

        return await _context.Database
                .SqlQueryRaw<string>(sql, query + "%", parentId ?? (object)DBNull.Value)
                .ToListAsync();
    }

    private async Task<bool> NodeExistsAsync(string name, long? parentId)
    {
        return await _context.Nodes.AnyAsync(n => n.Name == name && n.ParentId == parentId);
    }

    private async Task ValidateParentAsync(long? parentId)
    {
        if (parentId is null)
            return;

        var parent = await _context.Nodes.FindAsync(parentId.Value);
        if (parent is null)
            throw new Exceptions.NotFoundException($"Parent node with id {parentId} does not exist.");

        if (parent.Type != NodeType.Directory)
            throw new Exceptions.InvalidOperationException($"Parent node with id {parentId} is not a directory.");
    }
}
