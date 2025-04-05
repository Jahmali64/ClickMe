using System.Text;
using ClickMe.Application;
using ClickMe.Infrastructure;
using ClickMe.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try {
    Log.Information("Starting Application");
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    JwtSettings jwtSettings = new();
    builder.Configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped(typeof(CancellationToken),
        implementationFactory: serviceProvider => {
            IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
        });
    builder.Services.AddCors(options => {
        options.AddPolicy("Development", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        options.AddPolicy("Production",
            policy => {
                string[] allowedOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["*"];
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            });
    });
    builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true
        };
    });

    WebApplication app = builder.Build();
    if (app.Environment.IsDevelopment()) {
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseCors("Development");
    } else {
        app.UseCors("Production");
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapGet("/", () => Results.Ok("Hello World!"));
    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Unhandled exception");
} finally {
    Log.CloseAndFlush();
}