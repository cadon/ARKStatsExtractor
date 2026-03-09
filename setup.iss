#define AppName "ARK Smart Breeding"
#define AppPublisher "cadon & friends"
#define AppURL "https://github.com/cadon/ARKStatsExtractor"
#define AppExeName "ARK Smart Breeding.exe"
#define ReleaseDir ".work\dist"
#define OutputDir ".work\publish"
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
DotNetNeededCaption=.NET 10 Desktop Runtime required
de.DotNetNeededCaption=.NET 10 Desktop Runtime benötigt
DotNetNeededDescription=To run {#AppName} the .NET 10 Desktop Runtime is required.
de.DotNetNeededDescription=Um {#AppName} auszuführen wird die .NET 10 Desktop Runtime benötigt.
DotNetNeededSubCaption=Check the box below to download and install the .NET 10 Desktop Runtime.
de.DotNetNeededSubCaption=Markieren Sie das folgende Kästchen, um die .NET 10 Desktop Runtime herunterzuladen und zu installieren.
DotNetInstall=Download and install .NET 10 Desktop Runtime
de.DotNetInstall=Herunterladen und Installation der .NET 10 Desktop Runtime
IDP_DownloadFailed=Download of .NET 10 Desktop Runtime failed. The .NET 10 Desktop Runtime is required to run {#AppName}.
de.IDP_DownloadFailed=Herunterladen der .NET 10 Desktop Runtime fehlgeschlagen. Die .NET 10 Desktop Runtime wird benötigt um {#AppName} auszuführen.
IDP_RetryCancel=Click 'Retry' to try downloading the files again, or click 'Cancel' to terminate setup.
de.IDP_RetryCancel=Klicken Sie 'Wiederholen', um das Herunterladen der Dateien erneut zu versuchen, oder klicken Sie auf "Abbrechen", um die Installation abzubrechen.
InstallingDotNet=Installing .NET 10 Desktop Runtime. This might take a few minutes...
de.InstallingDotNet=Installiere .NET 10 Desktop Runtime. Das wird eine Weile dauern ...
DotNetFailedToLaunch=Failed to launch .NET 10 Desktop Runtime installer with error "%1". Please fix the error then run this installer again.
de.DotNetFailedToLaunch=Der Installer für das .NET 10 Desktop Runtime konnte nicht gestartet werden (Fehler "%1"). Bitte beheben Sie den Fehler und führen Sie den Installer erneut aus.
DotNetFailed1602=.NET 10 Desktop Runtime installation was cancelled. This installation can continue, but be aware that this application may not run unless the .NET 10 Desktop Runtime installation is completed successfully.
de.DotNetFailed1602=Die Installation der .NET 10 Desktop Runtime wurde abgebrochen. Diese Installation kann fortgesetzt werden. Beachten Sie jedoch, dass diese Anwendung möglicherweise nicht ausgeführt wird, bis die Installation der .NET 10 Desktop Runtime erfolgreich abgeschlossen wurde.
DotNetFailed1603=A fatal error occurred while installing the .NET 10 Desktop Runtime. Please fix the error, then run the installer again.
de.DotNetFailed1603=Ein schwerwiegender Fehler trat während der Installation der .NET 10 Desktop Runtime auf. Bitte den Fehler beheben und dieses Installationsprogramm erneut ausführen.
DotNetFailed5100=Your computer does not meet the requirements of the .NET 10 Desktop Runtime.
de.DotNetFailed5100=Ihr Computer erfüllt nicht die Voraussetzungen für die .NET 10 Desktop Runtime.
DotNetFailedOther=The .NET 10 Desktop Runtime installer exited with an unexpected status code "%1". Please review any other messages shown by the installer to determine whether the installation completed successfully, and abort this installation and fix the problem if it did not.
de.DotNetFailedOther=Die Installation der .NET 10 Desktop Runtime endete mit dem nicht erwarteten Statuscode "%1". Überprüfen Sie alle anderen vom Installationsprogramm angezeigten Meldungen, um festzustellen, ob die Installation erfolgreich abgeschlossen wurde, und falls nicht, brechen Sie die Installation ab und beheben Sie das Problem.

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
Source: "{#ReleaseDir}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion
Source: "{#ReleaseDir}\runtimes\*"; DestDir: "{app}\runtimes\"; Excludes: "*.pdb,*.xml"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist

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
Source: "{#ReleaseDir}\json\*.txt"; DestDir: "{localappdata}\{#AppName}\json\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\values\values.json"; DestDir: "{localappdata}\{#AppName}\json\values\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\values\ASA-values.json"; DestDir: "{localappdata}\{#AppName}\json\values\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\values\_manifest.json"; DestDir: "{localappdata}\{#AppName}\json\values\"; Flags: ignoreversion
Source: "{#ReleaseDir}\json\ocr\*.json"; DestDir: "{localappdata}\{#AppName}\json\ocr\"; Flags: ignoreversion skipifsourcedoesntexist


[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#AppName}}"; Filename: "{#AppURL}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Flags: nowait postinstall skipifsilent unchecked; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[UninstallRun]
Filename: "taskkill"; Parameters: "/im ""{#AppExeName}"""; Flags: runhidden

[UninstallDelete]
Type: filesandordirs; Name: "{app}\images"

[Code]
var
  requiresRestart: boolean;
  DotNetPage: TInputOptionWizardPage;
  InstallDotNet: Boolean; 
  downloadFiles: Boolean;
  DownloadPage: TDownloadWizardPage;

function DotNetIsMissing(): Boolean;
var
  ResultCode: Integer;
begin
  // Use dotnet --list-runtimes and check for Microsoft.WindowsDesktop.App 10.x
  // If dotnet is not installed at all, Exec returns False and we need the runtime.
  Result := True;
  if Exec('dotnet', '--list-runtimes', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if ResultCode = 0 then
    begin
      // dotnet exists; check for the desktop runtime via a more reliable registry key
      // HKLM\SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App
      if RegValueExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '10.0.3') then
        Result := False
      else if RegValueExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '10.0.2') then
        Result := False
      else if RegValueExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '10.0.1') then
        Result := False
      else if RegValueExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '10.0.0') then
        Result := False;
    end;
  end;
end;

procedure InitializeWizard;
begin
  DotNetPage := CreateInputOptionPage(wpSelectTasks, ExpandConstant('{cm:DotNetNeededCaption}'),
    ExpandConstant('{cm:DotNetNeededDescription}'), ExpandConstant('{cm:DotNetNeededSubCaption}'), False, False);
  DotNetPage.Add(ExpandConstant('{cm:DotNetInstall}'));
  DotNetPage.Values[0] := True;

  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), nil);
  DownloadPage.Clear;

  WizardForm.WelcomeLabel2.Font.Color := clRed;
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  if (PageID = DotNetPage.ID) and not DotNetIsMissing() then
    Result := True
  else
    Result := False;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if (CurPageID = DotNetPage.ID) and DotNetIsMissing() then begin
    if DotNetPage.Values[0] then begin   
      DownloadPage.Add('https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe', 'DotNetRuntimeInstaller.exe', '');
      InstallDotNet := True;
      downloadFiles := True;
    end;
  end;
  if (CurPageID = wpReady) and downloadFiles then begin
    DownloadPage.Show;
    try
      try
        DownloadPage.Download;
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

function InstallRuntime(): String;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := CustomMessage('InstallingDotNet');
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant('{tmp}\DotNetRuntimeInstaller.exe'), '/install /passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      Result := FmtMessage(CustomMessage('DotNetFailedToLaunch'), [SysErrorMessage(resultCode)]);
    end
    else
    begin
      case resultCode of
        0: begin
          // Successful
        end;
        1602 : begin
          MsgBox(CustomMessage('DotNetFailed1602'), mbInformation, MB_OK);
        end;
        1603: begin
          Result := CustomMessage('DotNetFailed1603');
        end;
        1641: begin
          requiresRestart := True;
        end;
        3010: begin
          requiresRestart := True;
        end;
        5100: begin
          Result := CustomMessage('DotNetFailed5100');
        end;
        else begin
          MsgBox(FmtMessage(CustomMessage('DotNetFailedOther'), [IntToStr(resultCode)]), mbError, MB_OK);
        end;
      end;
    end;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
    
    DeleteFile(ExpandConstant('{tmp}\DotNetRuntimeInstaller.exe'));
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  if InstallDotNet then
  begin
    Result := InstallRuntime();
  end;
end;

function NeedRestart(): Boolean;
begin
  Result := requiresRestart;
end;
