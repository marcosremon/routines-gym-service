using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Infraestructure.Persistence.Dependencies;
using RoutinesGymService.Transversal.Common.Utils;
using RoutinesGymService.Transversal.Security.SecurityUtils;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

// Registrar servicios de infraestructura
builder.Services.AddInfrastructureServices();

// Configuración JWT
IConfigurationSection jwtSettings = builder.Configuration.GetSection("JWT");
string keyString = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no está configurado");
byte[] key = Encoding.ASCII.GetBytes(keyString);
JwtUtils.Initialize(builder.Configuration);

// Configuración de Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

    // Configurar eventos para respetar respuestas personalizadas
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Si el filtro de autorización ya estableció una respuesta personalizada, respétala
            if (context.HttpContext.Items.ContainsKey("CustomAuthResponse"))
            {
                context.HandleResponse();
                return Task.CompletedTask;
            }

            // Para otros casos (token inválido, expirado, etc.), usa respuesta personalizada
            context.HandleResponse();

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsJsonAsync(new
                {
                    responseCodeJson = "UNAUTHORIZED",
                    isSuccess = false,
                    message = "Invalid or expired token"
                });
            }

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            // Logging opcional para debugging
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CacheUtils>();
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    IConfiguration configuration = services.GetRequiredService<IConfiguration>();
    ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        DatabaseSeeder seeder = new DatabaseSeeder(dbContext, configuration);
        await seeder.SeedAdminUserAsync();
        Console.WriteLine("Usuario admin creado o ya existente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creando usuario admin: {ex.Message}");
    }
}

app.Run();

public class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix;

    public RoutePrefixConvention(string prefix)
    {
        _routePrefix = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix));
    }

    public void Apply(ApplicationModel application)
    {
        foreach (ControllerModel controller in application.Controllers)
        {
            foreach (SelectorModel selector in controller.Selectors)
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