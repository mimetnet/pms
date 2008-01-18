[Files]
Source: ..\PMS\bin\Release\PMS.dll; DestDir: {app}\bin; Components: PMS
Source: ..\PMS\bin\Release\PMS.pdb; DestDir: {app}\bin; Components: DBG
Source: ..\..\..\..\..\WINDOWS\Microsoft.NET\Framework\v2.0.50727\log4net.dll; DestDir: {app}\bin; Components: PMS
Source: ..\..\..\..\..\WINDOWS\Microsoft.NET\Framework\v2.0.50727\Npgsql.dll; DestDir: {app}\bin; Components: PGSQL
Source: ..\..\..\..\..\Program Files\SQLite.NET\bin\System.Data.SQLite.DLL; DestDir: {app}\bin; Components: SQLite
Source: ..\PMS.Data.Pgsql\bin\Release\PMS.Data.Pgsql.dll; DestDir: {app}\bin; Components: PGSQL
Source: ..\PMS.Data.Sqlite\bin\Release\PMS.Data.Sqlite.dll; DestDir: {app}\bin; Components: SQLite
Source: ..\scripts\pms-providers; DestDir: {app}\scripts; Components: PMS; DestName: pms-providers.py

[Setup]
VersionInfoCompany=Matthew Metnetsky
DisableDirPage=true
DefaultDirName={pf}\Common Files\PMS
UsePreviousAppDir=false
DefaultGroupName=PMS
ShowLanguageDialog=yes
AppID={{F1EC159F-1DCB-4E97-BF43-EC14D05ED4EC}
AppName=PMS
AppVerName=PMS 0.6.91
AppVersion=0.6.91
AppCopyright=Matthew Metnetsky
AppPublisher=Matthew Metnetsky
AppPublisherURL=http://cowarthill.com/
AppSupportURL=http://cowarthill.com/PMS/
AppUpdatesURL=http://cowarthill.com/PMS/
AppMutex=pms-setup
OutputBaseFilename=pms-setup-0.6.91
VersionInfoVersion=0.6.91
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
Name: PMS; Description: Library; Types: full custom
Name: PGSQL; Description: PMS Support for Postgresql 8.x; Types: full
Name: SQLite; Description: PMS Support for SQLite 3.x; Types: full
Name: DBG; Description: Debug; Types: custom

[_ISTool]
UseAbsolutePaths=false

[Run]
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\bin\PMS.dll"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\bin\PMS.Data.Pgsql.dll"""; Components: PGSQL; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\bin\PMS.Data.Sqlite.dll"""; Components: SQLite; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\bin\log4net.dll"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\bin\Npgsql.dll"""; Components: PGSQL; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/i ""{app}\bin\System.Data.SQlite.dll"""; Components: SQLite; Flags: runhidden
Filename: {app}\scripts\pms-providers.py; Parameters: "sqlite ""PMS.Data.Sqlite.SqliteProvider, PMS.Data.Sqlite"""; Components: SQLite; Flags: shellexec runhidden
Filename: {app}\scripts\pms-providers.py; Parameters: "pgsql ""PMS.Data.Postgresql.PostgresqlProvider, PMS.Data.Pgsql"""; Components: PGSQL; Flags: shellexec runhidden

[UninstallRun]
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""PMS, Version=0.6.91.0, Culture=neutral, PublicKeyToken=1b9e664700a659b9, processorArchitecture=MSIL"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""PMS.Data.Pgsql, Version=0.3.0.0, Culture=neutral, PublicKeyToken=1b9e664700a659b9, processorArchitecture=MSIL"""; Components: PGSQL; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""PMS.Data.Sqlite, Version=0.0.1.0, Culture=neutral, PublicKeyToken=1b9e664700a659b9, processorArchitecture=MSIL"""; Components: SQLite; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""log4net, Version=1.2.10.0, Culture=neutral, PublicKeyToken=1b44e1d426115821, processorArchitecture=MSIL"""; Components: PMS; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""Npgsql, Version=0.99.3.0, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7, processorArchitecture=MSIL"""; Components: PGSQL; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""System.Data.SQLite, Version=1.0.48.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139, processorArchitecture=x86"""; Components: SQLite; Flags: runhidden
Filename: {reg:HKLM\SOFTWARE\GTK\2.0,Path|C:\GTK\2.8}\bin\gacco.exe; Parameters: "/u ""System.Data.SQLite, Version=1.0.48.0, Culture=neutral, PublicKeyToken=1fdb50b1b62b4c84, processorArchitecture=MSIL"""; Components: SQLite; Flags: runhidden

[Registry]
Root: HKLM; Subkey: SOFTWARE\PMS; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: SOFTWARE\PMS\0.6.91; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: SOFTWARE\PMS\0.6.91; ValueType: string; ValueName: Path; ValueData: {app}\bin; Flags: uninsdeletevalue
Root: HKLM; Subkey: SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\PMS; Flags: uninsdeletekeyifempty
Root: HKLM; Subkey: SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\PMS; ValueType: string; ValueData: {app}\bin; Flags: uninsdeletevalue

[Code]
function InitializeSetup(): Boolean;
var
	ErrorCode: Integer;
	DotNet: Boolean;
	Result1: Boolean;
	PMS: Boolean;
begin
	PMS := RegKeyExists(HKLM, 'SOFTWARE\PMS\0.6.91');

	if PMS =true then
	begin
		MsgBox('Already installed', mbInformation, MB_OK);
		Result := false
	end
	else
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
end;

//procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
//var
//	Result1: Boolean;
//begin
//	case CurUninstallStep of
//		usUninstall :
//			Result1 := MsgBox('uninstall', mbConfirmation, MB_YESNO) = idYes;
//	end;
//end;

[UninstallDelete]
Name: {app}\etc\providers; Type: files; Components: PMS
Name: {app}\etc; Type: dirifempty; Components: PMS
Name: {app}; Type: dirifempty; Components: PMS
