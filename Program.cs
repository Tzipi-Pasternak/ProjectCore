using ProjectCore.Services;
using ProjectCore.Middlewares;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ProjectCore.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTokenService();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<ITokenService>((options, tokenService) =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = tokenService.GetTokenValidationParameters();
    });

builder.Services.AddAuthorization(cfg =>
    {
        cfg.AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin"));
        cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User", "Admin"));
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Jewelry", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
            new string[] {}
        }
    });
});

builder.Services.AddJewelryService();
builder.Services.AddUserService();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.DateFormatPath(pathFormat: "Logs/log-{date:format=yyyy-MM-dd}.txt")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/login.html"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuditLogMiddleware();

app.UseErrorHandlingMiddleware();

app.UseDefaultFiles();
app.UseStaticFiles();

// app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers();

app.Run();
