<#
.SYNOPSIS
    Script tu dong hoa Deploy/Publish ma nguon BVTL-X len may chu IIS qua FTP (Ho tro da moi truong Prod/Dev).
.DESCRIPTION
    Cac che do:
      - Patch   : Chi day cac file thay doi (DLL, Views, Controllers) - cuc nhanh (3-15 giay).
      - Full    : Day toan bo thu muc WebApp_Publish (khoang 4,500 file).
      - Package : Day file nen release (.rar / .zip) vao thu muc Packages/ tren server.
      - Status  : Kiem tra trang thai ket noi FTP va thu muc tren server.
    Moi truong:
      - Prod    : Day vao thu muc Prod/, su dung config Prod (BVTL_REPORTING, debug=false, IsDev=0), healthcheck das.scdi.org.vn
      - Dev     : Day vao thu muc Dev/, su dung config Dev (BVTL_REPORTING_DEV, debug=true, IsDev=1), healthcheck port 8090
.EXAMPLE
    .\deploy-ftp.ps1 -Mode Status
    .\deploy-ftp.ps1 -Mode Full -Environment Prod -IncludeConfig
    .\deploy-ftp.ps1 -Mode Patch -Environment Prod
    .\deploy-ftp.ps1 -Mode Full -Environment Dev -IncludeConfig
#>

[CmdletBinding(SupportsShouldProcess = $true)]
param (
    [Parameter(Position = 0)]
    [ValidateSet("Status", "Patch", "Full", "Package")]
    [string]$Mode = "Patch",

    [Parameter()]
    [ValidateSet("Prod", "Dev")]
    [string]$Environment = "Prod",

    [Parameter()]
    [string]$Domain = "das.scdi.org.vn",

    [Parameter()]
    [string]$FtpHost = "103.77.167.206",

    [Parameter()]
    [int]$FtpPort = 21,

    [Parameter()]
    [string]$FtpUser = "Publisher",

    [Parameter()]
    [string]$FtpPass = "DuanCD45@2026",

    [Parameter()]
    [string]$RemoteDir = "",

    [Parameter()]
    [string]$SourceDir = "D:\Deploy\WebApp_Publish",

    [Parameter()]
    [string]$PackagePath = "D:\Deploy\BVTL_WebApp_Publish_v1.1.0.rar",

    [Parameter()]
    [datetime]$SinceDate = [datetime]"2026-08-17",

    [Parameter()]
    [switch]$IncludeConfig = $false
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RemoteDir)) {
    $RemoteDir = $Environment
}

