using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using simple_file_system.API.Data;
using simple_file_system.API.Middleware;
using simple_file_system.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IFileSystemService, FileSystemService>();

builder.Services.AddDbContext<FileSystemDbContext>(options =>
    options.UseSqlite("Data Source=FileSystemDb.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();