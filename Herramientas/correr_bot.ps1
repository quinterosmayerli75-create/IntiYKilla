# =====================================================================
# correr_bot.ps1 - Inti y Killa
# Corre el bot de pruebas en modo batch (Unity debe estar CERRADO).
# Ejemplos:
#   .\Herramientas\correr_bot.ps1                       # todo: sistema, Mundos 1 a 6, piezas y reinicio
#   .\Herramientas\correr_bot.ps1 -modos reinicio       # solo la prueba de reinicio de los 30 niveles
#   .\Herramientas\correr_bot.ps1 -modos todos          # solo Mundos 1 a 4
#   .\Herramientas\correr_bot.ps1 -modos "Mundo2_Nivel3,Mundo2_Nivel4"
# Los registros quedan en Logs/Bot; al final se muestra el resumen.
# =====================================================================
param(
    [string[]]$modos = @("", "todos", "desafio", "reinicio"),
    [int]$limiteSegundos = 900,
    [string]$unity = "C:\Program Files\Unity\Hub\Editor\6000.5.5f1\Editor\Unity.exe"
)

$raiz = Split-Path -Parent $PSScriptRoot
$carpetaLogs = Join-Path $raiz "Logs\Bot"
New-Item -ItemType Directory -Force $carpetaLogs | Out-Null

if (Get-Process -Name Unity -ErrorAction SilentlyContinue) {
    Write-Host "Unity está abierto: ciérralo antes de correr el bot en modo batch."
    exit 1
}

$fallos = 0
foreach ($modo in $modos) {
    $env:PRUEBA_BOT = "1"
    $env:SOLO_NIVEL = $modo
    $nombre = $modo
    if ($nombre -eq "") { $nombre = "sistema_y_mundos_5_6" }
    if ($nombre.Length -gt 40) { $nombre = "varios_niveles" }
    $log = Join-Path $carpetaLogs "bot_$nombre.log"

    $proceso = Start-Process -FilePath $unity -ArgumentList @('-batchmode', '-projectPath', $raiz, '-executeMethod', 'LanzadorBotPruebas.Ejecutar', '-logFile', $log) -PassThru
    if (-not $proceso.WaitForExit($limiteSegundos * 1000)) {
        Stop-Process -Id $proceso.Id -Force
        Write-Host "BOT CORTADO por tiempo en el modo $nombre"
        $fallos = $fallos + 1
    }

    Write-Host "===== $nombre ====="
    Select-String -Path $log -Pattern "RESULTADO:|BOT FALLO|FALLOS EN|error CS" | ForEach-Object {
        Write-Host $_.Line
        # "BOT FALLO " (con espacio) es una comprobación que falló; "FALLOS EN ...: 0" no lo es.
        $esFallo = $_.Line -match "BOT FALLO |error CS|FALLOS EN .*: [1-9]"
        $nivelPerdido = $_.Line -match "RESULTADO:" -and $_.Line -notmatch "GANADO"
        if ($esFallo -or $nivelPerdido) { $fallos = $fallos + 1 }
    }
}

Remove-Item Env:\PRUEBA_BOT -ErrorAction SilentlyContinue
Remove-Item Env:\SOLO_NIVEL -ErrorAction SilentlyContinue
Write-Host ""
Write-Host "Problemas encontrados: $fallos"
