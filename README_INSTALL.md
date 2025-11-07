How to build a distributable installer (.exe) for InventoryApp

This project is a Windows Forms app targeting .NET 8. The easiest way to create a single EXE and wrap it into a Windows installer is a two-step process:

1) Publish a single-file, self-contained EXE with dotnet
2) Package the published output into an installer using Inno Setup (or WiX/MSIX if you prefer)

Quick steps (PowerShell)

- From the project root run the included publish script (example publishes win-x64):

```powershell
# publish single-file, self-contained EXE (x64)
.\publish-win.ps1 -Runtime win-x64
```

Output location: `publish\win-x64` (the folder will contain `InventoryApp.exe` plus any required native files). The `--self-contained` + `PublishSingleFile` options try to bundle the runtime so users don't need to install .NET.

Notes about publish flags
- PublishSingleFile=true bundles files into one executable.
- PublishTrimmed=false: trimming can break WinForms reflection; keep it off for safety.
- IncludeNativeLibrariesForSelfExtract=true helps native libraries be available when using single-file.

2) Create an installer using Inno Setup

- Download and install Inno Setup: https://jrsoftware.org/isinfo.php
- Open `installer\setup.iss` in Inno Setup Compiler.
- Adjust the `Source` path inside the `[Files]` section if you published for `win-x86` or a different folder.
- Compile the script in Inno Setup; it will produce `InventoryAppInstaller.exe` in the Output folder.

Notes and alternatives
- WiX Toolset: for MSI packages and enterprise scenarios. More complex to author but integrates with CI.
- MSIX / MSIX Packaging: for Microsoft Store or modern deployment, requires additional tooling and manifest changes.
- Code signing: strongly recommended to avoid SmartScreen warnings. Use an EV code signing certificate and sign the final EXE/MSI.

Troubleshooting
- If `dotnet publish` fails or your EXE is still missing dependencies, try publishing without `PublishSingleFile` to a folder to inspect contents:

```powershell
dotnet publish .\InventoryApp.csproj -c Release -r win-x64 --self-contained true -o .\publish\win-x64\debug
```

- If the project is running while you build, the build may warn about locked files. Close running instances before building.

Want me to:
- Create and customize a WiX installer instead of Inno Setup?
- Add automatic packaging in CI (GitHub Actions) that produces an installer artifact?
- Add code-signing hooks (requires certificate)?

Pick one and I’ll implement it next.