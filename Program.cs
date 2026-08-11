var builder = WebApplication.CreateBuilder(args);

// Dynamic PORT for Render deployment
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// Add services to the container.
builder.Services.AddControllers();

// CORS Policy allowing all origins and GitHub Pages
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy", policy =>
    {
        policy.WithOrigins("https://mmervekayaa.github.io")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
