[Files]
Source: ..\PMS\bin\Release\PMS.dll; DestDir: {app}; Components: PMS
Source: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\log4net.dll; DestDir: {app}; Components: PMS
Source: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\Npgsql.dll; DestDir: {app}; Components: PMS
Source: ..\PMS\bin\Release\PMS.pdb; DestDir: {app}; Components: DBG

[Setup]
VersionInfoCompany=Matthew Metnetsky
DisableDirPage=true
DefaultDirName={pf}\Common Files\PMS
UsePreviousAppDir=false
DefaultGroupName=PMS
ShowLanguageDialog=yes
AppID={{F1EC159F-1DCB-4E97-BF43-EC14D05ED4EC}
AppName=PMS
AppVerName=PMS 0.6.40
AppVersion=0.6.40
AppCopyright=Matthew Metnetsky
AppPublisher=Matthew Metnetsky
AppPublisherURL=http://cowarthill.com/
AppSupportURL=http://cowarthill.com/PMS/
AppUpdatesURL=http://cowarthill.com/PMS/
AppMutex=pms-setup
OutputBaseFilename=pms-setup
VersionInfoVersion=0.6.40
VersionInfoDescription=PMS
VersionInfoCopyright=Matthew Metnetsky
Compression=lzma
InternalCompressLevel=normal
OutputDir=.\
SetupIconFile=setup.ico
LicenseFile=..\debian\copyright
DisableProgramgroupPage=true
UsePreviousGroup=false
AppReadmeFile=..\debian\copyright
SolidCompression=true
PrivilegesRequired=poweruser

[Components]
Name: PMS; Description: Library; Types: compact full custom
Name: DBG; Description: Debug; Types: custom

[_ISTool]
UseAbsolutePaths=false

[Run]
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\PMS.dll"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\log4net.dll"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\Npgsql.dll"""; Components: PMS; Flags: runhidden

[UninstallRun]
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""PMS, Version=0.6.40.5, Culture=neutral, PublicKeyToken=1b9e664700a659b9, processorArchitecture=MSIL"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""log4net, Version=1.2.10.0, Culture=neutral, PublicKeyToken=1b44e1d426115821, processorArchitecture=MSIL"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""Npgsql, Version=0.99.3.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7, processorArchitecture=MSIL"""; Components: PMS; Flags: runhidden

[Registry]
Root: HKLM; Subkey: SOFTWARE\PMS; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: SOFTWARE\PMS\0.6.40; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: SOFTWARE\PMS\0.6.40; ValueType: string; ValueName: Path; ValueData: {app}; Flags: uninsdeletevalue
Root: HKLM; Subkey: SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\PMS; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\PMS; ValueType: string; ValueData: {app}; Flags: uninsdeletevalue

[UninstallDelete]

[Code]
function InitializeSetup(): Boolean;
var
	ErrorCode: Integer;
	DotNet: Boolean;
	Result1: Boolean;
begin
	DotNet := RegKeyExists(HKLM, 'SOFTWARE\Microsoft\.NETFramework\policy\v2.0');
	if DotNet =true then
	begin
		Result := true
	end;

	If DotNet =false then
	begin
		Result1 := MsgBox('This setup requires the .NET Framework v2.0. Do you want to download the framework from Microsoft now?', mbConfirmation, MB_YESNO) = idYes;

		if Result1 =false then
		begin
			Result := false;
		end
		else
		begin
			Result := false;
			ShellExec('open', 'http://www.microsoft.com/downloads/info.aspx?na=90&p=&SrcDisplayLang=en&SrcCategoryId=&SrcFamilyId=0856eacb-4362-4b0d-8edd-aab15c5e04f5&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f5%2f6%2f7%2f567758a3-759e-473e-bf8f-52154438565a%2fdotnetfx.exe', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
		end
	end;
end;


