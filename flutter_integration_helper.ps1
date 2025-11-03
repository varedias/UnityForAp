# Unity 到 Flutter 集成助手脚本
# 使用方法: 在 PowerShell 中运行此脚本

param(
    [Parameter(Mandatory=$false)]
    [string]$FlutterProjectPath = "",
    
    [Parameter(Mandatory=$false)]
    [string]$UnityExportPath = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Unity to Flutter Integration Helper  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查 Flutter 是否安装
Write-Host "[1/6] 检查 Flutter 环境..." -ForegroundColor Yellow
try {
    $flutterVersion = flutter --version 2>&1 | Select-String "Flutter" | Select-Object -First 1
    Write-Host "✓ Flutter 已安装: $flutterVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Flutter 未安装或未添加到 PATH" -ForegroundColor Red
    Write-Host "请先安装 Flutter SDK: https://flutter.dev/docs/get-started/install" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# 询问或验证 Flutter 项目路径
if ($FlutterProjectPath -eq "") {
    $defaultPath = "F:\FlutterProjects\star_falling_flutter"
    $FlutterProjectPath = Read-Host "请输入 Flutter 项目路径 (按 Enter 使用默认: $defaultPath)"
    if ($FlutterProjectPath -eq "") {
        $FlutterProjectPath = $defaultPath
    }
}

Write-Host "[2/6] 检查 Flutter 项目..." -ForegroundColor Yellow
if (Test-Path $FlutterProjectPath) {
    Write-Host "✓ Flutter 项目存在: $FlutterProjectPath" -ForegroundColor Green
} else {
    Write-Host "Flutter 项目不存在，正在创建..." -ForegroundColor Yellow
    
    $projectName = Split-Path -Leaf $FlutterProjectPath
    $parentPath = Split-Path -Parent $FlutterProjectPath
    
    # 创建父目录（如果不存在）
    if (-not (Test-Path $parentPath)) {
        New-Item -ItemType Directory -Path $parentPath -Force | Out-Null
    }
    
    # 创建 Flutter 项目
    Push-Location $parentPath
    flutter create $projectName
    Pop-Location
    
    if (Test-Path $FlutterProjectPath) {
        Write-Host "✓ Flutter 项目已创建" -ForegroundColor Green
    } else {
        Write-Host "✗ 创建 Flutter 项目失败" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""

# 询问 Unity 导出路径
if ($UnityExportPath -eq "") {
    $defaultExportPath = "F:\Unity\UnityExport\Android"
    $UnityExportPath = Read-Host "请输入 Unity 导出路径 (按 Enter 使用默认: $defaultExportPath)"
    if ($UnityExportPath -eq "") {
        $UnityExportPath = $defaultExportPath
    }
}

Write-Host "[3/6] 检查 Unity 导出文件..." -ForegroundColor Yellow
$unityLibraryPath = Join-Path $UnityExportPath "unityLibrary"
if (Test-Path $unityLibraryPath) {
    Write-Host "✓ Unity 导出文件存在: $unityLibraryPath" -ForegroundColor Green
} else {
    Write-Host "✗ Unity 导出文件不存在: $unityLibraryPath" -ForegroundColor Red
    Write-Host "请先在 Unity 中导出项目:" -ForegroundColor Yellow
    Write-Host "  1. 在 Unity 中打开项目" -ForegroundColor Yellow
    Write-Host "  2. Flutter > Export Android" -ForegroundColor Yellow
    Write-Host "  3. 选择导出路径: $UnityExportPath" -ForegroundColor Yellow
    Write-Host "  4. 等待导出完成" -ForegroundColor Yellow
    Write-Host "  5. 重新运行此脚本" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# 复制 Unity 导出文件到 Flutter 项目
Write-Host "[4/6] 复制 Unity 文件到 Flutter 项目..." -ForegroundColor Yellow
$targetPath = Join-Path $FlutterProjectPath "android\unityLibrary"

if (Test-Path $targetPath) {
    Write-Host "unityLibrary 已存在，正在删除旧版本..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $targetPath
}

try {
    Copy-Item -Recurse -Force $unityLibraryPath $targetPath
    Write-Host "✓ Unity 文件已复制到: $targetPath" -ForegroundColor Green
} catch {
    Write-Host "✗ 复制失败: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 复制示例代码
Write-Host "[5/6] 复制示例代码..." -ForegroundColor Yellow
$currentDir = Split-Path -Parent $PSCommandPath
$exampleMainPath = Join-Path $currentDir "FlutterExample\main.dart"
$targetMainPath = Join-Path $FlutterProjectPath "lib\main.dart"

if (Test-Path $exampleMainPath) {
    Copy-Item -Force $exampleMainPath $targetMainPath
    Write-Host "✓ main.dart 已复制" -ForegroundColor Green
} else {
    Write-Host "⚠ 示例 main.dart 未找到，请手动复制" -ForegroundColor Yellow
}

Write-Host ""

# 更新 pubspec.yaml
Write-Host "[6/6] 更新 pubspec.yaml..." -ForegroundColor Yellow
$pubspecPath = Join-Path $FlutterProjectPath "pubspec.yaml"

if (Test-Path $pubspecPath) {
    $pubspecContent = Get-Content $pubspecPath -Raw
    
    # 检查是否已经添加了 flutter_unity_widget
    if ($pubspecContent -notmatch "flutter_unity_widget") {
        Write-Host "正在添加 flutter_unity_widget 依赖..." -ForegroundColor Yellow
        
        # 在 dependencies 部分添加
        $pubspecContent = $pubspecContent -replace "(dependencies:\s*\n\s*flutter:\s*\n\s*sdk:\s*flutter)", "`$1`n  flutter_unity_widget: ^2022.2.0"
        
        Set-Content -Path $pubspecPath -Value $pubspecContent
        Write-Host "✓ flutter_unity_widget 依赖已添加" -ForegroundColor Green
        
        # 运行 flutter pub get
        Write-Host "正在运行 flutter pub get..." -ForegroundColor Yellow
        Push-Location $FlutterProjectPath
        flutter pub get
        Pop-Location
        Write-Host "✓ 依赖已安装" -ForegroundColor Green
    } else {
        Write-Host "✓ flutter_unity_widget 依赖已存在" -ForegroundColor Green
    }
} else {
    Write-Host "✗ pubspec.yaml 未找到" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "         自动化步骤完成！" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 显示后续手动步骤
Write-Host "⚠ 请手动完成以下步骤:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. 修改 android/settings.gradle" -ForegroundColor White
Write-Host "   在文件末尾添加:" -ForegroundColor Gray
Write-Host "   include ':unityLibrary'" -ForegroundColor Gray
Write-Host "   project(':unityLibrary').projectDir = file('./unityLibrary')" -ForegroundColor Gray
Write-Host ""

Write-Host "2. 修改 android/app/build.gradle" -ForegroundColor White
Write-Host "   在 dependencies 块中添加:" -ForegroundColor Gray
Write-Host "   implementation project(':unityLibrary')" -ForegroundColor Gray
Write-Host ""

Write-Host "3. 修改 android/app/src/main/AndroidManifest.xml" -ForegroundColor White
Write-Host "   参考 FLUTTER_INTEGRATION_GUIDE.md 添加 Unity Activity" -ForegroundColor Gray
Write-Host ""

Write-Host "4. 运行应用" -ForegroundColor White
Write-Host "   cd $FlutterProjectPath" -ForegroundColor Gray
Write-Host "   flutter run" -ForegroundColor Gray
Write-Host ""

Write-Host "📚 详细文档:" -ForegroundColor Cyan
Write-Host "   - FLUTTER_INTEGRATION_GUIDE.md (完整指南)" -ForegroundColor White
Write-Host "   - FLUTTER_INTEGRATION_CHECKLIST.md (检查清单)" -ForegroundColor White
Write-Host ""

# 询问是否打开项目目录
$openFolder = Read-Host "是否打开 Flutter 项目目录? (Y/N)"
if ($openFolder -eq "Y" -or $openFolder -eq "y") {
    Start-Process explorer $FlutterProjectPath
}

Write-Host ""
Write-Host "✨ 祝您集成顺利！" -ForegroundColor Green
