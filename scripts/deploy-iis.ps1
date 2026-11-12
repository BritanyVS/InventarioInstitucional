
[CmdletBinding()]
param(
    # --- Servidor / credenciales de Web Deploy ---
    [string]$Servidor      = "https://SERVIDOR-IIS:8172/msdeploy.axd",
    [string]$Usuario       = "DOMINIO\usuario",
    [string]$Password      = "PASSWORD",

    # --- Sitios IIS destino ---
    [string]$SitioCSharp   = "InventarioApi",
    [string]$SitioVBNet    = "InventarioVB",
    [string]$SitioFrontend = "InventarioFrontend",

    # --- Rutas físicas en el servidor IIS ---
    [string]$RutaCSharp    = "C:\inetpub\InventarioApi",
    [string]$RutaVBNet     = "C:\inetpub\InventarioVB",
    [string]$RutaFrontend  = "C:\inetpub\InventarioFrontend",

    # --- Gate de Snyk ---
    [string]$Severidad     = "high",   # low | medium | high | critical

    # --- Interruptores ---
    [switch]$EjecutarDeploy,           # sin esto => MODO SIMULACIÓN
    [switch]$Forzar,                   # ignora el bloqueo del gate
    [switch]$OmitirSnyk,               # salta los escaneos
    [switch]$Monitor                   # ejecuta 'snyk monitor' al final
)

$ErrorActionPreference = "Continue"
$raiz = Split-Path -Parent $PSScriptRoot

# --- Estado global del gate ---
$script:Hallazgos = 0
$script:FalloEscaneo = $false

# ============================================================
# Utilidades de salida
# ============================================================
function Titulo($texto) {
    Write-Host ""
    Write-Host ("=" * 68) -ForegroundColor DarkCyan
    Write-Host "  $texto" -ForegroundColor Cyan
    Write-Host ("=" * 68) -ForegroundColor DarkCyan
}

function Paso($texto)    { Write-Host ">> $texto" -ForegroundColor White }
function Ok($texto)      { Write-Host "   [OK]    $texto" -ForegroundColor Green }
function Aviso($texto)   { Write-Host "   [AVISO] $texto" -ForegroundColor Yellow }
function Fallo($texto)   { Write-Host "   [FALLO] $texto" -ForegroundColor Red }
function Info($texto)    { Write-Host "   $texto" -ForegroundColor Gray }

# ============================================================
# Verificación de herramientas
# ============================================================
function Resolver-MsDeploy {
    $cmd = Get-Command msdeploy.exe -ErrorAction SilentlyContinue
    if ($cmd) { return $cmd.Source }
    $candidatos = @(
        "C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe",
        "C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe",
        "C:\Program Files\IIS\Microsoft Web Deploy\msdeploy.exe"
    )
    foreach ($c in $candidatos) { if (Test-Path $c) { return $c } }
    return $null
}

# ============================================================
# Ejecuta un escaneo de Snyk y actualiza el estado del gate.
#   $Tipo: "open-source" | "code"
# ============================================================
function Invoke-EscaneoSnyk {
    param(
        [string]   $Descripcion,
        [string]   $Directorio,
        [string[]] $Argumentos,
        [string]   $Tipo
    )
    Paso $Descripcion
    Push-Location (Join-Path $raiz $Directorio)
    try {
        & snyk @Argumentos | Out-Host
        $codigo = $LASTEXITCODE
    } finally {
        Pop-Location
    }

    if ($codigo -eq 0) {
        Ok "Sin hallazgos >= '$Severidad'."
    } elseif ($codigo -eq 1) {
        Fallo "Snyk encontró vulnerabilidades >= '$Severidad'."
        $script:Hallazgos++
    } else {
        Fallo "Snyk devolvió un error (código $codigo). ¿Sesión iniciada? Ejecute: snyk auth"
        $script:FalloEscaneo = $true
    }
    Write-Host ""
}

# ============================================================
# Construye los argumentos de msdeploy (Web Deploy) para un sitio.
# ============================================================
function New-MsDeployArgs {
    param([string]$Origen, [string]$Destino, [string]$Sitio)
    $destinoCompleto = "contentPath=$Destino,computerName=$Servidor" + "?site=$Sitio" +
                       ",authType=Basic,userName=$Usuario,password=$Password,includeAcls=False"
    return @(
        "-verb:sync",
        "-source:contentPath=$Origen",
        "-dest:$destinoCompleto",
        "-disableLink:AppPoolExtension",
        "-disableLink:ContentExtension",
        "-disableLink:CertificateExtension"
    )
}

