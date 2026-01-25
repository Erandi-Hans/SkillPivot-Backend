using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION (Must be BEFORE builder.Build()) ---

// Register controllers to handle API requests
builder.Services.AddControllers();

// Add support for API Explorer and Swagger Generator for documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the database connection using SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Setup CORS policy for React Frontend (Vite)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// --- 2. BUILD THE APP ---
var app = builder.Build();

// --- 3. MIDDLEWARE PIPELINE (Must be AFTER builder.Build()) ---

// Enable Swagger UI only in Development mode
if (app.Environment.IsDevelopment())
{
    // Generate the Swagger JSON document
    app.UseSwagger();

    // Enable the Swagger UI to interact with the API
    app.UseSwaggerUI(options =>
    {
        // For .NET 9, this is the most stable way to load the endpoint
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");

        // This makes Swagger the default page (at https://localhost:7118/)
        options.RoutePrefix = string.Empty;
    });
}

// Redirect all HTTP traffic to HTTPS for security
app.UseHttpsRedirection();

// Use the CORS policy before mapping controllers
app.UseCors("AllowReactApp");

// Handle User Authorization
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Launch the application
app.Run();