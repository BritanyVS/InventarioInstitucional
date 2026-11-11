# Arquitectura del sistema

El sistema implementa **Clean Architecture** (Arquitectura Limpia) en **ambos backends**, de modo que
la lógica de negocio no depende de la base de datos ni del framework web. Esto permite que el mismo
modelo de dominio se exprese en C# y en VB.NET manteniendo la estructura de carpetas equivalente.

## Capas

```
┌───────────────────────────────────────────────────────────┐
│                         Api (Web)                         │  Controllers, Program/Startup
│   Depende de: Application, Infrastructure                 │
├───────────────────────────────────────────────────────────┤
│                    Infrastructure                         │  Repositorios SQL, hashing, archivos
│   Depende de: Application, Domain                         │
├───────────────────────────────────────────────────────────┤
│                     Application                           │  Servicios, DTOs, interfaces
│   Depende de: Domain                                      │
├───────────────────────────────────────────────────────────┤
│                       Domain                              │  Entidades, enums, interfaces de repo
│   No depende de nadie                                     │
└───────────────────────────────────────────────────────────┘
```

Regla de dependencia: **las flechas apuntan siempre hacia dentro** (Domain). El dominio no conoce
SQL Server, ASP.NET, ni ningún detalle de infraestructura.

## Estructura por backend

### Backend 1 — C# / .NET 8

| Proyecto | Contenido |
|----------|-----------|
| `Inventario.Domain` | `Activo`, `Movimiento`, `Asignacion`, `Usuario`, enums e interfaces `I*Repository`. |
| `Inventario.Application` | DTOs, interfaces de servicios, `ActivoService`, `MovimientoService`, `AsignacionService`, `AuthService`, `ReporteService`. |
| `Inventario.Infrastructure` | `SqlConnectionFactory`, repositorios ADO.NET, `Md5PasswordHasher`, `DependencyInjection`. |
| `Inventario.Api` | Controladores ASP.NET Core, `Program.cs`, Swagger. |
| `Inventario.Tests` | Pruebas unitarias (xUnit) con repositorio en memoria. |

### Backend 2 — VB.NET / .NET Framework 4.8

Misma estructura de capas:

| Proyecto | Contenido |
|----------|-----------|
| `InventarioVB.Domain` | Entidades e interfaces. |
| `InventarioVB.Application` | DTOs, interfaces, servicios. |
| `InventarioVB.Infrastructure` | Repositorios ADO.NET, `Md5PasswordHasher`, `FileService`. |
| `InventarioVB.Api` | ASP.NET Web API 2 hospedado con OWIN self-host, `CompositionRoot`. |
| `InventarioVB.Tests` | Pruebas unitarias (MSTest) con repositorio en memoria. |

> El backend VB.NET usa un *Composition Root* manual (`CompositionRoot.vb`) en lugar de un contenedor
> de inyección de dependencias, para mantenerlo simple sobre .NET Framework.

## Frontend

Aplicación multipágina en HTML + CSS + JavaScript (sin framework), que consume la API por HTTP.
`js/config.js` permite alternar entre los dos backends con un selector en el encabezado.

| Archivo | Responsabilidad |
|---------|-----------------|
| `index.html` | Login y dashboard (métricas del inventario). |
| `activos.html` | Registro, búsqueda y consulta de activos. |
| `movimientos.html` | Entradas/salidas y asignación de equipos. |
| `reportes.html` | Reporte general, filtro de valor y lectura de exportaciones. |

## Modelo de datos (SQL Server)

```
Usuarios (Id, NombreUsuario, NombreCompleto, ClaveHash, Rol, Activo, FechaCreacion)
Activos  (Id, Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor, FechaRegistro)
Movimientos (Id, ActivoId → Activos, Tipo, Cantidad, Usuario, Observacion, Fecha)
Asignaciones (Id, ActivoId → Activos, UsuarioAsignado, Area, FechaAsignacion, FechaDevolucion)
```

## Contratos de API (idénticos en ambos backends)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/activos` | Lista todos los activos. |
| GET | `/api/activos/{id}` | Obtiene un activo. |
| GET | `/api/activos/buscar?termino=` | Búsqueda (**vulnerable a SQLi**). |
| POST | `/api/activos` | Crea un activo. |
| PUT | `/api/activos/{id}` | Actualiza un activo. |
| DELETE | `/api/activos/{id}` | Elimina un activo. |
| GET | `/api/movimientos` | Lista movimientos. |
| POST | `/api/movimientos` | Registra entrada/salida. |
| GET | `/api/asignaciones` | Lista asignaciones. |
| POST | `/api/asignaciones` | Asigna un equipo. |
| POST | `/api/auth/login` | Inicio de sesión (**vulnerable a SQLi**). |
| GET | `/api/reportes/inventario` | Reporte agregado. |
| GET | `/api/reportes/archivo?nombre=` | Lee un archivo (**Path Traversal**, solo VB.NET). |