function Invoke-WebDeploy {
    param([string]$Descripcion, [string[]]$MsDeployArgs, [string]$MsDeployExe)
    Paso $Descripcion
    if ($EjecutarDeploy -and $MsDeployExe) {
        & $MsDeployExe @MsDeployArgs | Out-Host
        if ($LASTEXITCODE -eq 0) {
            Ok "Desplegado correctamente."
        } else {
            Fallo "msdeploy falló (código $LASTEXITCODE)."
        }
    } else {
        Aviso "MODO SIMULACIÓN: no se ejecuta Web Deploy. Comando que se usaría:"
        Info ("msdeploy.exe " + ($MsDeployArgs -join " "))
    }
    Write-Host ""
}

# ============================================================
# 0. Encabezado y verificación de herramientas
# ============================================================
Titulo "Sistema de Inventario Institucional - Deploy a IIS con gate de Snyk"
Info "Raíz        : $raiz"
Info "Severidad   : $Severidad"
Info "Servidor    : $Servidor"
if ($EjecutarDeploy) {
    Aviso "Modo: DESPLIEGUE REAL (se ejecutará msdeploy)."
} else {
    Aviso "Modo: SIMULACIÓN (no se despliega). Use -EjecutarDeploy para desplegar de verdad)."
}

Titulo "Verificando herramientas"
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Fallo "No se encontró 'dotnet'. Instale el SDK de .NET."
    exit 1
}
Ok "dotnet  : $((Get-Command dotnet).Source)"

$snykOk = [bool](Get-Command snyk -ErrorAction SilentlyContinue)
if ($OmitirSnyk) {
    Aviso "Snyk omitido por -OmitirSnyk."
} elseif ($snykOk) {
    Ok "snyk    : $((Get-Command snyk).Source)"
} else {
    Fallo "No se encontró 'snyk'. Instale:  npm install -g snyk"
    exit 1
}

$msdeployExe = Resolver-MsDeploy
if ($msdeployExe) {
    Ok "msdeploy: $msdeployExe"
} else {
    Aviso "msdeploy.exe no encontrado. En modo simulación está bien;"
    Aviso "para desplegar de verdad instale Web Deploy (rol IIS + Web Deploy)."
}

# ============================================================
# 1. GATE - Snyk Open Source (dependencias)
# ============================================================
if (-not $OmitirSnyk) {
    Titulo "1/5  GATE: Snyk Open Source (dependencias)"

    # Restaurar dependencias -> genera obj/project.assets.json (lo que Snyk lee)
    Paso "dotnet restore (C#) - genera project.assets.json"
    Push-Location (Join-Path $raiz "backend-csharp"); dotnet restore Inventario.sln | Out-Null; Pop-Location
    Paso "dotnet restore (VB.NET) - genera project.assets.json"
    Push-Location (Join-Path $raiz "backend-vbnet");  dotnet restore InventarioVB.sln | Out-Null; Pop-Location
    Write-Host ""

    Invoke-EscaneoSnyk "Snyk Open Source - NuGet (C# / .NET 8)" "backend-csharp" `
        @("test", "--all-projects", "--severity-threshold=$Severidad") "open-source"

    Invoke-EscaneoSnyk "Snyk Open Source - NuGet (VB.NET / .NET Framework 4.8)" "backend-vbnet" `
        @("test", "--all-projects", "--severity-threshold=$Severidad") "open-source"

    # npm (frontend): asegurar node_modules
    if (-not (Test-Path (Join-Path $raiz "frontend\node_modules"))) {
        Paso "npm install (frontend)"
        Push-Location (Join-Path $raiz "frontend"); npm install | Out-Null; Pop-Location
    }
    Invoke-EscaneoSnyk "Snyk Open Source - npm (frontend)" "frontend" `
        @("test", "--severity-threshold=$Severidad") "open-source"
}

# ============================================================
# 2. GATE - Snyk Code (SAST)
# ============================================================
if (-not $OmitirSnyk) {
    Titulo "2/5  GATE: Snyk Code (SAST)"

    Invoke-EscaneoSnyk "Snyk Code - Backend C# (.NET 8)" "backend-csharp" `
        @("code", "test", "--severity-threshold=$Severidad") "code"

    Invoke-EscaneoSnyk "Snyk Code - Backend VB.NET (.NET Framework 4.8)" "backend-vbnet" `
        @("code", "test", "--severity-threshold=$Severidad") "code"

    Invoke-EscaneoSnyk "Snyk Code - Frontend JavaScript" "frontend" `
        @("code", "test", "--severity-threshold=$Severidad") "code"
}

