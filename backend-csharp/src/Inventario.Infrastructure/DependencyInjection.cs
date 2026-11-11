using Inventario.Application.Interfaces;
using Inventario.Application.Services;
using Inventario.Domain.Interfaces;
using Inventario.Infrastructure.Data;
using Inventario.Infrastructure.Repositories;
using Inventario.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventario.Infrastructure;

/// <summary>Registro de dependencias de la capa de infraestructura.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuracion)
    {
        var cadena = configuracion.GetConnectionString("InventarioDb");
        services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(cadena));

        services.AddScoped<IActivoRepository, ActivoRepository>();
        services.AddScoped<IMovimientoRepository, MovimientoRepository>();
        services.AddScoped<IAsignacionRepository, AsignacionRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        services.AddSingleton<IPasswordHasher, Md5PasswordHasher>();

        services.AddScoped<IActivoService, ActivoService>();
        services.AddScoped<IMovimientoService, MovimientoService>();
        services.AddScoped<IAsignacionService, AsignacionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReporteService, ReporteService>();

        return services;
    }
}
