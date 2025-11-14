using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using RoutinesGymService.Infraestructure.Persistence.Context;
using RoutinesGymService.Infraestructure.Persistence.Dependencies;
using RoutinesGymService.Transversal.Common.Utils;
using RoutinesGymService.Transversal.Security.Utils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ?? Inicializar JwtUtils para que los filtros puedan usarlo
JwtUtils.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Conventions.Insert(0, new RoutePrefixConvention("api"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de Mails
MailUtils.Initialize(builder.Configuration);

// Configuración de PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

// Registrar servicios de infraestructura
builder.Services.AddInfrastructureServices();

// CORS
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
builder.Services.AddAuthorization(); // Mantener por si se usan filtros o futuras autorizaciones

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAll");

// ?? Podemos dejarlo aunque no haya esquemas; solo para compatibilidad
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seeder admin
using (IServiceScope scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

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

// Convención de prefijo de rutas
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
