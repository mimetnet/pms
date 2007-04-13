[Files]
Source: ..\PMS\bin\Release\PMS.dll; DestDir: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727; Components: PMS
Source: ..\PMS\bin\Release\PMS.pdb; DestDir: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727; Components: DBG

[Setup]
VersionInfoCompany=Matthew Metnetsky
DisableDirPage=true
DefaultDirName={pf}\Common Files\PMS
UsePreviousAppDir=false
DefaultGroupName=PMS
ShowLanguageDialog=yes
AppID={{E58630D8-2042-40B8-ABF2-B4AE9A45CEC6}
UninstallDisplayName=PMS
AppCopyright=Matthew Metnetsky
AppName=PMS
AppMutex=pms-setup
AppVerName=0.6.38
OutputBaseFilename=pms-setup
VersionInfoVersion=0.6.38
VersionInfoDescription=PMS
VersionInfoCopyright=Matthew Metnetsky
Compression=lzma
InternalCompressLevel=normal
AppPublisher=PMS
AppVersion=0.6.38
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
Filename: C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\gacutil; Parameters: "/i ""{app}\PMS.dll"""; Components: PMS; Flags: runhidden

[Registry]

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

[UninstallRun]
Filename: C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\gacutil; Parameters: /u PMS; Components: PMS; Flags: runhidden
