using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Infraestructure.Persistence.Dependencies;
using RoutinesGymService.Transversal.Mailing;
using RoutinesGymService.Transversal.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Insert(0, new RoutePrefixConvention("api"));
});

// Configuración mínima
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de Mails
MailUtils.Initialize(builder.Configuration);

// Configuración de PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

// Registrar servicios de infraestructura
builder.Services.AddInfrastructureServices();

// Configuración JWT
var jwtSettings = builder.Configuration.GetSection("JWT");
var keyString = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no está configurado");
Console.WriteLine($"JWT Key: {keyString}"); // Debug
var key = Encoding.ASCII.GetBytes(keyString);
JwtUtils.Initialize(builder.Configuration);

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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddMemoryCache();

builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS (¡Importante que esté antes de UseAuthentication/UseAuthorization!)
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


// Clase para aplicar el prefijo /api a todas las rutas
public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix;

    public RoutePrefixConvention(string prefix)
    {
        _routePrefix = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix));
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        _routePrefix,
                        selector.AttributeRouteModel);
                }
            }
        }
    }
}