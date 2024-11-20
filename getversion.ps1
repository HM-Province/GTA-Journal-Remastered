$filePath = "setup.iss"

$appVersionLine = Select-String -Path $filePath -Pattern "AppVersion=" | ForEach-Object { $_.Line }

if ($appVersionLine) {
    $appVersion = $appVersionLine -replace "AppVersion=", ""
    Write-Output "JOURNAL_VERSION=$appVersion"
} else {
    Write-Output "JOURNAL_VERSION=0.0.0"
}