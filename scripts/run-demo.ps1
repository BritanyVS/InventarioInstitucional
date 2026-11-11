# ============================================================
# run-demo.ps1
# Compila y levanta el sistema completo para la demostración:
#   - Backend C# (.NET 8)        -> http://localhost:5080
#   - Backend VB.NET (.NET 4.8)  -> http://localhost:5081
#   - Frontend (http-server)     -> http://localhost:5500
#
# Uso:  pwsh scripts/run-demo.ps1
# ============================================================
$ErrorActionPreference = "Stop"
$raiz = Split-Path -Parent $PSScriptRoot

Write-Host "== Compilando backends ==" -ForegroundColor Cyan
dotnet build "$raiz\backend-csharp\Inventario.sln" -v q
dotnet build "$raiz\backend-vbnet\InventarioVB.sln" -v q

Write-Host "== Iniciando backend C# (.NET 8) en :5080 ==" -ForegroundColor Cyan
$binC = "$raiz\backend-csharp\src\Inventario.Api\bin\Debug\net8.0"
$env:ASPNETCORE_URLS = "http://localhost:5080"
Start-Process -FilePath "dotnet" -ArgumentList "`"$binC\Inventario.Api.dll`"" -WorkingDirectory $binC

Write-Host "== Iniciando backend VB.NET (.NET 4.8) en :5081 ==" -ForegroundColor Cyan
$binV = "$raiz\backend-vbnet\src\InventarioVB.Api\bin\Debug\net48"
Start-Process -FilePath "$binV\InventarioVB.Api.exe" -WorkingDirectory $binV

Write-Host "== Iniciando frontend en :5500 ==" -ForegroundColor Cyan
$frontend = Join-Path $raiz "frontend"
if (-not (Test-Path (Join-Path $frontend "node_modules"))) {
    Push-Location $frontend
    npm install
    Pop-Location
}
Start-Process -FilePath "powershell" -WorkingDirectory $frontend `
    -ArgumentList "-NoExit", "-Command", "node_modules\.bin\http-server.cmd -p 5500 -c-1"

Start-Sleep -Seconds 6
Write-Host ""
Write-Host "Sistema en ejecución:" -ForegroundColor Green
Write-Host "  Frontend      : http://localhost:5500"
Write-Host "  API C#        : http://localhost:5080/swagger"
Write-Host "  API VB.NET    : http://localhost:5081/api/activos"
Write-Host ""
Write-Host "Si el backend VB.NET muestra 'Acceso denegado', ejecute una vez como administrador:" -ForegroundColor Yellow
Write-Host "  netsh http add urlacl url=http://localhost:5081/ user=Everyone"
