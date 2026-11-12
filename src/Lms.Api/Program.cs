using Lms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Lms")
    ?? throw new InvalidOperationException("Connection string 'Lms' is not configured.");

builder.Services.AddDbContext<LmsDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "Lms.Api" }))
    .WithName("GetHealth")
    .WithTags("Health");

app.Run();

public partial class Program;
