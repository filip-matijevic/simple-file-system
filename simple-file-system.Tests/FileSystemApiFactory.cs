using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using simple_file_system.API.Data;

namespace simple_file_system.Tests;

public class FileSystemApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FileSystemDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<FileSystemDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FileSystemDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await base.DisposeAsync();
    }
}
