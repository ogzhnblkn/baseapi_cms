using BaseApi.Application.Features.Auth.Commands.Login;
using BaseApi.Application.Interfaces;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Interfaces;
using BaseApi.Infrastructure.Data;
using BaseApi.Infrastructure.Extensions;
using BaseApi.Infrastructure.Filters;
using BaseApi.Infrastructure.Middleware;
using BaseApi.Infrastructure.Repositories;
using BaseApi.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Create logs directory first
var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");
try
{
    Directory.CreateDirectory(logsPath);
    Console.WriteLine($"Logs directory created: {logsPath}");
}
catch (Exception ex)
{
    Console.WriteLine($"Could not create logs directory: {ex.Message}");
}

// Configure simple Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logsPath, "app.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(Path.Combine(logsPath, "error.log"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("=== Starting Application ===");
    Log.Information("Current Directory: {CurrentDirectory}", Directory.GetCurrentDirectory());
    Log.Information("Environment: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    Log.Information("Configuring services...");

    // Add Entity Framework with SQL Server retry logic
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Log.Information("Connection String configured");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(60);
                }));

        Log.Information("Entity Framework configured");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to configure Entity Framework");
        throw;
    }

    // Add MediatR
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

    // Add FluentValidation
    builder.Services.AddFluentValidationAutoValidation();

    // Add Repository Pattern
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();
    builder.Services.AddScoped<ISliderRepository, SliderRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IPageRepository, PageRepository>();
    builder.Services.AddScoped<ISocialMediaLinkRepository, SocialMediaLinkRepository>();
    builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

    // Add Services
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IFileUploadService, FileUploadService>();

    // Add JWT Authentication
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "MySecretKeyForJwtTokenGeneration2024";
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "BaseApi",
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"] ?? "BaseApiUsers",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    // Add Global XSS Protection
    builder.Services.AddGlobalXssProtection();

    // Add controllers with global XSS filter
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalXssProtectionFilter>();
    }).AddGlobalXssFilter();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Base API", Version = "v1" });

        // Add JWT Authentication to Swagger
        c.AddSecurityDefinition("Bearer", new()
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    Log.Information("Building application...");
    var app = builder.Build();

    Log.Information("Configuring middleware pipeline...");

    // Add test endpoints first
    app.MapGet("/", () =>
    {
        Log.Information("Root endpoint accessed");
        return Results.Ok(new
        {
            message = "API is running",
            timestamp = DateTime.UtcNow,
            environment = app.Environment.EnvironmentName,
            scheme = "HTTP/HTTPS"
        });
    });

    app.MapGet("/api/test", () =>
    {
        Log.Information("API Test endpoint accessed");
        return Results.Ok(new
        {
            message = "API Test endpoint working",
            environment = app.Environment.EnvironmentName,
            timestamp = DateTime.UtcNow
        });
    });

    // Configure static files to serve uploaded files
    app.UseStaticFiles(); // Default wwwroot serving

    // Additional static files configuration for uploads
    var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
    try
    {
        Directory.CreateDirectory(uploadsPath);
        Directory.CreateDirectory(Path.Combine(uploadsPath, "sliders"));
        Directory.CreateDirectory(Path.Combine(uploadsPath, "sliders", "mobile"));
        Directory.CreateDirectory(Path.Combine(uploadsPath, "images"));
        Directory.CreateDirectory(Path.Combine(uploadsPath, "documents"));
        Log.Information("Upload directories created: {UploadsPath}", uploadsPath);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Could not create upload directories");
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadsPath),
        RequestPath = "/uploads"
    });

    // Database operations - make it non-blocking for faster startup
    _ = Task.Run(async () =>
    {
        Log.Information("Starting background database operations...");
        try
        {
            await Task.Delay(2000); // Wait 2 seconds for app to start
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var maxRetries = 3;
            var delay = TimeSpan.FromSeconds(5);

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Log.Information("Database attempt {Attempt}", i + 1);

                    // Test connection
                    var canConnect = await context.Database.CanConnectAsync();
                    Log.Information("Can connect to database: {CanConnect}", canConnect);

                    if (!canConnect)
                    {
                        throw new Exception("Cannot connect to database");
                    }

                    Log.Information("Database connection verified");

                    // Seed default admin user if not exists
                    var adminExists = await userRepository.GetByUsernameAsync("admin");
                    if (adminExists == null)
                    {
                        var defaultAdmin = new User
                        {
                            Username = "admin",
                            Email = "admin@baseapi.com",
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                            FirstName = "System",
                            LastName = "Administrator",
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        await userRepository.CreateAsync(defaultAdmin);
                        Log.Information("Default admin user created");
                    }
                    else
                    {
                        Log.Information("Admin user already exists");
                    }

                    break;
                }
                catch (Exception ex) when (i < maxRetries - 1)
                {
                    Log.Warning(ex, "Database attempt {Attempt} failed, retrying...", i + 1);
                    await Task.Delay(delay);
                }
            }

            Log.Information("Database operations completed");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database operations failed");
        }
    });

    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Base API v1");
        c.RoutePrefix = "swagger";
    });

    // Add Global XSS Protection Middleware (before authentication)
    app.UseGlobalXssProtection();

    // Add security headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

        await next();
    });

    // Log all requests
    app.Use(async (context, next) =>
    {
        Log.Information("Request: {Method} {Path} {Scheme}",
            context.Request.Method,
            context.Request.Path,
            context.Request.Scheme);
        await next();
        Log.Information("Response: {StatusCode}", context.Response.StatusCode);
    });

    // HTTPS Redirect - Only in development or when explicitly enabled
    var enableHttpsRedirect = builder.Configuration.GetValue<bool>("EnableHttpsRedirect", false);
    if (app.Environment.IsDevelopment() || enableHttpsRedirect)
    {
        Log.Information("HTTPS Redirect enabled");
        app.UseHttpsRedirection();
    }
    else
    {
        Log.Information("HTTPS Redirect disabled for production hosting");
    }

    app.UseAuthentication();
    app.UseMiddleware<TokenValidationMiddleware>();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application ready, starting...");
    Log.Information("Available test endpoints: GET / and GET /api/test");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}
finally
{
    Log.CloseAndFlush();
}