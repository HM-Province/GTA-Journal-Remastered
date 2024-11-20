[Setup]
AppName=GTA Journal
AppVersion=0.1.0-alpha
DefaultDirName={pf}\GTA Journal
DefaultGroupName=GTA Journal
OutputDir=.
OutputBaseFilename=GTAJournalInstaller
Compression=lzma
SolidCompression=yes

[Files]
; Укажите файлы, которые нужно включить в установщик
Source: "output\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
; Создание ярлыков
Name: "{group}\GTA Journal"; Filename: "{app}\GTA Journal.exe"
Name: "{commondesktop}\GTA Journal"; Filename: "{app}\GTA Journal.exe"; Tasks: desktopicon

[Tasks]
; Задача для создания ярлыка на рабочем столе
Name: "desktopicon"; Description: "Создать ярлык на рабочем столе"; GroupDescription: "Дополнительно:"; Flags: unchecked

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"