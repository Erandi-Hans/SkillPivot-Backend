using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION ---

/**
 * Configure Controllers and JSON options.
 * Setting PropertyNamingPolicy to null ensures that the JSON keys 
 * match the exact casing of your C# Model properties (e.g., "Status" stays "Status").
 */
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core with SQL Server using the connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/**
 * CORS (Cross-Origin Resource Sharing) Configuration.
 * This allows your React frontend (running on localhost:5173) to securely 
 * communicate with this API.
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
 * Only enabled in development mode to help test the API endpoints easily.
 */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");
        options.RoutePrefix = string.Empty; // Makes Swagger the root page (http://localhost:7118/)
    });
}

// Global Middleware
app.UseRouting();

/**
 * CORS Middleware must be placed:
 * 1. After UseRouting
 * 2. Before UseAuthorization and MapControllers
 */
app.UseCors("AllowReactApp");

app.UseAuthentication(); // Recommended to add if you handle Login/Auth
app.UseAuthorization();

// Map controller routes to the request pipeline
app.MapControllers();

app.Run();