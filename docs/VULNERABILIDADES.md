# Catálogo de vulnerabilidades intencionales

> ⚠️ **TODAS las vulnerabilidades de este documento son intencionales** y existen únicamente para
> demostrar las capacidades de **Snyk Code** (SAST) y **Snyk Open Source** (SCA). **No usar en producción.**

Cada motor (C#, VB.NET y JavaScript) contiene al menos **dos vulnerabilidades de código** y
**dependencias de terceros vulnerables**, tal como se solicitó.

---

## Resumen

| # | Motor | Herramienta Snyk | Vulnerabilidad | CWE / CVE | Archivo |
|---|-------|------------------|----------------|-----------|---------|
| 1 | C# | Snyk Code | SQL Injection | CWE-89 | `ActivoRepository.cs` |
| 2 | C# | Snyk Code | SQL Injection (bypass de login) | CWE-89 | `UsuarioRepository.cs` |
| 3 | C# | Snyk Code | Criptografía débil (MD5) | CWE-327 / CWE-916 | `Md5PasswordHasher.cs` |
| 4 | C# | Snyk Code | Credenciales en el código | CWE-798 | `SqlConnectionFactory.cs` |
| 5 | C# | Snyk Open Source | `Newtonsoft.Json` 12.0.1 | CVE-2024-21907 | `Inventario.Infrastructure.csproj` |
| 6 | C# | Snyk Open Source | `log4net` 2.0.8 | CVE-2018-1285 | `Inventario.Infrastructure.csproj` |
| 7 | VB.NET | Snyk Code | SQL Injection | CWE-89 | `ActivoRepository.vb` |
| 8 | VB.NET | Snyk Code | SQL Injection (bypass de login) | CWE-89 | `UsuarioRepository.vb` |
| 9 | VB.NET | Snyk Code | Path Traversal | CWE-22 | `FileService.vb` |
| 10 | VB.NET | Snyk Code | Criptografía débil (MD5) | CWE-327 | `Md5PasswordHasher.vb` |
| 11 | VB.NET | Snyk Code | Credenciales en el código | CWE-798 | `SqlConnectionFactory.vb` |
| 12 | VB.NET | Snyk Open Source | `Newtonsoft.Json` 12.0.1 | CVE-2024-21907 | `InventarioVB.Api.vbproj` |
| 13 | VB.NET | Snyk Open Source | `System.Text.Encodings.Web` 4.5.0 | CVE-2021-26701 | `InventarioVB.Infrastructure.vbproj` |
| 14 | JavaScript | Snyk Code | DOM XSS (`innerHTML`) | CWE-79 | `frontend/js/util.js` |
| 15 | JavaScript | Snyk Code | Code Injection (`eval`) | CWE-95 | `frontend/js/util.js` |
| 16 | JavaScript | Snyk Open Source | `axios` 0.21.1 | CVE-2021-3749 | `frontend/package.json` |
| 17 | JavaScript | Snyk Open Source | `lodash` 4.17.15 | CVE-2020-8203 | `frontend/package.json` |
| 18 | JavaScript | Snyk Open Source | `jquery` 3.4.1 | CVE-2020-11022 / 11023 | `frontend/package.json` |

---

## Detalle por motor

### 🟦 Backend C# (.NET 8)

**1 y 2 — SQL Injection (CWE-89).**
La búsqueda de activos y el login concatenan datos del usuario en la sentencia SQL.

```csharp
cmd.CommandText = "... WHERE Codigo LIKE '%" + termino + "%' ...";           // ActivoRepository
cmd.CommandText = "... WHERE NombreUsuario = '" + nombreUsuario + "' AND ClaveHash = '" + claveHash + "'"; // UsuarioRepository
```

Explotación demostrable:
- Buscador de activos → `' OR '1'='1` devuelve **todos** los registros.
- Login → usuario `admin' --` con cualquier contraseña **omite la validación**.

**3 — Criptografía débil (CWE-327 / CWE-916).** Las contraseñas se almacenan con **MD5** sin *salt*.
Snyk Code lo reporta como *Use of a broken or risky cryptographic algorithm*.

**4 — Credenciales en el código (CWE-798).** Cadena de conexión con `sa` y contraseña embebida.

**5 y 6 — Dependencias NuGet vulnerables.** `Newtonsoft.Json` 12.0.1 y `log4net` 2.0.8.

---

### 🟪 Backend VB.NET (.NET Framework 4.8)

**7 y 8 — SQL Injection (CWE-89).** Mismo patrón que C#, en VB.NET.

**9 — Path Traversal (CWE-22).** El endpoint `GET /api/reportes/archivo?nombre=...` concatena el
nombre del archivo a la carpeta base:

```vb
Dim rutaCompleta As String = _carpetaBase & "\" & nombreArchivo
```

Explotación: `/api/reportes/archivo?nombre=..\..\..\Windows\win.ini` lee un archivo del sistema.

**10 — MD5 (CWE-327).** Igual que en C#.

**11 — Credenciales en el código (CWE-798).**

**12 y 13 — Dependencias NuGet vulnerables.** `Newtonsoft.Json` 12.0.1 y
`System.Text.Encodings.Web` 4.5.0 (CVE-2021-26701, ejecución de código).

---

### 🟨 Frontend (HTML / CSS / JavaScript)

**14 — DOM XSS (CWE-79).** `frontend/js/util.js` inserta el término buscado con `innerHTML`:

```js
contenedor.innerHTML = 'Mostrando resultados para: <strong>' + termino + '</strong>';
```

Explotación: escribir `<img src=x onerror=alert('XSS')>` en el buscador de activos.

**15 — Code Injection (CWE-95).** `frontend/js/util.js` evalúa una expresión del usuario con `eval()`:

```js
return eval(expresion);
```

Explotación: en *Reportes → Filtro de valor*, escribir `alert('ejecutado')`.

**16, 17 y 18 — Dependencias npm vulnerables.** `axios` 0.21.1, `lodash` 4.17.15 y `jquery` 3.4.1.

---

## Cómo corregiría cada hallazgo (Snyk Fix)

| Vulnerabilidad | Corrección |
|----------------|------------|
| SQL Injection | Usar **consultas parametrizadas** (`@param` / `SqlParameter`). |
| MD5 | Migrar a **BCrypt / PBKDF2 / Argon2** con *salt*. |
| Credenciales en código | Variables de entorno / Secret Manager / Azure Key Vault. |
| Path Traversal | Validar con `Path.GetFullPath` y comprobar que la ruta resultante esté dentro de la carpeta base. |
| DOM XSS | Usar `textContent` o escapar el HTML. |
| `eval` | Eliminar `eval` y parsear la expresión de forma segura. |
| Dependencias | `snyk fix` o actualizar a las versiones indicadas por Snyk. |
