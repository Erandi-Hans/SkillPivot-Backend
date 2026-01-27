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
// This allows your React app on port 5173 to communicate with this API
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");

        // This makes Swagger the default page (at root URL)
        options.RoutePrefix = string.Empty;
    });
}

// Redirect all HTTP traffic to HTTPS for security
app.UseHttpsRedirection();

// IMPORTANT: Use CORS policy before Authorization and Mapping
// This activates the "AllowReactApp" policy we defined above
app.UseCors("AllowReactApp");

// Handle User Authorization
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Launch the application
app.Run();