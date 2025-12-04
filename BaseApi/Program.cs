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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework with SQL Server retry logic
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

// Add Repository Pattern
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<ISocialMediaLinkRepository, SocialMediaLinkRepository>();
builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

// Add JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Add File Upload Service
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

var app = builder.Build();

// Configure static files to serve uploaded files
app.UseStaticFiles(); // Default wwwroot serving

// Additional static files configuration for uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

// Auto migrate database and seed default data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    // Ensure uploads directory exists
    var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
    Directory.CreateDirectory(uploadsPath);
    Directory.CreateDirectory(Path.Combine(uploadsPath, "sliders"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "sliders", "mobile"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "images"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "documents"));

    var maxRetries = 5;
    var delay = TimeSpan.FromSeconds(5);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            context.Database.EnsureCreated();

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
                app.Logger.LogInformation("Default admin user created successfully.");
                app.Logger.LogInformation("Username: admin, Password: Admin123!");
            }

            break;
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            app.Logger.LogWarning($"Database creation attempt {i + 1} failed: {ex.Message}. Retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
