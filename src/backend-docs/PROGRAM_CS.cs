// ============================================
// What's New API - Program.cs (Main Entry Point)
// ============================================

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using WhatsNewAPI.Middleware;
using WhatsNewAPI.Services.Interfaces;
using WhatsNewAPI.Services.Implementations;
using WhatsNewAPI.Repositories.Interfaces;
using WhatsNewAPI.Repositories.Implementations;
using WhatsNewAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURE SERILOG
// ============================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/whatsnew-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================
// ADD SERVICES TO CONTAINER
// ============================================

// Add Controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? 
            new[] { "http://localhost:3000", "http://localhost:5173" }
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// ============================================
// CONFIGURE JWT AUTHENTICATION
// ============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long. Please update appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Information("JWT Token validated for user: {User}", 
                context.Principal?.Identity?.Name ?? "Unknown");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ============================================
// REGISTER APPLICATION SERVICES
// ============================================

// Database Connection String
builder.Services.AddSingleton<string>(provider => 
    builder.Configuration.GetConnectionString("WhatsNewDB") 
    ?? throw new InvalidOperationException("Database connection string not found"));

// Helpers
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<PasswordHelper>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IChangeRepository, ChangeRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReleaseService, ReleaseService>();
builder.Services.AddScoped<IChangeService, ChangeService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// ============================================
// CONFIGURE SWAGGER/OPENAPI
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "What's New API",
        Version = "v1",
        Description = "API for managing software release notes and changelogs",
        Contact = new OpenApiContact
        {
            Name = "What's New Team",
            Email = "support@whatsnew.com"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ============================================
// ADD HTTP CONTEXT ACCESSOR
// ============================================
builder.Services.AddHttpContextAccessor();

// ============================================
// BUILD APP
// ============================================
var app = builder.Build();

// ============================================
// CONFIGURE HTTP REQUEST PIPELINE
// ============================================

// Global Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "What's New API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Enable HTTPS Redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Audit Logging Middleware (after authentication)
app.UseMiddleware<AuditLoggingMiddleware>();

// Map Controllers
app.MapControllers();

// Health Check Endpoint
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Root Endpoint
app.MapGet("/", () => Results.Ok(new
{
    message = "What's New API",
    version = "1.0.0",
    documentation = "/swagger"
}));

// ============================================
// SEED DEFAULT DATA (Development Only)
// ============================================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    try
    {
        // Seed default users if needed
        var userService = services.GetRequiredService<IUserService>();
        await SeedDefaultUsers(userService);
        
        Log.Information("Default data seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding default data");
    }
}

// ============================================
// RUN APPLICATION
// ============================================
Log.Information("Starting What's New API...");

try
{
    app.Run();
    Log.Information("What's New API stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "What's New API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// ============================================
// HELPER METHODS
// ============================================

async Task SeedDefaultUsers(IUserService userService)
{
    // Check if users already exist
    var existingUsers = await userService.GetAllUsersAsync();
    
    if (!existingUsers.Any())
    {
        Log.Information("Seeding default users...");
        
        // Note: In production, these users should be created via a secure admin panel
        // or command-line tool, not automatically seeded
        
        // Seed admin user
        // var adminUser = new CreateUserDto
        // {
        //     Username = "admin",
        //     Email = "admin@whatsnew.com",
        //     Password = "Admin@123",
        //     FirstName = "Admin",
        //     LastName = "User",
        //     Role = "admin"
        // };
        // await userService.CreateUserAsync(adminUser);
        
        // Seed viewer user
        // var viewerUser = new CreateUserDto
        // {
        //     Username = "john.viewer",
        //     Email = "john@whatsnew.com",
        //     Password = "Viewer@123",
        //     FirstName = "John",
        //     LastName = "Viewer",
        //     Role = "viewer"
        // };
        // await userService.CreateUserAsync(viewerUser);
        
        Log.Information("Default users seeded. Please change passwords immediately!");
    }
}
