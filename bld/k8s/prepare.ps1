param(
    $SitecoreReleaseVersion = "10.2.0.006766.683",
    $DcrmReleaseVersion = "7.0.0.01499.150",
    $Topology = "xm1",
    $Platform = "ltsc2019",
    $DestinationRoot = (Join-Path $PSScriptRoot "sitecore"),

    [Switch]$SkipDownload,
    [Switch]$SkipCorrections
)

$sitecoreAssetFile = "https://github.com/Sitecore/container-deployment/releases/download/sxp%2F${SitecoreReleaseVersion}/SitecoreContainerDeployment.${SitecoreReleaseVersion}.zip"
$localZip = "k8s.zip"
$localTmp = "tmp"

$ErrorActionPreference = "STOP"

function DownloadAndExtract {
    param(
        $Uri,
        $FolderInArchive,
        $Destination,

        [Switch]$SkipRemoveTemp

    )

    If (Test-Path $localZip) { Remove-Item $localZip -Force }
    If (Test-Path $localTmp) { Remove-Item $localTmp -Force -Recurse }
    If (Test-Path $Destination) { Remove-Item $Destination -Force -Recurse }
    $parent = Split-Path $Destination -Parent
    If (-not (Test-Path $parent)) { New-Item $parent -ItemType Directory }

    Write-Host "Downloading Sitecore Container Deployment package ${Uri}" -ForegroundColor Green
    Invoke-WebRequest -Uri $Uri -OutFile $localZip
    Write-Host "Extracting" -ForegroundColor Green
    Expand-Archive $localZip -DestinationPath $localTmp
    Remove-Item $localZip

    Move-Item (Join-Path $localTmp $FolderInArchive) $Destination -Verbose
    If (-not $SkipRemoveTemp) {
        Remove-Item $localTmp -Force -Recurse
    }

    Write-Host "Extracting done" -ForegroundColor Green
}

function RemoveNewNameTags {
    param(
        $Path
    )

    Write-Host "Corrections to ${Path} (remove new image spec)..." -ForegroundColor Green
    $content = get-content $Path -Raw
    $content = $content -replace "images:`n(- name: .+`n  newName: (.+)`n  newTag: (.+)`n)+", ""
    Set-Content -Path ${Path} -Value $content -Encoding UTF8
}

function RemoveTlsSecretEntries {
    param(
        $Path
    )

    Write-Host "Corrections to secrets ${Path} (remove tls config)..." -ForegroundColor Green
    $content = get-content $Path -Raw
    $content = $content -replace "- name: ([a-z\-]+)\-tls`n  files:`n(    - tls/.*`n)+  type: kubernetes.io/tls", ""
    Set-Content -Path "${Path}" -Value $content -Encoding UTF8
}

function RemoveBases {
    param(
        $Path
    )

    $content = get-content $Path -Raw
    $content = $content -replace "bases:`n(- .+`n)+", ""
    Set-Content -Path "${Path}" -Value $content -Encoding UTF8
}

function GenerateKustomizationFileForFolder {
    param(
        $Path
    )

    if (-not (Test-Path $Path -PathType Container)) {
        Write-Error "Cannot create Kustomization file as ${Path} is not folder"
        return
    }

    $resultFile = Join-Path $Path "kustomization.yaml"
    If (Test-Path $resultFile) {
        Write-Warning "Kustomization file $resultFile already exists"
        return        
    }

    $files = Get-ChildItem -Path $Path -Filter "*.yaml"
    Write-Host ("Create {0} for {1} files" -f $resultFile, $files.Length) -ForegroundColor Green
    $content = "apiVersion: kustomize.config.k8s.io/v1beta1`nkind: Kustomization`n`nresources:`n"
    $files | ForEach-Object { $content += ("- {0}`n" -f $_.Name) }
    $content | Out-File -FilePath $resultFile -Encoding utf8 -Force
}


$sitecoreDestination = "${DestinationRoot}/${Platform}/${Topology}"

if (-not $SkipDownload) {

    DownloadAndExtract -Uri $sitecoreAssetFile -FolderInArchive "k8s/${Platform}/${Topology}" -Destination $sitecoreDestination
}

if (-not $SkipCorrections) {
    Write-Host "Corrections to packages..." -ForegroundColor Green

    @(
        (Resolve-Path $sitecoreDestination\kustomization.yaml),
        (Resolve-Path $sitecoreDestination\external\kustomization.yaml)
        (Resolve-Path $sitecoreDestination\init\kustomization.yaml)
    ) | ForEach-Object {
        RemoveNewNameTags -Path $_
    }

    RemoveBases -Path (Resolve-path $sitecoreDestination\kustomization.yaml)
    RemoveTlsSecretEntries -Path (Resolve-Path $sitecoreDestination\secrets\kustomization.yaml)
    GenerateKustomizationFileForFolder -Path (Resolve-Path ${sitecoreDestination}\volumes\azurefile)
}

Write-Host "Done!" -ForegroundColor Green
