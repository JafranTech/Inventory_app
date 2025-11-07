; Inno Setup script for InventoryApp
; Edit AppVersion and OutputDir as needed, then open with Inno Setup Compiler (https://jrsoftware.org/isinfo.php)

[Setup]
AppName=InventoryApp
AppVersion=1.0
DefaultDirName={pf}\InventoryApp
DefaultGroupName=InventoryApp
OutputBaseFilename=InventoryAppInstaller
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Include entire publish folder for the chosen runtime (adjust path if you used a different RID)
Source: "{#\"..\\publish\\win-x64\\*\"}"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\InventoryApp"; Filename: "{app}\InventoryApp.exe"
Name: "{group}\Uninstall InventoryApp"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\InventoryApp.exe"; Description: "Launch InventoryApp"; Flags: nowait postinstall skipifsilent
