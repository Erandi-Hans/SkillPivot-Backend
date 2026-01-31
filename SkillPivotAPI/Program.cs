using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES REGISTRATION ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS setup for React (Vite)
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

// සංවර්ධන කටයුතු වලදී (Development) HTTPS redirection එක තාවකාලිකව අක්‍රීය කර බැලිය හැක 
// එවිට http://localhost:7118 ලෙස භාවිතා කළ හැක
// app.UseHttpsRedirection(); 

// Activate CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();