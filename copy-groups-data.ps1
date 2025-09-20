#!/usr/bin/env pwsh
# PowerShell script to copy MT5 groups data to debug folders

Write-Host "=== Copying MT5 Groups Data to Debug Folders ===" -ForegroundColor Cyan
Write-Host ""

# Define debug directories
$debugDirs = @(
    "MT5ManagerAPI\bin\Debug",
    "MT5WebAPI\bin\Debug", 
    "MT5ConsoleApp\bin\Debug"
)

# Create debug directories if they don't exist
foreach ($dir in $debugDirs) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "Created directory: $dir" -ForegroundColor Yellow
    }
}

# Check if source file exists
$sourceFile = "complete_mt5_groups.json"
if (Test-Path $sourceFile) {
    $fileSize = (Get-Item $sourceFile).Length
    Write-Host "Found source file: $sourceFile ($([math]::Round($fileSize/1KB, 1)) KB)" -ForegroundColor Green
    Write-Host ""
    
    # Copy to all debug directories
    foreach ($dir in $debugDirs) {
        $targetPath = Join-Path $dir "complete_mt5_groups.json"
        try {
            Copy-Item $sourceFile $targetPath -Force
            Write-Host "✓ Copied to: $dir" -ForegroundColor Green
        } catch {
            Write-Host "✗ Failed to copy to: $dir - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "=== Verification ===" -ForegroundColor Cyan
    
    # Verify copies
    foreach ($dir in $debugDirs) {
        $targetPath = Join-Path $dir "complete_mt5_groups.json"
        if (Test-Path $targetPath) {
            $targetSize = (Get-Item $targetPath).Length
            Write-Host "✓ $dir\complete_mt5_groups.json ($([math]::Round($targetSize/1KB, 1)) KB)" -ForegroundColor Green
        } else {
            Write-Host "✗ $dir\complete_mt5_groups.json missing" -ForegroundColor Red
        }
    }
    
} else {
    Write-Host "✗ Source file not found: $sourceFile" -ForegroundColor Red
    Write-Host "Please ensure complete_mt5_groups.json exists in the workspace root" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Copy operation completed ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Build your solution (build.bat)"
Write-Host "2. Run your MT5 Web API"
Write-Host "3. The 187 groups will be automatically loaded on startup"
Write-Host "4. Test with: GET http://localhost:8080/api/groups"