using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PPM;
using PPM.Interfaces;
using PPM.Models.Interfaces;
using PPM.Models.Repositories;
using PPM.Models.Services;
using PPM.Repositories;
using PPM.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

/*
// Load Kestrel configuration conditionally
builder.WebHost.ConfigureKestrel(options =>
{
    var env = builder.Environment;

    if (env.IsProduction() && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = new X509Certificate2(
                "/etc/letsencrypt/live/ppmparks.xyz/fullchain.pem",
                "/etc/letsencrypt/live/ppmparks.xyz/privkey.pem"
            );
        });
    }
});
*/


// JWT configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


// Add services to the container.

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT Key is missing from configuration. Please ensure 'Jwt:Key' is defined in appsettings.json.");
}

Console.WriteLine($"JWT Secret Key Loaded: {(secretKey != null ? "[REDACTED]" : "NULL")}");

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PPMDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PPMDatabase"))
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IParkRepository, ParkRepository>();
builder.Services.AddScoped<ParkService>();

builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<RegistrationService>();

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<EventService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
        }
        else
        {
            // For production, allow no origins by default (most restrictive) to avoid accidental exposure
            // This means no cross-origin calls are allowed unless you update this later
            // It avoids accidentally opening your API up
            // In production, allow either:
            // a) Your real frontend URL (if frontend hosted separately)
            // b) Or allow no CORS if frontend served by backend (same origin)
            // Uncomment and change below when ready to allow production frontend URL:
            /*
            policy.WithOrigins("https://ppmparks.xyz")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            */

            // Minimal fallback to avoid crash: disallow all cross-origin requests explicitly
            policy.SetIsOriginAllowed(_ => false)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .DisallowCredentials();
            /*
            // For now, let's allow all origins — but **be cautious with this**
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
            */
        }
    });
});

// JWT Authentication & Authorization
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
            //token expiration exact (default allows 5 minutes of leeway).
            //A token that's expired up to 5 minutes ago is still accepted as it accounts for small time sync
            //differences between systems (server vs client). TimeSpan.Zero on the other hand makes it so token 
            //expiration is strict and expires on time
            ClockSkew = TimeSpan.FromMinutes(5) 
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token as: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

//Load Kestrel settings from config (for production TLS certs)
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//Serve React static frontend
app.UseDefaultFiles(); // Looks for index.html by default
app.UseStaticFiles();  // Serves from wwwroot/

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToFile("index.html");
app.MapControllers();

app.Run();
