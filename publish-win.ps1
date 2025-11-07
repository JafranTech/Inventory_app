param(
    [string]$Runtime = "win-x64"
)

# Publishes a single-file, self-contained Windows executable for the InventoryApp
# Usage examples:
#  .\publish-win.ps1             # publishes win-x64
#  .\publish-win.ps1 -Runtime win-x86

Write-Host "Publishing InventoryApp for runtime: $Runtime"

dotnet publish .\InventoryApp.csproj -c Release -r $Runtime --self-contained true `
    -p:PublishSingleFile=true -p:PublishTrimmed=false -p:IncludeNativeLibrariesForSelfExtract=true `
    -o .\publish\$Runtime

if ($LASTEXITCODE -eq 0) {
    Write-Host "Publish succeeded. Output in: .\publish\$Runtime"
} else {
    Write-Error "Publish failed. Check the dotnet output for details."
}
