using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION ---

/**
 * Configure Controllers and JSON options.
 * Setting PropertyNamingPolicy to null ensures that the JSON keys 
 * match the exact casing of your C# Model properties.
 */
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/**
 * Configure Entity Framework Core with SQL Server.
 */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/**
 * CORS Configuration to allow requests from your Vite/React app.
 */
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// --- 2. MIDDLEWARE PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

/**
 * Static Files Middleware.
 * Standard call to serve files from wwwroot.
 */
app.UseStaticFiles();

/**
 * Custom Static File Configuration for Uploads.
 * This ensures that even if the default wwwroot mapping has issues,
 * the /uploads route will specifically look into the correct folder.
 */
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();