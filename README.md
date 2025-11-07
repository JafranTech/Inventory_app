😄😄😄JUST CLICK ->DOWNLOAD->GET FILE->ENJOY😄😄😄

## 🧾 InventoryApp Executable (EXE)
If you want to **run the application directly** without building the code, download the ready-to-use **InventoryApp.exe** or **InventoryAppInstaller.exe** from this repository:  
👉 [Download InventoryApp Executable Files](https://github.com/JafranTech/InventoryApp-Builds)

The code in this repository contains only the **source files**, while the **EXE link above** provides the **compiled version** built using .NET 8 (self-contained Windows app).


# InventoryApp

A small Windows Forms inventory management demo written in C# and .NET 8.

This README explains how to run and develop this project in Visual Studio Code (VS Code), how to publish a self-contained .exe, and where to find the installer script.

---

## Prerequisites

- Windows (the UI is WinForms)
- .NET SDK 8.x installed (dotnet CLI available). Verify with:

```powershell
dotnet --version
```

- Visual Studio Code
- C# extension for VS Code (ms-dotnettools.csharp)

Optional:
- Inno Setup (if you want to build an installer): https://jrsoftware.org/isinfo.php

---

## Opening the project in VS Code

1. Clone your repo (if not already):

```powershell
git clone https://github.com/YourUsername/Inventory_app.git
cd Inventory_app
```

2. Open the folder in VS Code:

- From PowerShell:

```powershell
code .
```

- Or use File → Open Folder in the VS Code UI.

3. If VS Code prompts to add required assets for building and debugging, accept them. Ensure the C# extension is installed.

---

## Restore, build and run (via terminal)

From the project root (where `InventoryApp.csproj` lives) you can use the terminal in VS Code.

```powershell
# Restore packages (usually not required because dotnet will restore automatically)
dotnet restore

# Build in Release (or Debug)
dotnet build -c Release

# Run the app (runs the WinForms UI)
dotnet run --project .\InventoryApp.csproj
```

When running you should see the InventoryApp window. Use the UI to add products, view inventory, use the Reset Data and Clear Output controls, and toggle Dark Mode.

---

## Run / Debug from VS Code (F5)

1. If you don't have a `.vscode/launch.json` file, VS Code will offer to create one. Choose `.NET 6+` / `C# (Windows)` scenario.
2. Press F5 to build and start debugging. Breakpoints in the C# code will be hit.

Note: WinForms apps launched with the debugger will open a UI window. When stopping the debugger, the app will close.

---

## Publish a self-contained .exe (single-file)

A helper script `publish-win.ps1` is included to publish a single-file self-contained EXE for Windows.

From PowerShell in the project root:

```powershell
# x64 example
.\publish-win.ps1 -Runtime win-x64
```

This produces `./publish/win-x64/InventoryApp.exe` (self-contained). You can run that .exe directly — it includes the .NET runtime.

Notes:
- Self-contained single-file builds are large (~100+ MB) because they bundle the runtime.
- If you prefer a smaller output, publish framework-dependent and require users to install the .NET runtime.

If PowerShell refuses to run the script due to execution policy, run it with the bypass option:

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-win.ps1 -Runtime win-x64
```

---

## Create an installer (Inno Setup)

An example `installer\setup.iss` Inno Setup script is included. Steps:

1. Publish the app as above (so the `publish\win-x64` folder contains the exe).
2. Install Inno Setup and open `installer\setup.iss` in the Inno Setup Compiler.
3. Adjust the `[Files]` source path if your publish path differs.
4. Compile the script — the compiler will generate an `InventoryAppInstaller.exe` installer.

See `README_INSTALL.md` for details.

---

## What I added in this branch / project

- Dark Mode toggle on the main form.
- `Reset Data` button: clears the in-memory inventory list (no persistence) and clears the output panel.
- `Clear Output` button: clears the right-side output textbox.
- `publish-win.ps1` to produce a self-contained EXE.
- `installer/setup.iss` and `README_INSTALL.md` to create an Inno Setup installer.

---

## Troubleshooting

- Locked files when building/publishing: close any running InventoryApp.exe before building/publishing — the running application locks the output EXE and causes copy warnings.
- SmartScreen / Antivirus warnings for unsigned installers or binaries: code-sign your binaries to avoid warnings.
- If publish appears to be missing files, try publishing without single-file to inspect the published folder:

```powershell
dotnet publish .\InventoryApp.csproj -c Release -r win-x64 --self-contained true -o .\publish\win-x64\debug
```

---

## Git / GitHub notes

You already added a `.gitignore` that ignores `publish/`, `bin/`, `obj/`, `*.exe` and `*.pdb`. Good — do not commit built binaries into the repository. Instead, attach published artifacts to a GitHub Release or use CI to publish artifacts.

If you'd like, I can add a GitHub Actions workflow to build the project and upload the EXE (and/or installer) as release artifacts automatically.

---

## Want me to help with next steps?

I can:
- Add a GitHub Actions workflow that builds and publishes the EXE and/or installer as artifacts.
- Create an MSI installer (WiX) instead of Inno Setup.
- Persist user preferences (Dark Mode) to disk and restore them on startup.
- Add unit tests and configure `dotnet test` in CI.

Tell me which you'd like and I’ll implement it.
