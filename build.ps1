$dir = Split-Path -parent $PSCommandPath

& MSBuild.exe /t:Rebuild "$dir\LeveledUp.sln"

& "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --pack-extension="$dir\chrome_extension\src\" --pack-extension-key="$dir\chrome_extension\chrome_extension.pem"

[System.Threading.Thread]::Sleep(1000)
move -Force "$dir\chrome_extension\src.crx" "$dir\chrome_extension\chrome_extension.crx"

& MSBuild.exe "$dir\Installer\Installer.wixproj" /t:Rebuild /p:Configuration=Release /p:Platform="x64"
copy "$dir\Installer\bin\x64\Release\*.exe" "$dir\build\"
