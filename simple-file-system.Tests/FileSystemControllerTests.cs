using System.Net;
using System.Net.Http.Json;
using simple_file_system.API.DTOs;
using simple_file_system.API.Models;

namespace simple_file_system.Tests;

public class FileSystemControllerTests : IClassFixture<FileSystemApiFactory>
{
    private readonly HttpClient _client;

    public FileSystemControllerTests(FileSystemApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static string UniqueName(string prefix) => $"{prefix}-{Guid.NewGuid():N}"[..24];

    private async Task<NodeResponseDTO> CreateDirectoryAsync(string name, long? parentId = null)
    {
        var response = await _client.PostAsJsonAsync("/api/filesystem/directory", new CreateDirectoryDTO(name, parentId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NodeResponseDTO>())!;
    }

    private async Task<NodeResponseDTO> CreateFileAsync(string name, long? parentId = null)
    {
        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(name, parentId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<NodeResponseDTO>())!;
    }

    [Fact]
    public async Task CreateFile_ReturnsCreated_WithCorrectBody()
    {
        var name = UniqueName("file");

        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(name, null));
        var body = await response.Content.ReadFromJsonAsync<NodeResponseDTO>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.True(body.Id > 0);
        Assert.Equal(name, body.Name);
        Assert.Equal(NodeType.File, body.Type);
        Assert.Null(body.ParentId);
    }

    [Fact]
    public async Task CreateFile_LocationHeaderPointsToGetNode()
    {
        var name = UniqueName("file");

        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(name, null));
        var body = await response.Content.ReadFromJsonAsync<NodeResponseDTO>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/FileSystem/{body!.Id}", response.Headers.Location!.AbsolutePath);
    }

    [Fact]
    public async Task CreateFile_EmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO("   ", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateFile_DuplicateName_ReturnsConflict()
    {
        var name = UniqueName("file");
        await CreateFileAsync(name);

        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(name, null));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateFile_SameNameAsDirectory_ReturnsConflict()
    {
        var name = UniqueName("node");
        await CreateDirectoryAsync(name);

        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(name, null));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateFile_ParentNotFound_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(UniqueName("file"), 99999));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateFile_ParentIsFile_ReturnsUnprocessableEntity()
    {
        var parent = await CreateFileAsync(UniqueName("parent"));

        var response = await _client.PostAsJsonAsync("/api/filesystem/file", new CreateFileDTO(UniqueName("child"), parent.Id));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task CreateFile_UnderDirectory_ReturnCorrectParentId()
    {
        var dir = await CreateDirectoryAsync(UniqueName("dir"));

        var file = await CreateFileAsync(UniqueName("file"), dir.Id);

        Assert.Equal(dir.Id, file.ParentId);
    }

    [Fact]
    public async Task CreateDirectory_ReturnsCreated_WithCorrectBody()
    {
        var name = UniqueName("dir");

        var response = await _client.PostAsJsonAsync("/api/filesystem/directory", new CreateDirectoryDTO(name, null));
        var body = await response.Content.ReadFromJsonAsync<NodeResponseDTO>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.True(body.Id > 0);
        Assert.Equal(name, body.Name);
        Assert.Equal(NodeType.Directory, body.Type);
    }

    [Fact]
    public async Task CreateDirectory_SameNameAsFile_ReturnsConflict()
    {
        var name = UniqueName("node");
        await CreateFileAsync(name);

        var response = await _client.PostAsJsonAsync("/api/filesystem/directory", new CreateDirectoryDTO(name, null));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
 
    [Fact]
    public async Task GetNode_ReturnsNode()
    {
        var created = await CreateFileAsync(UniqueName("file"));

        var response = await _client.GetAsync($"/api/filesystem/{created.Id}");
        var body = await response.Content.ReadFromJsonAsync<NodeResponseDTO>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(created.Id, body!.Id);
        Assert.Equal(created.Name, body.Name);
    }

    [Fact]
    public async Task GetNode_NotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/filesystem/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- DeleteNode ---

    [Fact]
    public async Task DeleteNode_ReturnsNoContent()
    {
        var node = await CreateFileAsync(UniqueName("file"));

        var response = await _client.DeleteAsync($"/api/filesystem/{node.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNode_NotFound_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/filesystem/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNode_CascadesChildren()
    {
        var dir = await CreateDirectoryAsync(UniqueName("dir"));
        var child = await CreateFileAsync(UniqueName("file"), dir.Id);

        await _client.DeleteAsync($"/api/filesystem/{dir.Id}");

        var childResponse = await _client.GetAsync($"/api/filesystem/{child.Id}");
        Assert.Equal(HttpStatusCode.NotFound, childResponse.StatusCode);
    }

    [Fact]
    public async Task SearchNodes_ReturnsMatchingPaths()
    {
        var dir = await CreateDirectoryAsync(UniqueName("searchdir"));
        await CreateFileAsync(UniqueName("match"), dir.Id);
        await CreateFileAsync(UniqueName("other"), dir.Id);

        var results = await _client.GetFromJsonAsync<List<string>>(
            $"/api/filesystem/search?query={dir.Name}&parentId=");

        Assert.NotNull(results);
        Assert.Contains(results, p => p == dir.Name);
    }

    [Fact]
    public async Task SearchNodes_ParentNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/filesystem/search?query=foo&parentId=99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchNodes_ReturnsFullPaths()
    {
        var dir = await CreateDirectoryAsync(UniqueName("pathdir"));
        var file = await CreateFileAsync(UniqueName("pathfile"), dir.Id);

        var results = await _client.GetFromJsonAsync<List<string>>(
            $"/api/filesystem/search?query={file.Name}");

        Assert.NotNull(results);
        Assert.Contains(results, p => p == $"{dir.Name}/{file.Name}");
    }

    [Fact]
    public async Task GetFileSystem_ReturnsAllPaths()
    {
        var dir = await CreateDirectoryAsync(UniqueName("fsdir"));
        var file = await CreateFileAsync(UniqueName("fsfile"), dir.Id);

        var results = await _client.GetFromJsonAsync<List<string>>("/api/filesystem");

        Assert.NotNull(results);
        Assert.Contains(results, p => p == dir.Name);
        Assert.Contains(results, p => p == $"{dir.Name}/{file.Name}");
    }
}
