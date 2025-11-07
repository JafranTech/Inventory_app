******🧾 InventoryApp******

A simple Windows Forms Inventory Management app built using C# (.NET 8).
This project shows how to build, run, and publish a desktop app using VS Code and GitHub.
----------------------------------------------------------------------------------------------------
******⚙️ Requirements******

Before running, make sure you have:

Windows OS (since it’s a WinForms app)

.NET SDK 8.x → check version:

dotnet --version

----------------------------------------------------------------------------------------------------

Visual Studio Code + C# extension (ms-dotnettools.csharp)

Optional (for installer creation):

Inno Setup

🚀 How to Open and Run
1. Clone the Project
git clone https://github.com/YourUsername/Inventory_app.git
cd Inventory_app

2. Open in VS Code
code .

----------------------------------------------------------------------------------------------------

If VS Code asks to add build/debug assets — click Yes.

▶️ Run the App

In VS Code terminal:

dotnet build -c Release
dotnet run --project .\InventoryApp.csproj


This opens the InventoryApp window — you can add products, view inventory, reset data, and toggle dark mode.

🐞 Run with Debugger

Press F5 → VS Code will build and start debugging.
Stop debugging → app closes automatically.

----------------------------------------------------------------------------------------------------

🧱 Build a Standalone EXE

Use the PowerShell script provided:

.\publish-win.ps1 -Runtime win-x64


Output:

.\publish\win-x64\InventoryApp.exe


✅ This EXE runs on any Windows PC (no .NET install needed).
⚠️ Note: It’s large (~150 MB) because it includes the runtime.

If PowerShell blocks the script, run:

powershell -ExecutionPolicy Bypass -File .\publish-win.ps1 -Runtime win-x64

📦 Create an Installer (Optional)

Publish the app first.

Open installer/setup.iss in Inno Setup Compiler.

Compile → You’ll get InventoryAppInstaller.exe.

----------------------------------------------------------------------------------------------------

🧹 GitHub Notes

Your .gitignore skips:

/publish/
bin/
obj/
*.exe
*.pdb


So only your source code is uploaded — not large build files.

If you want to share the .exe, upload it under GitHub → Releases → New Release → Attach EXE.

🛠 Features

🌓 Dark Mode toggle

🔄 Reset Data button

🧹 Clear Output button

⚙️ PowerShell script to build EXE

📦 Inno Setup script to make installer

❗Common Issues

File locked: Close any running InventoryApp.exe before rebuilding.

SmartScreen warning: Sign your EXE to avoid this.

Missing files: Try this for a detailed publish:

dotnet publish -c Release -r win-x64 --self-contained true -o .\publish\win-x64\debug

🚧 Future Enhancements

GitHub Actions to auto-build EXE & upload as release.

MSI installer (WiX).

Save user preferences (e.g., Dark Mode).

Add unit tests.
