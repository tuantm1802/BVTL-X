<#
.SYNOPSIS
    Script quan ly version, dieu phoi ban va va tu dong hoa publish/deploy len host BVTL-X.
.DESCRIPTION
    Cung cap cac lenh:
      - status         : Xem trang thai nhanh, commit, tag va working tree cua ca 2 repo.
      - list-tags      : Xem danh sach cac tag phien ban da danh dau.
      - new-release    : Tao phien ban phat hanh moi (tag Git chuan SemVer).
      - list-patches   : Liet ke cac ban va .patch do AI sinh ra.
      - apply-patch    : Ap dung ban va .patch vao ma nguon.
      - publish        : Bien dich Release va dong goi WebApp vao D:\Deploy.
      - deploy         : Tu dong day cac file ban va (Patch) len may chu IIS qua FTP (3-10s).
      - deploy-full    : Day toan bo web len host qua FTP.
      - deploy-package : Day file nen release (.rar) len may chu.
.EXAMPLE
    .\manage-version.ps1 status
    .\manage-version.ps1 publish
    .\manage-version.ps1 deploy
    .\manage-version.ps1 deploy-package
    .\manage-version.ps1 new-release -Version "v1.1.0" -Message "Phat hanh phan he CD45"
#>

[CmdletBinding()]
param (
    [Parameter(Position = 0)]
    [ValidateSet("status", "list-tags", "new-release", "list-patches", "apply-patch", "publish", "deploy", "deploy-full", "deploy-package")]
    [string]$Command = "status",

    [Parameter()]
    [string]$Version,

    [Parameter()]
    [string]$Message,

    [Parameter()]
    [string]$PatchFile
)

$ErrorActionPreference = "Stop"
$BvtlRoot = $PSScriptRoot
$AgentWorkspaceRoot = Join-Path $BvtlRoot "..\BVTL-X-agent-workspace"
$DeployScript = Join-Path $BvtlRoot "deploy-ftp.ps1"

function Show-Status {
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host "         TRANG THAI PHIEN BAN HE THONG BVTL-X              " -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan

    # Status BVTL-X
    Write-Host "`n[1] REPO BVTL-X (Core Application):" -ForegroundColor Yellow
    Push-Location $BvtlRoot
    try {
        $branch = (git rev-parse --abbrev-ref HEAD).Trim()
        $commit = (git rev-parse --short HEAD).Trim()
        $latestTag = (git describe --tags --abbrev=0 2>$null)
        if (-not $latestTag) { $latestTag = "Chua co tag" }
        $dirtyFiles = (git status -s | Measure-Object).Count

        Write-Host "    - Thu muc       : $BvtlRoot"
        Write-Host "    - Nhanh hien tai: $branch" -ForegroundColor Green
        Write-Host "    - Commit Hash   : $commit"
        Write-Host "    - Tag moi nhat  : $latestTag" -ForegroundColor Magenta
        if ($dirtyFiles -gt 0) {
            Write-Host "    - Working Tree  : Co $dirtyFiles file chua commit (DANG LAM DO)" -ForegroundColor Yellow
        } else {
            Write-Host "    - Working Tree  : Clean (Sach se)" -ForegroundColor Gray
        }
    } finally {
        Pop-Location
    }

    # Status Agent Workspace
    Write-Host "`n[2] REPO AGENT WORKSPACE (Multi-Agent Framework):" -ForegroundColor Yellow
    if (Test-Path (Join-Path $AgentWorkspaceRoot ".git")) {
        Push-Location $AgentWorkspaceRoot
        try {
            $wsBranch = (git rev-parse --abbrev-ref HEAD).Trim()
            $wsCommit = (git rev-parse --short HEAD).Trim()
            $wsTag = (git describe --tags --abbrev=0 2>$null)
            if (-not $wsTag) { $wsTag = "Chua co tag" }

            Write-Host "    - Thu muc       : $AgentWorkspaceRoot"
            Write-Host "    - Nhanh hien tai: $wsBranch" -ForegroundColor Green
            Write-Host "    - Commit Hash   : $wsCommit"
            Write-Host "    - Tag moi nhat  : $wsTag" -ForegroundColor Magenta
        } finally {
            Pop-Location
        }
    } else {
        Write-Host "    - Chua khoi tao git tai: $AgentWorkspaceRoot" -ForegroundColor Gray
    }

    # Status Patches
    Write-Host "`n[3] BAN VA CHO REVIEW (output/patches/):" -ForegroundColor Yellow
    $patchDir = Join-Path $AgentWorkspaceRoot "output\patches"
    if (Test-Path $patchDir) {
        $patches = Get-ChildItem -Path $patchDir -Filter "*.patch"
        if ($patches.Count -gt 0) {
            foreach ($p in $patches) {
                Write-Host "    - $($p.Name) ($([Math]::Round($p.Length / 1KB, 1)) KB, $($p.LastWriteTime.ToString('yyyy-MM-dd HH:mm')))" -ForegroundColor Cyan
            }
        } else {
            Write-Host "    - Khong co ban va .patch nao dang cho." -ForegroundColor Gray
        }
    } else {
        Write-Host "    - Thu muc patch chua duoc tao." -ForegroundColor Gray
    }

    # Status Host
    Write-Host "`n[4] KET NOI MAY CHU HOST (103.77.167.206:21):" -ForegroundColor Yellow
    if (Test-Path $DeployScript) {
        & $DeployScript -Mode Status
    }

    Write-Host "`n============================================================" -ForegroundColor Cyan
}

