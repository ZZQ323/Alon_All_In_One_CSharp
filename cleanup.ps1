# TKCompass 完整清理脚本
# 用于解决重复 App 类和编译缓存问题

Write-Host "=== TKCompass 完整清理脚本 ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: 检查并删除旧项目目录
Write-Host "[1/5] 检查旧项目目录..." -ForegroundColor Yellow
if (Test-Path "TKCompass") {
    Write-Host "  发现旧项目目录 TKCompass/，正在删除..." -ForegroundColor Red
    Remove-Item -Recurse -Force "TKCompass"
    Write-Host "  ✓ 已删除" -ForegroundColor Green
} else {
    Write-Host "  ✓ 旧项目目录不存在（已删除）" -ForegroundColor Green
}

# Step 2: 清理所有 obj 和 bin 目录
Write-Host ""
Write-Host "[2/5] 清理所有 obj/bin 缓存..." -ForegroundColor Yellow
$objBinDirs = Get-ChildItem -Path . -Recurse -Directory -Include obj,bin -ErrorAction SilentlyContinue
if ($objBinDirs) {
    foreach ($dir in $objBinDirs) {
        Write-Host "  删除: $($dir.FullName)" -ForegroundColor Gray
        Remove-Item -Recurse -Force $dir.FullName -ErrorAction SilentlyContinue
    }
    Write-Host "  ✓ 已清理 $($objBinDirs.Count) 个目录" -ForegroundColor Green
} else {
    Write-Host "  ✓ 没有需要清理的目录" -ForegroundColor Green
}

# Step 3: 清理 .vs 缓存
Write-Host ""
Write-Host "[3/5] 清理 Visual Studio 缓存..." -ForegroundColor Yellow
if (Test-Path ".vs") {
    Write-Host "  删除 .vs 目录..." -ForegroundColor Gray
    Remove-Item -Recurse -Force ".vs" -ErrorAction SilentlyContinue
    Write-Host "  ✓ 已清理" -ForegroundColor Green
} else {
    Write-Host "  ✓ .vs 目录不存在" -ForegroundColor Green
}

# Step 4: 执行 dotnet clean
Write-Host ""
Write-Host "[4/5] 执行 dotnet clean..." -ForegroundColor Yellow
dotnet clean TKCompass.slnx --verbosity quiet
Write-Host "  ✓ 完成" -ForegroundColor Green

# Step 5: 重新构建
Write-Host ""
Write-Host "[5/5] 重新构建解决方案..." -ForegroundColor Yellow
Write-Host ""
dotnet build TKCompass.slnx

Write-Host ""
Write-Host "=== 清理完成 ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "如果仍有错误，请：" -ForegroundColor Yellow
Write-Host "1. 关闭 Visual Studio" -ForegroundColor White
Write-Host "2. 重新打开 TKCompass.slnx" -ForegroundColor White
Write-Host "3. 再次执行 '生成 -> 重新生成解决方案'" -ForegroundColor White
