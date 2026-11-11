# Guía de demostración de Snyk

Guion paso a paso para mostrar **Snyk Code** (análisis de código, SAST) y **Snyk Open Source**
(análisis de dependencias, SCA) sobre el sistema de inventario.

---

## 0. Requisitos

- Node.js (para el CLI de Snyk y el frontend).
- .NET SDK 8 (para el backend C#) y .NET Framework 4.8 (para el backend VB.NET).
- Cuenta gratuita en <https://snyk.io>.

## 1. Instalar y autenticar el CLI

```powershell
npm install -g snyk
snyk --version
snyk auth
```

---

## 2. Snyk Code (análisis de código fuente)

Detecta **SQL Injection, Path Traversal, MD5, credenciales en duro, XSS, eval**, etc.

### Backend C# (.NET 8)

```powershell
snyk code test backend-csharp
```

Esperado: **SQL Injection** (2), **MD5**, **credenciales en el código**.

### Backend VB.NET (.NET Framework 4.8)

```powershell
snyk code test backend-vbnet
```

Esperado: **SQL Injection** (2), **Path Traversal**, **MD5**, **credenciales en el código**.

### Frontend JavaScript

```powershell
snyk code test frontend
```

Esperado: **DOM XSS** (`innerHTML`), **Code Injection** (`eval`).

> Para un reporte con formato atractivo:
> ```powershell
> npm install -g snyk-to-html
> snyk code test backend-csharp -o snyk-code-csharp.json
> snyk-to-html -i snyk-code-csharp.json -o snyk-code-csharp.html
> start snyk-code-csharp.html
> ```

---

## 3. Snyk Open Source (dependencias)

Detecta paquetes **NuGet** y **npm** con vulnerabilidades conocidas.

### C# — NuGet

```powershell
snyk test --file=backend-csharp/src/Inventario.Infrastructure/Inventario.Infrastructure.csproj
```

Esperado: `Newtonsoft.Json@12.0.1` (CVE-2024-21907), `log4net@2.0.8` (CVE-2018-1285).

### VB.NET — NuGet

```powershell
snyk test --file=backend-vbnet/src/InventarioVB.Infrastructure/InventarioVB.Infrastructure.vbproj
snyk test --file=backend-vbnet/src/InventarioVB.Api/InventarioVB.Api.vbproj
```

Esperado: `System.Text.Encodings.Web@4.5.0` (CVE-2021-26701), `Newtonsoft.Json@12.0.1`.

### Frontend — npm

```powershell
cd frontend
npm install
snyk test
```

Esperado: `axios@0.21.1`, `lodash@4.17.15`, `jquery@3.4.1`.

### Todo el repositorio de una vez

```powershell
snyk test --all-projects
```

---

## 4. Monitoreo continuo

```powershell
snyk monitor --all-projects
```

Envía el inventario de dependencias a la plataforma Snyk para recibir alertas de nuevas
vulnerabilidades (por ejemplo, cuando aparece un CVE nuevo en una versión ya instalada).

---

## 5. Corrección guiada (Snyk Fix)

```powershell
snyk fix --all-projects
```

Snyk propone y aplica las versiones corregidas de las dependencias vulnerables.
También se puede actualizar manualmente:

```powershell
cd frontend
npm install axios@1.7.9 lodash@4.17.21 jquery@3.7.1
```

Y en NuGet, actualizar `Newtonsoft.Json` a `13.0.3`, `log4net` a `2.0.15` y
`System.Text.Encodings.Web` a `4.7.2`.

---

## 6. Guion sugerido para la presentación (MAG)

1. **Contexto** (2 min): mostrar la arquitectura con dos backends (C# y VB.NET) y un frontend.
2. **Snyk Code – C#** (3 min): `snyk code test backend-csharp` y explicar la SQL Injection.
   Luego demostrar el *exploit* real contra la API en ejecución (`' OR '1'='1`).
3. **Snyk Code – VB.NET** (3 min): repetir en el segundo backend y mostrar el **Path Traversal**.
4. **Snyk Code – JavaScript** (3 min): mostrar el **DOM XSS** en el navegador.
5. **Snyk Open Source** (4 min): `snyk test --all-projects` y explicar los CVE de NuGet y npm.
6. **Remediación** (3 min): aplicar `snyk fix`, reconstruir y volver a escanear para mostrar el "verde".
7. **Integración** (2 min): mencionar la extensión de IDE y el escaneo en CI/CD (GitHub Actions).

---

## 7. Script de apoyo

El repositorio incluye `scripts/demo-snyk.ps1`, que ejecuta todos los escaneos y genera reportes
JSON/HTML en la carpeta `reportes-snyk`:

```powershell
pwsh scripts/demo-snyk.ps1
```
