using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WhatsNewAPI.Repositories;
using WhatsNewAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS - Updated to support iframe embedding from internal apps
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            // Add your internal parent application domains here
            policy.WithOrigins(
                "http://localhost:5173", 
                "http://localhost:3000",
                // Add your production domains for iframe embedding
                // "https://your-parent-app.company.com",
                // "https://another-app.company.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

// Add JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"];
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IChangeRepository, ChangeRepository>();
builder.Services.AddScoped<ISqlIntegrationRepository, SqlIntegrationRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ITimeToActionRepository, TimeToActionRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ISqlIntegrationService, SqlIntegrationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

// Add security headers middleware - UPDATED FOR IFRAME EMBEDDING
app.Use(async (context, next) =>
{
    // Add security headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    
    // IFRAME EMBEDDING CONFIGURATION:
    // Option 1: Allow from same origin only (most restrictive)
    // context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    
    // Option 2: Allow from specific domains using CSP (recommended for production)
    // Update this list with your actual internal application domains
    var allowedFrameOrigins = new[]
    {
        "'self'",
        "http://localhost:5173",
        "http://localhost:3000",
        // Add your production parent app domains here:
        // "https://your-parent-app.company.com",
        // "https://another-app.company.com"
    };
    
    var frameAncestors = string.Join(" ", allowedFrameOrigins);
    context.Response.Headers.Add("Content-Security-Policy", 
        $"frame-ancestors {frameAncestors}");
    
    // Note: X-Frame-Options is deprecated in favor of CSP frame-ancestors
    // If you need X-Frame-Options for older browser support:
    // context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();