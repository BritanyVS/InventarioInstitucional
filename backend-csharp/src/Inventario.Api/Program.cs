using System.Text.Json.Serialization;
using Inventario.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
// Serializar los enums como texto ("Disponible", "Entrada") para que el frontend
// pinte las etiquetas y use el valor como clase CSS.
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Inventario Institucional API (C# / .NET 8)", Version = "v1" });
});

// CORS abierto para permitir el frontend de demostración.
builder.Services.AddCors(opciones =>
    opciones.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// Registro de la capa de infraestructura (repositorios, servicios, SQL Server).
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventario API v1"));

app.UseCors();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    sistema = "Sistema de Gestión de Inventario Institucional",
    backend = "C# / .NET 8",
    version = "1.0.0",
    swagger = "/swagger"
}));

app.Run();
