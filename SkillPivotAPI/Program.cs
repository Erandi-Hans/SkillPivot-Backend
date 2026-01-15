using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION (Must be BEFORE builder.Build()) ---

// Add Controllers service
builder.Services.AddControllers();

// Add OpenAPI (Swagger) service
builder.Services.AddOpenApi();

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS Policy for React (Vite uses port 5173)
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

// Configure Swagger for Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

// Enable CORS (This is very important for React connection)
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

// Map the Controllers
app.MapControllers();

// Start the Application
app.Run();