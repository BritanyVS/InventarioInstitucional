# Descripción del Proyecto, Estructura y Archivos Open Source

> Documento consolidado: qué es el sistema, cómo está organizado y **cuáles son exactamente
> los archivos que analiza Snyk Open Source**.

---

## 1. Descripción general

**Sistema de Gestión de Inventario Institucional** es una aplicación web de demostración
para presentar **Snyk Code (SAST)** y **Snyk Open Source (SCA)** sobre un mismo sistema con
**dos backends** y un **frontend**, usando tecnologías similares a las de la institución.

- **Objetivo:** mostrar, en vivo, cómo Snyk detecta vulnerabilidades en **código propio**
  y en **dependencias de terceros** (NuGet y npm).
- **Ubicación:** `C:\Users\<usuario>\OneDrive - 01ZeroGroup\Escritorio\InventarioInstitucional`
- **Aviso:** contiene **vulnerabilidades intencionales** con fines **educativos**. NO usar en producción.

---

## 2. Arquitectura (Clean Architecture)

Las 4 capas se replican en **ambos** backends. La regla de dependencias va siempre
**hacia adentro**: `Api → Infrastructure → Application → Domain`.

```
        ┌──────────────────────────────────────────────────┐
        │  Api  (Controladores HTTP, Program/Startup, DI)   │
        └───────────────────────┬──────────────────────────┘
                                │ depende de
        ┌───────────────────────▼──────────────────────────┐
        │  Infrastructure (SQL Server, repositorios, MD5)   │  ← dependencias NuGet
        └───────────────────────┬──────────────────────────┘
                                │ depende de
        ┌───────────────────────▼──────────────────────────┐
        │  Application (DTOs, interfaces de servicio, lógica)│
        └───────────────────────┬──────────────────────────┘
                                │ depende de
        ┌───────────────────────▼──────────────────────────┐
        │  Domain (entidades, enums, interfaces de repo)     │  ← núcleo puro
        └──────────────────────────────────────────────────┘
```

**Regla clave para Snyk Open Source:** las dependencias de terceros se declaran **por proyecto**,
en el proyecto que realmente las usa (normalmente `Infrastructure` y `Api`). El `.sln` solo agrupa.

---

## 3. Tecnologías

| Capa | Tecnología |
|------|-----------|
| Backend 1 | **C# / .NET 8** — ASP.NET Core Web API (`http://localhost:5080`) |
| Backend 2 | **VB.NET / .NET Framework 4.8** — ASP.NET Web API 2 + OWIN self-host (`http://localhost:5081`) |
| Frontend | **HTML, CSS, JavaScript** (vanilla + axios/lodash/jquery) — `http://localhost:5500` |
| Base de datos | **SQL Server** (MSSQL Express) — BD `InventarioInstitucional` |

---

## 4. Jerarquía de archivos

Leyenda: **`⬤ OS`** = archivo de manifiesto que **Snyk Open Source** analiza.