# ============================================================
# 3. DECISIÓN DEL GATE
# ============================================================
Titulo "3/5  Decisión del gate"
if (-not $OmitirSnyk) {
    Info "Escaneos con hallazgos >= '$Severidad': $script:Hallazgos"
    if ($script:FalloEscaneo) {
        Fallo "Hubo errores al ejecutar Snyk. Revise la sesión (snyk auth)."
    }
    if ($script:Hallazgos -gt 0 -and -not $Forzar) {
        Fallo "BLOQUEADO: no se despliega código con vulnerabilidades >= '$Severidad'."
        Info "Corrija los hallazgos o use -Forzar para omitir el gate (solo demo)."
        exit 2
    }
    if ($script:Hallazgos -gt 0 -and $Forzar) {
        Aviso "Hay hallazgos, pero -Forzar permite continuar (DEMO)."
    }
    if ($script:Hallazgos -eq 0 -and -not $script:FalloEscaneo) {
        Ok "Gate superado: el despliegue puede continuar."
    }
} else {
    Aviso "Escaneos de Snyk omitidos."
}

# ============================================================
# 4. PUBLICACIÓN (build de artefactos)
# ============================================================
Titulo "4/5  Publicación de artefactos"
$pubApi    = Join-Path $raiz "publish\api"
$pubVbnet  = Join-Path $raiz "publish\vbnet"
$pubFront  = Join-Path $raiz "publish\frontend"
New-Item -ItemType Directory -Force -Path $pubApi, $pubVbnet, $pubFront | Out-Null

Paso "dotnet publish - Backend C# (.NET 8)"
dotnet publish (Join-Path $raiz "backend-csharp\src\Inventario.Api") -c Release -o $pubApi | Out-Null
Ok "Publicado en: $pubApi"

Paso "dotnet publish - Backend VB.NET (.NET Framework 4.8)"
dotnet publish (Join-Path $raiz "backend-vbnet\src\InventarioVB.Api") -c Release -o $pubVbnet | Out-Null
Ok "Publicado en: $pubVbnet"

Paso "Copiando frontend (sitio estático, sin node_modules)"
Get-ChildItem -Path (Join-Path $raiz "frontend") -Force |
    Where-Object { $_.Name -ne "node_modules" } |
    ForEach-Object { Copy-Item $_.FullName -Destination $pubFront -Recurse -Force }
Ok "Copiado en: $pubFront"
Write-Host ""

# ============================================================
# 5. WEB DEPLOY (msdeploy) hacia IIS
# ============================================================
Titulo "5/5  Web Deploy hacia IIS"

Invoke-WebDeploy "Web Deploy - API C#  -> sitio '$SitioCSharp'" `
    (New-MsDeployArgs -Origen $pubApi -Destino $RutaCSharp -Sitio $SitioCSharp) $msdeployExe

Invoke-WebDeploy "Web Deploy - API VB.NET -> sitio '$SitioVBNet'" `
    (New-MsDeployArgs -Origen $pubVbnet -Destino $RutaVBNet -Sitio $SitioVBNet) $msdeployExe

Invoke-WebDeploy "Web Deploy - Frontend -> sitio '$SitioFrontend'" `
    (New-MsDeployArgs -Origen $pubFront -Destino $RutaFrontend -Sitio $SitioFrontend) $msdeployExe

# ============================================================
# 6. (Opcional) snyk monitor -> registra la versión desplegada
# ============================================================
if ($Monitor -and -not $OmitirSnyk) {
    Titulo "Extra: snyk monitor (seguimiento continuo)"
    Push-Location (Join-Path $raiz "backend-csharp"); snyk monitor --all-projects | Out-Host; Pop-Location
    Push-Location (Join-Path $raiz "backend-vbnet");  snyk monitor --all-projects | Out-Host; Pop-Location
    Push-Location (Join-Path $raiz "frontend");       snyk monitor | Out-Host; Pop-Location
}

# ============================================================
# Resumen
# ============================================================
Titulo "Resumen"
if ($OmitirSnyk) {
    Info "Snyk           : omitido"
} else {
    Info "Hallazgos      : $script:Hallazgos escaneo(s) con vulnerabilidades >= '$Severidad'"
}
if ($EjecutarDeploy) {
    Info "Despliegue     : EJECUTADO (msdeploy)"
} else {
    Info "Despliegue     : SIMULADO (no se ejecutó msdeploy)"
}
Write-Host ""
Write-Host "Listo. Recuerde: en un flujo real, este script se ejecuta ANTES de" -ForegroundColor Green
Write-Host "publicar; si Snyk bloquea, el despliegue manual se detiene." -ForegroundColor Green
