using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My APIReport", Version = "v1" });
//});

builder.Services.AddOpenApi();
//------------------------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "http://localhost:8080/realms/reports-realm";
    options.Audience = "reports-api"; // ID клиента, который вы настроили в Keycloak
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "http://localhost:8080/realms/reports-realm",
        ValidAudience = "reports-api",
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProtheticUserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();        
        policy.RequireClaim("realm_access", "prothetic_user");// Проверка роли
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"
                )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();        
    });
});

var app = builder.Build();

app.UseRouting();

app.UseCors("default");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.UseSwagger(); // Включает Swagger
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Укажите путь к документу Swagger
//    c.RoutePrefix = string.Empty; // Установите это, если хотите, чтобы Swagger UI был доступен по корневому пути
//});

app.Run();