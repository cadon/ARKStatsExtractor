#define AppName "ARK Smart Breeding"
#define AppPublisher "cadon & friends"
#define AppURL "https://github.com/cadon/ARKStatsExtractor"
#define AppExeName "ARK Smart Breeding.exe"
#define ReleaseDir "ARKBreedingStats\bin\Release"
#define ReleaseDirUpdater "ASB-Updater\bin\Release"
#define OutputDir "_publish"
#define AppVersion GetVersionNumbersString(ReleaseDir + "\" + AppExeName)

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{8DDA440C-714D-4BE6-AD7B-F549ABB1BB02}
AppName={#AppName}
AppVersion={#AppVersion}  
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={commonpf}\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
DisableWelcomePage=no
OutputDir={#OutputDir}
OutputBaseFilename=setup-ArkSmartBreeding-{#AppVersion}
Compression=lzma
SolidCompression=yes
CloseApplications=yes
UninstallDisplayIcon={app}\{#AppExeName}

[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.%n%nIf you plan to run [name] as a portable version in a shared location (i.e. not in the system's Program Files folder), we recommend to use the zip file version instead of this installer.
de.WelcomeLabel2=Dieser Assistent wird jetzt [name/ver] auf Ihrem Computer installieren.%n%nWenn Sie planen [name] als portable Version in einem gemeinsam genutzten Verzeichnis (das heißt, außerhalb des Verzeichnisses für Programme) auszuführen, empfehlen wir anstelle dieses Installationsprogramms die Zip-Datei-Version zu nutzen.

[CustomMessages]
DotNetFrameworkNeededCaption=.NET Framework 4.8 required
de.DotNetFrameworkNeededCaption=.NET Framework 4.8 benötigt
DotNetFrameworkNeededDescription=To run {#AppName} the .NET Framework 4.8 is required.
de.DotNetFrameworkNeededDescription=Um {#AppName} auszuführen wird .NET Framework 4.8 benötigt.
DotNetFrameworkNeededSubCaption=Check the box below to download and install .NET Framework 4.8.
de.DotNetFrameworkNeededSubCaption=Markieren Sie das folgende Kästchen, um .NET Framework 4.8 herunterzuladen und zu installieren.
DotNetFrameworkInstall=Download and install .NET Framework 4.8
de.DotNetFrameworkInstall=Herunterladen und Installation von .NET Framework 4.8
IDP_DownloadFailed=Download of .NET Framework 4.8 failed. .NET Framework 4.8 is required to run {#AppName}.
de.IDP_DownloadFailed=Herunterladen von .NET Framework 4.8 fehlgeschlagen. .NET Framework 4.8 wird benötigt um {#AppName} auszuführen.
IDP_RetryCancel=Click 'Retry' to try downloading the files again, or click 'Cancel' to terminate setup.
de.IDP_RetryCancel=Klicken Sie 'Wiederholen', um das Herunterladen der Dateien erneut zu versuchen, oder klicken Sie auf "Abbrechen", um die Installation abzubrechen.
InstallingDotNetFramework=Installing .NET Framework 4.8. This might take a few minutes...
de.InstallingDotNetFramework=Installiere .NET Framework 4.8. Das wird eine Weile dauern ...
DotNetFrameworkFailedToLaunch=Failed to launch .NET Framework Installer with error "%1". Please fix the error then run this installer again.
de.DotNetFrameworkFailedToLaunch=Starten des .NET Framework Installer fehlgeschlagen mit Fehler "%1". Bitte den Fehler beheben und dieses Installationsprogramm erneut ausführen.
DotNetFrameworkFailed1602=.NET Framework installation was cancelled. This installation can continue, but be aware that this application may not run unless the .NET Framework installation is completed successfully.
de.DotNetFrameworkFailed1602=Die .NET Framework Installation wurde abgebrochen. Diese Installation kann fortgesetzt werden. Beachten Sie jedoch, dass diese Anwendung möglicherweise nicht ausgeführt wird, bis die .NET Framework-Installation erfolgreich abgeschlossen wurde.
DotNetFrameworkFailed1603=A fatal error occurred while installing the .NET Framework. Please fix the error, then run the installer again.
de.DotNetFrameworkFailed1603=Ein schwerwiegender Fehler trat während der Installiion des .NET Frameworks auf. Bitte den Fehler beheben und dieses Installationsprogramm erneut ausführen.
DotNetFrameworkFailed5100=Your computer does not meet the requirements of the .NET Framework.
de.DotNetFrameworkFailed5100=Ihr Computer erfüllt nicht die Voraussetzungen für das .NET Framework.
DotNetFrameworkFailedOther=The .NET Framework installer exited with an unexpected status code "%1". Please review any other messages shown by the installer to determine whether the installation completed successfully, and abort this installation and fix the problem if it did not.
de.DotNetFrameworkFailedOther=Die .NET Framework Installation endete mit dem nicht erwarteten Statuscode "%1". Überprüfen Sie alle anderen vom Installationsprogramm angezeigten Meldungen, um festzustellen, ob die Installation erfolgreich abgeschlossen wurde, und falls nicht, brechen Sie die Installation ab und beheben Sie das Problem.
DownloadImages=Download images for some of the species (~21 MB).
de.DownloadImages=Bilder für einige der Dinos herunterladen (~21 MB).
CreatureImages=Additional creature images
de.CreatureImages=Zusätzliche Dino-Bilder
InstallImages=Installing additionally creature images
de.InstallImages=Installiere zusätzliche Dino-Bilder

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
; images download is configured in function NextButtonClick, unzipping is run as powershell command
Name: "images"; Description: "{cm:DownloadImages}"; GroupDescription: "{cm:CreatureImages}"; Flags: checkedonce
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
Source: "{#ReleaseDir}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion
Source: "{#ReleaseDir}\de\*"; DestDir: "{app}\de\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\es\*"; DestDir: "{app}\es\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\fr\*"; DestDir: "{app}\fr\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\it\*"; DestDir: "{app}\it\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\ja\*"; DestDir: "{app}\ja\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\pl\*"; DestDir: "{app}\pl\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\pt-BR\*"; DestDir: "{app}\pt-BR\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\ru\*"; DestDir: "{app}\ru\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\tr\*"; DestDir: "{app}\tr\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\zh\*"; DestDir: "{app}\zh\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDir}\_manifest.json"; DestDir: "{localappdata}\{#AppName}\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\*.json"; DestDir: "{localappdata}\{#AppName}\json\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\values\values.json"; DestDir: "{localappdata}\{#AppName}\json\values\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\values\_manifest.json"; DestDir: "{localappdata}\{#AppName}\json\values\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\ocr\*.json"; DestDir: "{localappdata}\{#AppName}\json\ocr\"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#ReleaseDirUpdater}\asb-updater.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#AppName}}"; Filename: "{#AppURL}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "powershell.exe"; Parameters: "-nologo -noprofile -command ""& {{ Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('{tmp}\speciesImages.zip', '{localappdata}\{#AppName}\images\speciesImages\'); }"""; Flags: runminimized; StatusMsg: "{cm:InstallImages}"; Tasks: images
Filename: "{app}\{#AppExeName}"; Flags: nowait postinstall skipifsilent unchecked; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[UninstallRun]
Filename: "taskkill"; Parameters: "/im ""{#AppExeName}"""; Flags: runhidden

[UninstallDelete]
Type: filesandordirs; Name: "{app}\images"

[Code]
var
  requiresRestart: boolean;
  DotNetPage: TInputOptionWizardPage;
  InstallDotNetFramework: Boolean; 
  downloadFiles: Boolean;
  DownloadPage: TDownloadWizardPage;

function DotNetFrameworkIsMissing(): Boolean;
var
  bSuccess: Boolean;
  regVersion: Cardinal;
begin
  Result := True;

  // determine installed .NET version via
  // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed#detect-net-framework-45-and-later-versions
  bSuccess := RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', regVersion);
  if (True = bSuccess) and (regVersion >= 528040) then begin
    Result := False;
  end;
end;

procedure InitializeWizard;
begin
  DotNetPage := CreateInputOptionPage(wpSelectTasks, ExpandConstant('{cm:DotNetFrameworkNeededCaption}'),
    ExpandConstant('{cm:DotNetFrameworkNeededDescription}'), ExpandConstant('{cm:DotNetFrameworkNeededSubCaption}'), False, False);
  DotNetPage.Add(ExpandConstant('{cm:DotNetFrameworkInstall}'));
  DotNetPage.Values[0] := True;

  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), nil);
  DownloadPage.Clear;

  //WizardForm.WelcomeLabel2.Font.Style := [fsBold]; //Bold
  WizardForm.WelcomeLabel2.Font.Color := clRed; // And red colour
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  if (PageID = DotNetPage.ID) and not DotNetFrameworkIsMissing() then
    Result := True
  else
    Result := False;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if (CurPageID = DotNetPage.ID) and DotNetFrameworkIsMissing() then begin
    if DotNetPage.Values[0] then begin   
      DownloadPage.Add('https://go.microsoft.com/fwlink/?LinkId=2085155', 'NetFrameworkInstaller.exe', '');
      InstallDotNetFramework := True;
      downloadFiles := True;
    end;
  end;
  if (CurPageID = wpSelectTasks) then begin
    if WizardIsTaskSelected('images') then begin
      DownloadPage.Add('{#AppURL}/raw/master/speciesImages/speciesImages.zip', 'speciesImages.zip', '');
      downloadFiles := True;
      end;
  end;
    if (CurPageID = wpReady) and downloadFiles then begin
    DownloadPage.Show;
    try
      try
        DownloadPage.Download; // This downloads the files to {tmp}
        Result := True;
      except
        if DownloadPage.AbortedByUser then
          Log('Aborted by user.')
        else
          SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
        Result := False;
      end;
    finally
      DownloadPage.Hide;
    end;
  end;
  Result := True;
end;

function InstallFramework(): String;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := CustomMessage('InstallingDotNetFramework');
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'), '/passive /norestart /showrmui /showfinalerror', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := FmtMessage(CustomMessage('DotNetFrameworkFailedToLaunch'), [SysErrorMessage(resultCode)]);
    end
    else
    begin
      // See https://msdn.microsoft.com/en-us/library/ee942965(v=vs.110).aspx#return_codes
      case resultCode of
        0: begin
          // Successful
        end;
        1602 : begin
          MsgBox(CustomMessage('DotNetFrameworkFailed1602'), mbInformation, MB_OK);
        end;
        1603: begin
          Result := CustomMessage('DotNetFrameworkFailed1603');
        end;
        1641: begin
          requiresRestart := True;
        end;
        3010: begin
          requiresRestart := True;
        end;
        5100: begin
          Result := CustomMessage('DotNetFrameworkFailed5100');
        end;
        else begin
          MsgBox(FmtMessage(CustomMessage('DotNetFrameworkFailedOther'), [IntToStr(resultCode)]), mbError, MB_OK);
        end;
      end;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
    
    DeleteFile(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  // 'NeedsRestart' only has an effect if we return a non-empty string, thus aborting the installation.
  // If the installers indicate that they want a restart, this should be done at the end of installation.
  // Therefore we set the global 'restartRequired' if a restart is needed, and return this from NeedRestart()

  if InstallDotNetFramework then
  begin
    Result := InstallFramework();
  end;
end;

function NeedRestart(): Boolean;
begin
  Result := requiresRestart;
end;
