$filePath = "setup.iss"

$appVersionLine = Select-String -Path $filePath -Pattern "AppVersion=" | ForEach-Object { $_.Line }

if ($appVersionLine) {
    $appVersion = $appVersionLine -replace "AppVersion=", ""
    Write-Output "version=$appVersion"
} else {
    Write-Output "version=0.0.0"
}