```
InventarioInstitucional/
├── README.md
├── .gitignore
│
├── backend-csharp/                                  ← Backend C# / .NET 8
│   ├── Inventario.sln
│   ├── Directory.Build.props
│   ├── src/
│   │   ├── Inventario.Domain/                       (núcleo: entidades, enums, interfaces)
│   │   │   ├── Entities/  (Activo.cs, Asignacion.cs, Movimiento.cs, Usuario.cs)
│   │   │   ├── Enums/     (EstadoActivo.cs, TipoMovimiento.cs)
│   │   │   ├── Interfaces/(IActivoRepository.cs, IAsignacionRepository.cs, ...)
│   │   │   └── Inventario.Domain.csproj              ⬤ OS  (sin paquetes externos)
│   │   ├── Inventario.Application/                  (DTOs, interfaces, servicios)
│   │   │   ├── DTOs/      (ActivoDtos.cs, AuthDtos.cs, MovimientoDtos.cs)
│   │   │   ├── Interfaces/(IPasswordHasher.cs, IServices.cs, ISqlConnectionFactory.cs)
│   │   │   ├── Services/  (ActivoService.cs, AuthService.cs, MovimientoService.cs, ReporteService.cs)
│   │   │   └── Inventario.Application.csproj         ⬤ OS  (sin paquetes externos)
│   │   ├── Inventario.Infrastructure/               (datos, DI, seguridad)  ← aquí están las vulns NuGet
│   │   │   ├── Data/      (SqlConnectionFactory.cs)
│   │   │   ├── Repositories/(ActivoRepository.cs, AsignacionRepository.cs, MovimientoRepository.cs, UsuarioRepository.cs)
│   │   │   ├── Security/  (Md5PasswordHasher.cs)
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Inventario.Infrastructure.csproj      ⬤ OS  (Newtonsoft.Json ⚠, log4net ⚠, ...)
│   │   └── Inventario.Api/                          (capa HTTP)
│   │       ├── Controllers/(ActivosController.cs, AsignacionesController.cs, AuthController.cs, MovimientosController.cs, ReportesController.cs)
│   │       ├── Program.cs
│   │       ├── appsettings.json
│   │       ├── Properties/launchSettings.json
│   │       └── Inventario.Api.csproj                 ⬤ OS  (Swashbuckle.AspNetCore)
│   └── tests/
│       └── Inventario.Tests/
│           ├── ActivoServiceTests.cs
│           ├── ReporteServiceTests.cs
│           ├── GlobalUsings.cs
│           └── Inventario.Tests.csproj               ⬤ OS  (xUnit, Test.Sdk)
│
├── backend-vbnet/                                   ← Backend VB.NET / .NET Framework 4.8
│   ├── InventarioVB.sln
│   ├── Directory.Build.props
│   ├── src/
│   │   ├── InventarioVB.Domain/
│   │   │   ├── Entities/  (Activo.vb, Asignacion.vb, Movimiento.vb, Usuario.vb)
│   │   │   ├── Enums/     (EstadoActivo.vb, TipoMovimiento.vb)
│   │   │   ├── Interfaces/(IRepositories.vb)
│   │   │   └── InventarioVB.Domain.vbproj            ⬤ OS  (sin paquetes externos)
│   │   ├── InventarioVB.Application/
│   │   │   ├── DTOs/      (Dtos.vb)
│   │   │   ├── Interfaces/(IInterfaces.vb)
│   │   │   ├── Services/  (Services.vb)
│   │   │   └── InventarioVB.Application.vbproj       ⬤ OS  (sin paquetes externos)
│   │   ├── InventarioVB.Infrastructure/             ← aquí están las vulns NuGet
│   │   │   ├── Data/      (SqlConnectionFactory.vb)
│   │   │   ├── Repositories/(ActivoRepository.vb, AsignacionRepository.vb, MovimientoRepository.vb, UsuarioRepository.vb)
│   │   │   ├── Security/  (Md5PasswordHasher.vb)
│   │   │   ├── Services/  (FileService.vb)           ← Path Traversal (Snyk Code)
│   │   │   └── InventarioVB.Infrastructure.vbproj    ⬤ OS  (Newtonsoft.Json ⚠, System.Text.Encodings.Web ⚠)
│   │   └── InventarioVB.Api/                        (host OWIN)
│   │       ├── Controllers/(ActivosController.vb, MovimientosController.vb, ReportesController.vb)
│   │       ├── Program.vb
│   │       ├── Startup.vb
│   │       ├── CompositionRoot.vb
│   │       ├── App.config
│   │       └── InventarioVB.Api.vbproj               ⬤ OS  (Newtonsoft.Json ⚠, WebApi.OwinSelfHost, WebApi.Cors)
│   └── tests/
│       └── InventarioVB.Tests/
│           ├── ActivoRepositoryFalso.vb
│           ├── ReporteServiceTests.vb
│           └── InventarioVB.Tests.vbproj             ⬤ OS  (MSTest, Test.Sdk)
│
├── frontend/                                        ← Frontend HTML/CSS/JS
│   ├── index.html
│   ├── activos.html
│   ├── movimientos.html
│   ├── reportes.html
│   ├── css/styles.css
│   ├── js/
│   │   ├── config.js         (selector de backend C#/VB)
│   │   ├── api.js            (cliente axios)
│   │   ├── app.js
│   │   ├── activos.js
│   │   ├── movimientos.js
│   │   ├── reportes.js
│   │   └── util.js           (DOM XSS + eval() → Snyk Code)
│   ├── package.json                                  ⬤ OS  (axios ⚠, jquery ⚠, lodash ⚠)
│   └── package-lock.json                             ⬤ OS  (árbol resuelto npm)
│
├── database/
│   ├── 01_crear_bd.sql
│   └── 02_datos_prueba.sql
│
├── docs/
│   ├── ARQUITECTURA.md
│   ├── VULNERABILIDADES.md
│   ├── GUIA_SNYK.md
│   └── DESCRIPCION_PROYECTO.md      (este documento)
│
└── scripts/
    ├── run-demo.ps1                 (compila y levanta los 3 servicios)
    └── demo-snyk.ps1
```

---

## 5. Archivos Open Source analizados por Snyk Open Source

Snyk Open Source analiza **manifiestos de dependencias**, no código fuente.
Para NuGet, Snyk lee el **`obj/project.assets.json`** resuelto, y los `.csproj` / `.vbproj`
son los archivos de proyecto que resuelven a ese `project.assets.json`.

### 5.1 NuGet — archivos `.csproj` / `.vbproj` (10 en total)