function Get-FtpBaseUrl {
    $cleanRemote = $RemoteDir.Trim("/").Trim("\")
    if ([string]::IsNullOrWhiteSpace($cleanRemote)) {
        return "ftp://$FtpHost`:$FtpPort"
    }
    return "ftp://$FtpHost`:$FtpPort/$cleanRemote"
}

$Script:CreatedDirs = @{}

function Ensure-RemoteDirectory {
    param ([string]$RelativeDirPath)

    if ([string]::IsNullOrWhiteSpace($RelativeDirPath)) { return }
    $parts = $RelativeDirPath.Replace("\", "/").Trim("/").Split("/")
    $accum = ""

    foreach ($part in $parts) {
        if ([string]::IsNullOrWhiteSpace($part)) { continue }
        $accum = if ($accum -eq "") { $part } else { "$accum/$part" }

        if ($Script:CreatedDirs.ContainsKey($accum)) { continue }

        $url = "$(Get-FtpBaseUrl)/$accum"
        try {
            $req = [System.Net.FtpWebRequest]::Create($url)
            $req.Credentials = New-Object System.Net.NetworkCredential($FtpUser, $FtpPass)
            $req.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
            $req.UsePassive = $true
            $req.KeepAlive = $false
            $req.Timeout = 5000
            $resp = $req.GetResponse()
            $resp.Close()
        } catch {
            # Thu muc da ton tai la binh thuong
        }
        $Script:CreatedDirs[$accum] = $true
    }
}

function Upload-SingleFile {
    param (
        [string]$LocalFilePath,
        [string]$RelativeFilePath
    )

    $relativeNorm = $RelativeFilePath.Replace("\", "/").TrimStart("/")
    $dirName = [System.IO.Path]::GetDirectoryName($RelativeFilePath)
    if (-not [string]::IsNullOrWhiteSpace($dirName)) {
        Ensure-RemoteDirectory -RelativeDirPath $dirName
    }

    $targetUrl = "$(Get-FtpBaseUrl)/$relativeNorm"
    $fileInfo = Get-Item $LocalFilePath

    $maxRetries = 3
    $attempt = 0
    $success = $false

    while (-not $success -and $attempt -lt $maxRetries) {
        $attempt++
        try {
            $req = [System.Net.FtpWebRequest]::Create($targetUrl)
            $req.Credentials = New-Object System.Net.NetworkCredential($FtpUser, $FtpPass)
            $req.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
            $req.UsePassive = $true
            $req.UseBinary = $true
            $req.KeepAlive = $true
            $req.Timeout = 15000
            $req.ContentLength = $fileInfo.Length

            $fileStream = [System.IO.File]::OpenRead($LocalFilePath)
            $reqStream = $req.GetRequestStream()

            $buffer = New-Object byte[] 65536
            $bytesRead = 0
            while (($bytesRead = $fileStream.Read($buffer, 0, $buffer.Length)) -gt 0) {
                $reqStream.Write($buffer, 0, $bytesRead)
            }

            $reqStream.Close()
            $fileStream.Close()

            $resp = $req.GetResponse()
            $resp.Close()
            $success = $true
        } catch {
            if ($attempt -ge $maxRetries) {
                throw "Loi upload file [$relativeNorm] sau $maxRetries lan thu: $($_.Exception.Message)"
            }
            Start-Sleep -Milliseconds 1000
        }
    }
}

# ============================================================
# MAIN EXECUTION
# ============================================================

Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host "         HE THONG PUBLISH & DEPLOY TU DONG BVTL-X          " -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  - Host dich   : $FtpHost`:$FtpPort"
Write-Host "  - Thu muc FTP : /$RemoteDir"
Write-Host "  - Moi truong  : $Environment"
Write-Host "  - Che do chay : $Mode"
Write-Host "------------------------------------------------------------`n"

if ($Mode -eq "Status") {
    Write-Host "[*] Dang kiem tra ket noi toi may chu FTP..." -ForegroundColor Yellow
    try {
        $req = [System.Net.FtpWebRequest]::Create("ftp://$FtpHost`:$FtpPort/")
        $req.Credentials = New-Object System.Net.NetworkCredential($FtpUser, $FtpPass)
        $req.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectoryDetails
        $req.UsePassive = $true
        $req.Timeout = 8000
        $resp = $req.GetResponse()
        $reader = New-Object System.IO.StreamReader($resp.GetResponseStream())
        $list = $reader.ReadToEnd()
        $reader.Close()
        $resp.Close()

        Write-Host "[OK] Ket noi FTP thanh cong!" -ForegroundColor Green
        Write-Host "`nDanh sach thu muc tren FTP Root:" -ForegroundColor Gray
        Write-Host $list
    } catch {
        Write-Host "[X] Loi ket noi: $($_.Exception.Message)" -ForegroundColor Red
    }
    return
}

if ($Mode -eq "Package") {
    if (-not (Test-Path $PackagePath)) {
        throw "Khong tim thay file goi nen release tai: $PackagePath"
    }
    $pkgInfo = Get-Item $PackagePath
    $pkgSizeMb = [Math]::Round($pkgInfo.Length / 1MB, 2)
    Write-Host "[*] Dang upload goi nen release [$($pkgInfo.Name)] ($pkgSizeMb MB) vao Packages/..." -ForegroundColor Yellow

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    Upload-SingleFile -LocalFilePath $PackagePath -RelativeFilePath "Packages/$($pkgInfo.Name)"
    $stopwatch.Stop()

    Write-Host "[OK] Da upload goi nen release thanh cong vao Packages/! ($([Math]::Round($stopwatch.Elapsed.TotalSeconds, 1))s)" -ForegroundColor Green
    return
}

# Xac dinh danh sach file can deploy
if (-not (Test-Path $SourceDir)) {
    throw "Khong tim thay thu muc nguon: $SourceDir. Hay build publish truoc!"
}

$allFiles = Get-ChildItem -Path $SourceDir -Recurse -File
$filesToDeploy = @()

if ($Mode -eq "Patch") {
    Write-Host "[*] Dang loc cac file cap nhat (tu sau ngay $($SinceDate.ToString('dd/MM/yyyy')))..." -ForegroundColor Yellow
    $filesToDeploy = $allFiles | Where-Object {
        $_.LastWriteTime -gt $SinceDate
    }
} elseif ($Mode -eq "Full") {
    Write-Host "[*] Che do FULL: Chuan bi day toan bo tap tin ($($allFiles.Count) files)..." -ForegroundColor Yellow
    $filesToDeploy = $allFiles
}

# Loc bo cac config mac dinh o thu muc goc de tranh ghi de nham (nhung luon giu Views\Web.config)
$filesToDeploy = $filesToDeploy | Where-Object {
    if ($_.FullName -like "*\Views\Web.config") { return $true }
    $_.Name -notin @("connectionStrings.config", "appSettings.config", "Web.config")
}

# Luon dam bao version.json duoc upload len host de kiem chung phien ban
$verFile = Get-Item (Join-Path $SourceDir "version.json") -ErrorAction SilentlyContinue
if ($verFile -and ($filesToDeploy.FullName -notcontains $verFile.FullName)) {
    $filesToDeploy = @($verFile) + $filesToDeploy
}

if ($filesToDeploy.Count -eq 0 -and -not $IncludeConfig) {
    Write-Host "[INFO] Khong co file nao can deploy!" -ForegroundColor Green
    return
}

# Sap xep do uu tien upload: bin, root, Views, app, Scripts, Report, assets
$filesToDeploy = $filesToDeploy | Sort-Object {
    if ($_.FullName -like "*\bin\*") { 1 }
    elseif ($_.DirectoryName -eq (Resolve-Path $SourceDir).Path) { 2 }
    elseif ($_.FullName -like "*\Views\*") { 3 }
    elseif ($_.FullName -like "*\app\*") { 4 }
    elseif ($_.FullName -like "*\Scripts\*") { 5 }
    elseif ($_.FullName -like "*\Report\*") { 6 }
    elseif ($_.FullName -like "*\App_Assets\*") { 7 }
    else { 8 }
}

Write-Host "-> Tim thay $($filesToDeploy.Count) file can day len host ($Environment):`n" -ForegroundColor Cyan

$sourceRootLen = (Resolve-Path $SourceDir).Path.Length
$index = 0
$total = $filesToDeploy.Count
$hasBinFiles = $filesToDeploy | Where-Object { $_.FullName -like "*\bin\*" -or $_.Extension -eq ".dll" }

$offlineUploaded = $false
if ($hasBinFiles) {
    try {
        Write-Host "[*] Phat hien co cap nhat DLL trong bin/. Dang bat che do bao tri tam thoi (app_offline.htm) cho $Environment..." -ForegroundColor Yellow
        $offlineBytes = [System.Text.Encoding]::UTF8.GetBytes("<!DOCTYPE html><html><body><h1>He thong BVTL-X ($Environment) dang cap nhat phien ban moi trong giay lat...</h1></body></html>")
        $offlineReq = [System.Net.FtpWebRequest]::Create("$(Get-FtpBaseUrl)/app_offline.htm")
        $offlineReq.Credentials = New-Object System.Net.NetworkCredential($FtpUser, $FtpPass)
        $offlineReq.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
        $offlineReq.UsePassive = $true
        $offlineReq.KeepAlive = $false
        $s = $offlineReq.GetRequestStream()
        $s.Write($offlineBytes, 0, $offlineBytes.Length)
        $s.Close()
        $offlineResp = $offlineReq.GetResponse()
        $offlineResp.Close()
        $offlineUploaded = $true
        Start-Sleep -Seconds 2
        Write-Host "    -> IIS da giai phong file lock trong bin/. Bat dau upload!" -ForegroundColor Cyan
    } catch {
        Write-Host "    [!] Khong the tao app_offline.htm: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

try {
    foreach ($file in $filesToDeploy) {
        $index++
        $relPath = $file.FullName.Substring($sourceRootLen).TrimStart("\")
        $sizeKb = [Math]::Round($file.Length / 1KB, 1)

        $percent = [Math]::Round(($index / $total) * 100)
        Write-Host "[$index/$total] ($percent%) Uploading: $relPath ($sizeKb KB)... " -NoNewline -ForegroundColor Gray

        Upload-SingleFile -LocalFilePath $file.FullName -RelativeFilePath $relPath
        Write-Host "OK" -ForegroundColor Green
    }

    # Upload Config rieng biet theo moi truong (neu duoc yeu cau hoac la Full deploy)
    if ($IncludeConfig -or $Mode -eq "Full") {
        $envConfigDir = Join-Path $PSScriptRoot "Config\$Environment"
        if (Test-Path $envConfigDir) {
            Write-Host "`n[*] Dang upload cau hinh dac thu cho moi truong [$Environment]..." -ForegroundColor Yellow
            $configFiles = Get-ChildItem -Path $envConfigDir -File
            foreach ($cfg in $configFiles) {
                Write-Host " -> Uploading config: $($cfg.Name)... " -NoNewline -ForegroundColor Cyan
                Upload-SingleFile -LocalFilePath $cfg.FullName -RelativeFilePath $cfg.Name
                Write-Host "OK" -ForegroundColor Green
            }
        }
    }
} finally {
    if ($offlineUploaded) {
        try {
            Write-Host "[*] Dang go bo app_offline.htm de mo lai website $Environment..." -ForegroundColor Yellow
            $delReq = [System.Net.FtpWebRequest]::Create("$(Get-FtpBaseUrl)/app_offline.htm")
            $delReq.Credentials = New-Object System.Net.NetworkCredential($FtpUser, $FtpPass)
            $delReq.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile
            $delReq.UsePassive = $true
            $delReq.KeepAlive = $false
            $delResp = $delReq.GetResponse()
            $delResp.Close()
            Write-Host "    -> Website da san sang phuc vu!" -ForegroundColor Green
        } catch {
            Write-Host "    [!] Khong the xoa app_offline.htm: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
}

$stopwatch.Stop()
$elapsedSec = [Math]::Round($stopwatch.Elapsed.TotalSeconds, 1)

Write-Host "`n[*] Dang gui yeu cau kiem tra suc khoe website (Healthcheck - $Environment)..." -ForegroundColor Yellow
try {
    if ($Environment -eq "Prod") {
        $health = Invoke-WebRequest -Uri "http://$FtpHost/Login/Index" -Headers @{ Host = $Domain } -UseBasicParsing -TimeoutSec 10
        if ($health.StatusCode -eq 200) {
            Write-Host "    -> [XAC NHAN] Website PROD ($Domain) hoat dong on dinh (HTTP 200 OK)!" -ForegroundColor Green
        } else {
            Write-Host "    [!] Canh bao: Website phan hoi ma: $($health.StatusCode)" -ForegroundColor Yellow
        }
    } else {
        $health = Invoke-WebRequest -Uri "http://$FtpHost`:8090/Login/Index" -UseBasicParsing -TimeoutSec 10
        if ($health.StatusCode -eq 200) {
            Write-Host "    -> [XAC NHAN] Website DEV (Port 8090) hoat dong on dinh (HTTP 200 OK)!" -ForegroundColor Green
        } else {
            Write-Host "    [!] Canh bao: Website phan hoi ma: $($health.StatusCode)" -ForegroundColor Yellow
        }
    }
} catch {
    Write-Host "    [!] Canh bao kiem tra website: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host "`n============================================================" -ForegroundColor Green
Write-Host " [THANH CONG] Da deploy xong [$Environment] trong $elapsedSec giay!" -ForegroundColor Green
Write-Host "============================================================`n"
