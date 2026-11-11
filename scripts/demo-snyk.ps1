# ============================================================
# demo-snyk.ps1
# Ejecuta los escaneos de Snyk Code y Snyk Open Source sobre el
# Sistema de Gestión de Inventario Institucional y genera reportes.
#
# Uso:
#   pwsh scripts/demo-snyk.ps1
#   pwsh scripts/demo-snyk.ps1 -Html     (requiere snyk-to-html)
# ============================================================
param(
    [switch]$Html
)

$ErrorActionPreference = "Continue"
$raiz = Split-Path -Parent $PSScriptRoot
$salida = Join-Path $raiz "reportes-snyk"
New-Item -ItemType Directory -Force -Path $salida | Out-Null

function Titulo($texto) {
    Write-Host ""
    Write-Host ("=" * 64) -ForegroundColor DarkCyan
    Write-Host "  $texto" -ForegroundColor Cyan
    Write-Host ("=" * 64) -ForegroundColor DarkCyan
}

function Ejecutar($descripcion, $archivo, [string[]]$argumentos) {
    Titulo $descripcion
    Push-Location $raiz
    try {
        & snyk @argumentos
    } finally {
        Pop-Location
    }
}

Titulo "Verificando Snyk CLI"
if (-not (Get-Command snyk -ErrorAction SilentlyContinue)) {
    Write-Host "Snyk CLI no está instalado. Ejecute:  npm install -g snyk" -ForegroundColor Red
    exit 1
}
snyk --version

# ---------------- Snyk Code (SAST) ----------------
Ejecutar "Snyk Code - Backend C# (.NET 8)" "backend-csharp" @("code", "test", "backend-csharp", "-o", "$salida/code-csharp.json")
Ejecutar "Snyk Code - Backend VB.NET (.NET Framework 4.8)" "backend-vbnet" @("code", "test", "backend-vbnet", "-o", "$salida/code-vbnet.json")
Ejecutar "Snyk Code - Frontend JavaScript" "frontend" @("code", "test", "frontend", "-o", "$salida/code-frontend.json")

# ---------------- Snyk Open Source (SCA) ----------------
Ejecutar "Snyk Open Source - NuGet C#" "backend-csharp" @("test", "--file=backend-csharp/src/Inventario.Infrastructure/Inventario.Infrastructure.csproj", "--json-file-output=$salida/oss-nuget-csharp.json")
Ejecutar "Snyk Open Source - NuGet VB.NET" "backend-vbnet" @("test", "--file=backend-vbnet/src/InventarioVB.Infrastructure/InventarioVB.Infrastructure.vbproj", "--json-file-output=$salida/oss-nuget-vbnet.json")
Ejecutar "Snyk Open Source - npm (frontend)" "frontend" @("test", "--json-file-output=$salida/oss-npm-frontend.json")

if ($Html) {
    Titulo "Generando reportes HTML"
    if (Get-Command snyk-to-html -ErrorAction SilentlyContinue) {
        Get-ChildItem "$salida/*.json" | ForEach-Object {
            snyk-to-html -i $_.FullName -o ($_.FullName -replace '\.json$', '.html')
        }
    } else {
        Write-Host "snyk-to-html no está instalado. Ejecute: npm install -g snyk-to-html" -ForegroundColor Yellow
    }
}

Titulo "Listo"
Write-Host "Reportes en: $salida" -ForegroundColor Green
