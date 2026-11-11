# Sistema de Gestión de Inventario Institucional

Sistema web de demostración para la presentación de **Snyk (Snyk Code + Snyk Open Source)** ante el **MAG** (Ministerio de Agricultura, Ganadería y Alimentación).

El objetivo es mostrar cómo Snyk detecta vulnerabilidades en **código propio** y en **dependencias de terceros** sobre un mismo sistema con **dos backends** y un **frontend**, usando tecnologías similares a las que utiliza la institución.

---

## 🧱 Tecnologías

| Capa | Tecnología |
|------|-----------|
| Backend 1 | **C# / .NET 8** (ASP.NET Core Web API) |
| Backend 2 | **VB.NET / .NET Framework 4.8** (ASP.NET Web API 2 + OWIN self-host) |
| Frontend | **HTML, CSS y JavaScript** (vanilla + npm) |
| Base de datos | **SQL Server** (MSSQL Express) |

Arquitectura: **Clean Architecture** (Domain → Application → Infrastructure → Api) replicada en ambos backends.

---

## 📁 Estructura del repositorio

```
InventarioInstitucional/
├── database/                 # Scripts SQL Server (esquema + datos de prueba)
├── docs/                     # Arquitectura, vulnerabilidades y guía de Snyk
├── frontend/                 # HTML, CSS, JavaScript + package.json (npm)
├── backend-csharp/           # Backend 1: C# .NET 8 (Clean Architecture)
│   ├── src/
│   │   ├── Inventario.Domain/
│   │   ├── Inventario.Application/
│   │   ├── Inventario.Infrastructure/
│   │   └── Inventario.Api/
│   └── tests/Inventario.Tests/
└── backend-vbnet/            # Backend 2: VB.NET .NET Framework 4.8 (Clean Architecture)
    ├── src/
    │   ├── InventarioVB.Domain/
    │   ├── InventarioVB.Application/
    │   ├── InventarioVB.Infrastructure/
    │   └── InventarioVB.Api/
    └── tests/InventarioVB.Tests/
```

---

## 🚀 Puesta en marcha rápida

### Opción A — un solo comando

```powershell
pwsh scripts/run-demo.ps1
```

Compila ambos backends y levanta frontend + APIs.

### Opción B — paso a paso

#### 1. Base de datos

Ejecutar los scripts en SQL Server (SSMS o `sqlcmd`):

```powershell
sqlcmd -S .\SQLEXPRESS -E -C -i database\01_crear_bd.sql
sqlcmd -S .\SQLEXPRESS -E -C -i database\02_datos_prueba.sql
```

#### 2. Backend C# (.NET 8)

```powershell
cd backend-csharp
dotnet build
dotnet run --project src\Inventario.Api
# API: http://localhost:5080/swagger
```

#### 3. Backend VB.NET (.NET Framework 4.8)

```powershell
cd backend-vbnet
dotnet build
dotnet run --project src\InventarioVB.Api
# API: http://localhost:5081/api/activos
```

> El backend VB.NET usa `Microsoft.NETFramework.ReferenceAssemblies` para poder compilar
> con `dotnet build` sin necesidad del Developer Pack de .NET Framework.
> Si aparece "Acceso denegado" al iniciar (HttpListener), ejecute una vez como administrador:
> `netsh http add urlacl url=http://localhost:5081/ user=Everyone`

#### 4. Frontend

```powershell
cd frontend
npm install
npm start
# Abrir http://localhost:5500
```

El frontend puede apuntar a cualquiera de los dos backends con el selector del encabezado
(o editando `js/config.js`).

### Pruebas unitarias

```powershell
dotnet test backend-csharp\Inventario.sln   # xUnit
dotnet test backend-vbnet\InventarioVB.sln  # MSTest
```

---

## ⚠️ IMPORTANTE: vulnerabilidades intencionales

Este repositorio contiene **vulnerabilidades intencionales** con fines **exclusivamente educativos**
para demostrar Snyk. **NO usar en producción.** Ver `docs/VULNERABILIDADES.md` y `docs/GUIA_SNYK.md`.

---

## 📚 Documentación

- [`docs/ARQUITECTURA.md`](docs/ARQUITECTURA.md) — diseño de capas y endpoints.
- [`docs/VULNERABILIDADES.md`](docs/VULNERABILIDADES.md) — catálogo de vulnerabilidades por motor.
- [`docs/GUIA_SNYK.md`](docs/GUIA_SNYK.md) — guion de demostración paso a paso.
