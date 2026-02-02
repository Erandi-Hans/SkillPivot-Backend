using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION ---

/**
 * Configure Controllers and JSON options.
 * Setting PropertyNamingPolicy to null ensures that the JSON keys 
 * match the exact casing of your C# Model properties (e.g., "CompanyName" stays "CompanyName").
 * This is crucial for matching the casing in your React frontend state.
 */
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Swagger/OpenAPI configuration for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/**
 * Configure Entity Framework Core with SQL Server.
 * Uses the connection string defined as 'DefaultConnection' in appsettings.json.
 */
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/**
 * CORS (Cross-Origin Resource Sharing) Configuration.
 * Essential for allowing your Vite/React application (http://localhost:5173) 
 * to make requests to this API (https://localhost:7118).
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

/**
 * Swagger UI Configuration.
 * Enabled in Development to provide a visual interface for testing API endpoints.
 * Setting RoutePrefix to empty makes Swagger the default landing page.
 */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");
        options.RoutePrefix = string.Empty;
    });
}

// Enable HTTPS redirection to ensure secure communication
app.UseHttpsRedirection();

/**
 * Static Files Middleware.
 * Required to serve uploaded company logos or images stored in the 'wwwroot' folder.
 */
app.UseStaticFiles();

// Routing Middleware
app.UseRouting();

/**
 * CORS Middleware implementation.
 * Must be placed after UseRouting and before UseAuthorization.
 */
app.UseCors("AllowReactApp");

/**
 * Authentication and Authorization Middleware.
 * Secures endpoints and manages user roles (e.g., Hiring Manager, Admin).
 */
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes to the request pipeline based on [Route("api/[controller]")]
app.MapControllers();

app.Run();