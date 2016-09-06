Param($action)

Set-Alias migrate "..\..\packages\FluentMigrator.Tools.1.6.2\tools\x86\40\Migrate.exe"



$migrationsDll = "..\bin\Debug\Traffix.migrations.dll"
$devConnectionString = "TraffixDb"

	if ($action -eq "m")
	{
		migrate  -c "$devConnectionString" -a "$migrationsDll" -db SqlServer2012 
		return
	}

	if ($action -eq "mt")
	{
		migrate  -c "$testConnectionString" -a "$migrationsDll" -db SqlServer2012 
		return
	}

	if ($action -eq "md")
	{
		migrate  -c "$devConnectionString" -a "$migrationsDll" -db SqlServer2012 --profile=Development
		return
	}

	if ($action -eq "mdt")
	{
		migrate  -c "$testConnectionString" -a "$migrationsDll" -db SqlServer2012 --profile=Development
		return
	}

	if ($action -eq "r")
	{
		migrate  -c "$devConnectionString" -a "$migrationsDll" -db SqlServer2012 -t rollback 
		return
	}

	if ($action -eq "a")
	{
		migrate  -c "$devConnectionString" -a "$migrationsDll" -db SqlServer2012 -t rollback:all 
		return
	}

	if ($action -eq "at")
	{
		migrate  -c "$testConnectionString" -a "$migrationsDll" -db SqlServer2012 -t rollback:all 
		return
	}


	Write-Host "Action? [m]igrate, [r]ollback, rollback [a]ll, append t for test environment" -foregroundcolor yellow