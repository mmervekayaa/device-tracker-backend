using System.Reflection;
using System.Text.Json.Serialization;
using DeviceTracker.Api.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Dynamic PORT for Render deployment
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Thread-safe in-memory store (uygulama ömrü boyunca tek örnek)
builder.Services.AddSingleton<IDeviceStore>(_ => new InMemoryDeviceStore());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Device Tracker API",
        Version = "v1",
        Description = "Cihaz / sensör kayıtları için in-memory CRUD API"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendPolicy", policy =>
    {
        policy.WithOrigins(
                  "https://mmervekayaa.github.io",
                  "http://localhost:5173"
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger canlı ortamda da açık (staj projesi / demo amaçlı)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Device Tracker API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontendPolicy");

app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();

// Integration testlerinde erişim için
public partial class Program { }