| # | Archivo de manifiesto | Dependencias declaradas | ¿Vulnerable? |
|---|---|---|---|
| 1 | `backend-csharp/src/Inventario.Api/Inventario.Api.csproj` | Swashbuckle.AspNetCore 6.5.0 | — |
| 2 | `backend-csharp/src/Inventario.Infrastructure/Inventario.Infrastructure.csproj` | Microsoft.Data.SqlClient 5.1.5 · Microsoft.Extensions.Configuration.Abstractions 8.0.0 · Microsoft.Extensions.DependencyInjection.Abstractions 8.0.0 · **Newtonsoft.Json 12.0.1** · **log4net 2.0.8** | ⚠ **2 vulns** |
| 3 | `backend-csharp/src/Inventario.Application/Inventario.Application.csproj` | (sin paquetes externos) | — |
| 4 | `backend-csharp/src/Inventario.Domain/Inventario.Domain.csproj` | (sin paquetes externos) | — |
| 5 | `backend-csharp/tests/Inventario.Tests/Inventario.Tests.csproj` | Microsoft.NET.Test.Sdk 17.11.1 · xunit 2.9.2 · xunit.runner.visualstudio 2.8.2 | — |
| 6 | `backend-vbnet/src/InventarioVB.Api/InventarioVB.Api.vbproj` | Microsoft.AspNet.WebApi.OwinSelfHost 5.2.9 · Microsoft.AspNet.WebApi.Cors 5.2.9 · **Newtonsoft.Json 12.0.1** | ⚠ **1 vuln** |
| 7 | `backend-vbnet/src/InventarioVB.Infrastructure/InventarioVB.Infrastructure.vbproj` | **Newtonsoft.Json 12.0.1** · **System.Text.Encodings.Web 4.5.0** | ⚠ **2 vulns** |
| 8 | `backend-vbnet/src/InventarioVB.Application/InventarioVB.Application.vbproj` | (sin paquetes externos) | — |
| 9 | `backend-vbnet/src/InventarioVB.Domain/InventarioVB.Domain.vbproj` | (sin paquetes externos) | — |
| 10 | `backend-vbnet/tests/InventarioVB.Tests/InventarioVB.Tests.vbproj` | Microsoft.NET.Test.Sdk 17.11.1 · MSTest.TestFramework 3.6.1 · MSTest.TestAdapter 3.6.1 | — |

> **Archivos generados (no commiteados):** `obj/project.assets.json`, uno por cada proyecto
> (10 en total). Se crean con `dotnet restore` y son los que Snyk lee realmente.
> **No existe `packages.config`** porque se usa `PackageReference` (formato moderno).

### 5.2 npm — archivos del frontend

| # | Archivo de manifiesto | Dependencias declaradas | ¿Vulnerable? |
|---|---|---|---|
| 11 | `frontend/package.json` | **axios 0.21.1** · **jquery 3.4.1** · **lodash 4.17.15** · http-server 14.1.1 (dev) | ⚠ **3 paquetes** |
| 12 | `frontend/package-lock.json` | Árbol de dependencias resuelto (versiones exactas) | ⚠ (heredado) |

---

## 6. Dependencias vulnerables intencionales (Snyk Open Source)

| Ecosistema | Paquete | Versión | CVE | Severidad | Corregido en |
|---|---|---|---|---|---|
| NuGet | `Newtonsoft.Json` | 12.0.1 | CVE-2024-21907 (DoS) | Alta | 13.0.1 |
| NuGet | `log4net` | 2.0.8 | CVE-2018-1285 (XXE) | Alta | 2.0.10 |
| NuGet | `System.Text.Encodings.Web` | 4.5.0 | CVE-2021-26701 (RCE) | **Crítica** | 4.7.2 / 5.0.1 / 6.0.0 |
| npm | `axios` | 0.21.1 | CVE-2021-3749 (ReDoS) | Alta | 0.21.2 |
| npm | `lodash` | 4.17.15 | CVE-2020-8203 · CVE-2021-23337 · CVE-2020-28500 | Alta/Alta/Media | 4.17.21 |
| npm | `jquery` | 3.4.1 | CVE-2020-11022 · CVE-2020-11023 (XSS) | Media | 3.5.0 |

---

## 7. Cómo ejecutar el análisis

```powershell
# ---------- Backend C# ----------
cd backend-csharp
dotnet restore Inventario.sln          # genera obj/project.assets.json
snyk test --all-projects               # Snyk Open Source (NuGet)
snyk code test                         # Snyk Code (archivos .cs)

# ---------- Backend VB.NET ----------
cd backend-vbnet
dotnet restore InventarioVB.sln        # genera obj/project.assets.json
snyk test --all-projects               # Snyk Open Source (NuGet) — soporta *.vbproj
snyk code test                         # Snyk Code (archivos .vb)

# ---------- Frontend ----------
cd frontend
npm install
snyk test                              # Snyk Open Source (npm)
snyk code test                         # Snyk Code (archivos .js)
```

> `dotnet restore` es **obligatorio** antes de `snyk test`: si no existe
> `obj/project.assets.json`, Snyk no ve las dependencias NuGet.

---

## 8. Snyk Open Source vs Snyk Code (no confundir)

| Producto | Qué analiza | Archivos de ESTE proyecto |
|---|---|---|
| **Snyk Open Source** (SCA) | Manifiestos de dependencias | `*.csproj`, `*.vbproj` (+ `obj/project.assets.json`), `package.json`, `package-lock.json` |
| **Snyk Code** (SAST) | Código fuente | `*.cs`, `*.vb`, `*.js` |

- **Open Source** es agnóstico al lenguaje: para NuGet lee el manifiesto; **VB.NET está soportado** vía `*.vbproj`.
- **Snyk Code** sí depende del lenguaje, y **soporta C# y VB.NET** (`.vb`) oficialmente.
