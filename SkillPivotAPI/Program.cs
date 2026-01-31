using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION ---

// Add Controllers and configure JSON options to maintain property names as they are in the Model
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This prevents the API from changing "Status" to "status" automatically
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration for SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS setup to allow the React Frontend (Vite) to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// --- 2. MIDDLEWARE PIPELINE ---

// Enable Swagger UI only in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillPivotlk API v1");
        options.RoutePrefix = string.Empty; // Set Swagger as the default landing page
    });
}

// Redirect HTTP requests to HTTPS (Optional: Keep disabled if testing on local HTTP)
// app.UseHttpsRedirection(); 

// IMPORTANT: CORS must be placed before MapControllers and after UseRouting
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();