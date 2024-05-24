$script_dir = Split-Path -Parent $MyInvocation.MyCommand.Definition
New-Item -Path (Join-Path -Path $script_dir -ChildPath "bin") -ItemType Directory -Force
Remove-Item -Path (Join-Path -Path $script_dir -ChildPath "bin/*") -Force
New-Item -Path (Join-Path -Path $script_dir -ChildPath "package/release") -ItemType Directory -Force
Copy-Item -Path (Join-Path -Path $script_dir -ChildPath ".\Source\bin\Release\net7.0\*.dll") -Destination (Join-Path -Path $script_dir -ChildPath "bin") -Force -Recurse
Copy-Item -Path (Join-Path -Path $script_dir -ChildPath ".\Source\bin\Release\net7.0\*.pdb") -Destination (Join-Path -Path $script_dir -ChildPath "bin") -Force -Recurse

$Files = New-Object -TypeName 'System.Collections.ArrayList'
$Files.Add(".\bin")
$Files.Add(".\everest.yaml")

Compress-Archive -Path $Files -DestinationPath (Join-Path -Path $script_dir -ChildPath "package/release/CelesteTASLobbyWarpHelper.zip") -Force