function Show-Tags {
    Write-Host "=== DANH SACH TAGS (BVTL-X) ===" -ForegroundColor Cyan
    Push-Location $BvtlRoot
    try {
        git tag -n --sort=-creatordate
    } finally {
        Pop-Location
    }
}

function New-ReleaseVersion {
    if (-not $Version) {
        Write-Host "Loi: Vui long truyen tham so -Version (Vi du: -Version v1.1.0)" -ForegroundColor Red
        return
    }

    if (-not $Message) {
        $Message = "Release $Version"
    }

    Push-Location $BvtlRoot
    try {
        Write-Host "Dang tao Git Tag: $Version voi noi dung: $Message" -ForegroundColor Cyan
        git tag -a $Version -m $Message
        Write-Host "Tao tag thanh cong!" -ForegroundColor Green
    } finally {
        Pop-Location
    }
}

function List-Patches {
    $patchDir = Join-Path $AgentWorkspaceRoot "output\patches"
    if (-not (Test-Path $patchDir)) {
        Write-Host "Thu muc patch chua ton tai: $patchDir" -ForegroundColor Yellow
        return
    }

    $patches = Get-ChildItem -Path $patchDir -Filter "*.patch"
    Write-Host "=== CAC BAN VA HIEN CO TRONG AGENT WORKSPACE ===" -ForegroundColor Cyan
    if ($patches.Count -eq 0) {
        Write-Host "Khong co file patch nao." -ForegroundColor Gray
        return
    }

    foreach ($p in $patches) {
        Write-Host " - Tệp: $($p.FullName)" -ForegroundColor Green
        Write-Host "   Kích thước: $([Math]::Round($p.Length / 1KB, 1)) KB | Thời gian: $($p.LastWriteTime)"
    }
}

function Apply-PatchFile {
    if (-not $PatchFile) {
        Write-Host "Loi: Vui long truyen tham so -PatchFile." -ForegroundColor Red
        return
    }

    $fullPath = if ([System.IO.Path]::IsPathRooted($PatchFile)) {
        $PatchFile
    } else {
        Join-Path (Get-Location) $PatchFile
    }

    if (-not (Test-Path $fullPath)) {
        Write-Host "Loi: Khong tim thay file patch tai: $fullPath" -ForegroundColor Red
        return
    }

    Push-Location $BvtlRoot
    try {
        Write-Host "Kiem tra tinh hop le cua patch: $fullPath ..." -ForegroundColor Cyan
        $check = git apply --check $fullPath 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Loi: Patch bi xung dot hoac khong the apply truc tiep!`n$check" -ForegroundColor Red
            return
        }

        Write-Host "Patch hop le! Dang ap dung patch vao working directory..." -ForegroundColor Cyan
        git apply $fullPath
        Write-Host "Da ap dung patch thanh cong! Hay kiem tra lai tren Visual Studio 2022." -ForegroundColor Green
    } finally {
        Pop-Location
    }
}

function Invoke-PublishApp {
    Write-Host "=== DONG GOI PUBLISH BVTL-X ===" -ForegroundColor Cyan
    $msBuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    $projPath = Join-Path $BvtlRoot "WebApp\WebApp.csproj"
    $pubProfile = Join-Path $BvtlRoot "WebApp\Properties\PublishProfiles\FolderProfile1.pubxml"
    $deployFolder = "D:\Deploy\WebApp_Publish"

    Write-Host "[1/2] Dang bien dich va publish bang MSBuild..." -ForegroundColor Yellow
    & $msBuild $projPath /p:DeployOnBuild=true /p:PublishProfile=$pubProfile /p:Configuration=Release /verbosity:minimal

    if ($LASTEXITCODE -ne 0) {
        throw "Loi bien dich MSBuild khi publish!"
    }

    Write-Host "[2/2] Dang nen file goi release .rar..." -ForegroundColor Yellow
    $rarExe = "C:\Program Files\WinRAR\Rar.exe"
    if (Test-Path $rarExe) {
        $rarTarget = "D:\Deploy\BVTL_WebApp_Publish_v1.1.0.rar"
        & $rarExe a -r -y $rarTarget "$deployFolder\*" | Out-Null
        Write-Host "[OK] Da tao file nen release: $rarTarget" -ForegroundColor Green
    }

    Write-Host "`n[THANH CONG] Dong goi publish hoan tat tai: $deployFolder" -ForegroundColor Green
}

function Invoke-DeployApp {
    param ([string]$DeployMode = "Patch")

    if (-not (Test-Path $DeployScript)) {
        throw "Khong tim thay script: $DeployScript"
    }

    & $DeployScript -Mode $DeployMode
}

switch ($Command) {
    "status"         { Show-Status }
    "list-tags"      { Show-Tags }
    "new-release"    { New-ReleaseVersion }
    "list-patches"   { List-Patches }
    "apply-patch"    { Apply-PatchFile }
    "publish"        { Invoke-PublishApp }
    "deploy"         { Invoke-DeployApp -DeployMode "Patch" }
    "deploy-full"    { Invoke-DeployApp -DeployMode "Full" }
    "deploy-package" { Invoke-DeployApp -DeployMode "Package" }
}
