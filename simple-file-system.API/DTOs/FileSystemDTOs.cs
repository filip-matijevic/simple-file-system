using simple_file_system.API.Models;

namespace simple_file_system.API.DTOs;

public record CreateFileDTO(string Name, long? ParentId);
public record CreateDirectoryDTO(string Name, long? ParentId);
public record NodeResponseDTO(long Id, string Name, NodeType Type, long? ParentId);
