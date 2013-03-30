$dir = Split-Path -parent $PSCommandPath

& MSBuild.exe /t:Rebuild "$dir\LeveledUp.sln"

& "C:\Program Files (x86)\Google\Chrome\Application\chrome.exe" --pack-extension="$dir\chrome_extension\" --pack-extension-key="$dir\chrome_extension.pem"

[System.Threading.Thread]::Sleep(1000)
move "$dir\chrome_extension.crx" "$dir\chrome_extension\chrome_extension.crx"

& MSBuild.exe "$dir\msi\MSI.wixproj" /t:Rebuild /p:Configuration=Release /p:Platform="x64"
copy "$dir\msi\bin\x64\Release\*.msi" "$dir\build\"
