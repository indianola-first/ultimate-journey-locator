using Microsoft.EntityFrameworkCore;
using LocationFinder.API.Data;
using LocationFinder.API.Services;
using LocationFinder.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register LocationService for dependency injection
builder.Services.AddScoped<ILocationService, LocationService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Configure CORS for WordPress iframe embedding
builder.Services.AddCors(options =>
{
    options.AddPolicy("WordPressIframe", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow localhost origins
            policy.WithOrigins(
                    "http://localhost:4200", // Angular dev server
                    "http://localhost:3000", // Alternative dev port
                    "https://localhost:4200",
                    "https://localhost:3000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            // Production: Allow only specific WordPress domains
            policy.WithOrigins(
                    "https://yourwordpresssite.com",
                    "https://yourdomain.smarterasp.net"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Location Finder API",
        Version = "v1",
        Description = "API for finding locations near a zip code",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Location Finder Support",
            Email = "support@locationfinder.com"
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Location Finder API v1");
        c.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger in development
    });
}
else
{
    // Production configuration
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Location Finder API v1");
        c.RoutePrefix = "api-docs"; // Serve Swagger UI at /api-docs in production
    });
}

// Configure CORS
app.UseCors("WordPressIframe");

// Configure static files for Angular app
app.UseStaticFiles();

// Map API controllers
app.MapControllers();

// URL rewriting for Angular routing (SPA fallback)
app.MapFallbackToFile("index.html");

// Global exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Message = "An unexpected error occurred. Please try again later."
        };

        await context.Response.WriteAsJsonAsync(error);
    });
});

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.Run();
