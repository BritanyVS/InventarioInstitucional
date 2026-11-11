using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Interfaces;

namespace Inventario.Application.Services;

public class ActivoService : IActivoService
{
    private readonly IActivoRepository _repositorio;

    public ActivoService(IActivoRepository repositorio) => _repositorio = repositorio;

    public async Task<IEnumerable<ActivoDto>> ObtenerTodosAsync()
    {
        var activos = await _repositorio.ObtenerTodosAsync();
        return activos.Select(Mapear);
    }

    public async Task<ActivoDto?> ObtenerPorIdAsync(int id)
    {
        var activo = await _repositorio.ObtenerPorIdAsync(id);
        return activo is null ? null : Mapear(activo);
    }

    public async Task<IEnumerable<ActivoDto>> BuscarAsync(string termino)
    {
        var activos = await _repositorio.BuscarAsync(termino);
        return activos.Select(Mapear);
    }

    public Task<int> CrearAsync(CrearActivoDto dto) => _repositorio.CrearAsync(new Activo
    {
        Codigo = dto.Codigo,
        Nombre = dto.Nombre,
        Categoria = dto.Categoria,
        Marca = dto.Marca,
        Modelo = dto.Modelo,
        NumeroSerie = dto.NumeroSerie,
        Ubicacion = dto.Ubicacion,
        Valor = dto.Valor
    });

    public async Task<bool> ActualizarAsync(int id, CrearActivoDto dto)
    {
        var existente = await _repositorio.ObtenerPorIdAsync(id);
        if (existente is null) return false;

        existente.Codigo = dto.Codigo;
        existente.Nombre = dto.Nombre;
        existente.Categoria = dto.Categoria;
        existente.Marca = dto.Marca;
        existente.Modelo = dto.Modelo;
        existente.NumeroSerie = dto.NumeroSerie;
        existente.Ubicacion = dto.Ubicacion;
        existente.Valor = dto.Valor;
        return await _repositorio.ActualizarAsync(existente);
    }

    public Task<bool> EliminarAsync(int id) => _repositorio.EliminarAsync(id);

    private static ActivoDto Mapear(Activo a) => new()
    {
        Id = a.Id,
        Codigo = a.Codigo,
        Nombre = a.Nombre,
        Categoria = a.Categoria,
        Marca = a.Marca,
        Modelo = a.Modelo,
        NumeroSerie = a.NumeroSerie,
        Estado = a.Estado,
        Ubicacion = a.Ubicacion,
        Valor = a.Valor,
        FechaRegistro = a.FechaRegistro
    };
}
