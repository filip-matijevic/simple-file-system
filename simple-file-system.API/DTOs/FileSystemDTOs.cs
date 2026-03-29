using System;

namespace simple_file_system.API.DTOs;

public record CreateFileDTO(string Name, long? ParentId);
public record CreateDirectoryDTO(string Name, long? ParentId);
