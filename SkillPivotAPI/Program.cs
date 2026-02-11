using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION ---

/**
 * Configure Controllers and JSON options.
 * ReferenceHandler.IgnoreCycles: Prevents 'Infinite Loop' errors when entities have circular references.
 * PropertyNamingPolicy = null: Ensures JSON keys match C# property names exactly (e.g., 'JobPostId' instead of 'jobPostId').
 */
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger/OpenAPI configuration for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database Context using the SQL Server connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/**
 * CORS (Cross-Origin Resource Sharing) Configuration:
 * This policy allows the React frontend (running on localhost:5173) to send requests to this API.
 * It permits any HTTP method (GET, POST, etc.) and any header, while allowing credentials.
 */
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();

// --- 2. MIDDLEWARE PIPELINE ---

// Enable Swagger UI only in the development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger at the app's root URL
    });
}

// Security: Redirect all HTTP traffic to HTTPS
app.UseHttpsRedirection();

// Serve default static files from the wwwroot directory
app.UseStaticFiles();

/**
 * Custom Static File Configuration for Uploads:
 * Maps the physical 'wwwroot/uploads' directory to the '/uploads' URL path.
 * It also ensures the directory exists to avoid runtime errors during file access.
 */
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
if (!Directory.Exists(uploadPath))
{
    Directory.CreateDirectory(uploadPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/uploads"
});

// Configure the routing middleware
app.UseRouting();

/**
 * IMPORTANT: Middleware Execution Order
 * UseCors must be placed after UseRouting but BEFORE UseAuthorization.
 * This ensures CORS headers are processed before security checks are performed.
 */
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// Map all controller endpoints
app.MapControllers();

// Launch the application
app.Run();