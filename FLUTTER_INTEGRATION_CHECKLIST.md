# Unity 到 Flutter 集成 - 快速检查清单

## ✅ 准备阶段

### Unity 准备
- [ ] Unity 6 已安装
- [ ] 项目可以正常运行（播放按钮测试）
- [ ] 场景文件在 `Assets/Scenes/SampleScene.unity`

### Flutter 准备
- [ ] Flutter SDK 已安装（运行 `flutter doctor` 检查）
- [ ] Android Studio / Xcode 已安装（取决于目标平台）
- [ ] 命令行可以访问 `flutter` 命令

---

## 📦 第一步：Unity 项目设置（预计 10 分钟）

### 1. 导入 Flutter Unity Integration 包
```
⏱️ 时间: 2 分钟

步骤:
1. 打开 Unity 项目
2. Assets > Import Package > Custom Package
3. 选择: Assets/FlutterAssert/unitypackages/fuw-6000.0.2.unitypackage
4. 点击 Import All
5. 等待导入完成

验证:
- Assets 目录下出现 FlutterUnityIntegration 文件夹
- 没有编译错误
```

- [ ] ✅ 包已导入
- [ ] ✅ 无编译错误

### 2. 配置 Unity Player Settings
```
⏱️ 时间: 3 分钟

步骤:
1. Edit > Project Settings > Player

Android 设置:
- Minimum API Level: 22
- Target API Level: 33
- Scripting Backend: IL2CPP
- Target Architectures: ARM64 ✓, ARMv7 ✓
- Graphics APIs: OpenGLES3

iOS 设置:
- Minimum iOS Version: 12.0
- Architecture: ARM64

其他设置:
- Scripting Define Symbols: FLUTTER_UNITY_WIDGET
- Strip Engine Code: 关闭
```

- [ ] ✅ Android 设置完成
- [ ] ✅ iOS 设置完成（如果需要）
- [ ] ✅ 其他设置完成

### 3. 添加 Flutter 通信脚本
```
⏱️ 时间: 2 分钟

步骤:
1. FlutterCommunication.cs 已在 Assets/Scripts/ 中
2. 打开场景: Assets/Scenes/SampleScene.unity
3. 创建空 GameObject，命名为 "FlutterBridge"
4. 添加 FlutterCommunication 组件到 FlutterBridge
5. 保存场景 (Ctrl+S)
```

- [ ] ✅ FlutterBridge GameObject 已创建
- [ ] ✅ FlutterCommunication 脚本已附加
- [ ] ✅ 场景已保存

### 4. 将场景添加到 Build Settings
```
⏱️ 时间: 1 分钟

步骤:
1. File > Build Settings
2. 确认 SampleScene 在 "Scenes In Build" 列表中
3. 如果没有，点击 "Add Open Scenes"
```

- [ ] ✅ 场景已添加到 Build Settings

### 5. 导出 Unity 项目
```
⏱️ 时间: 2-5 分钟（取决于项目大小）

Android:
1. Flutter > Export Android
2. 选择导出路径: F:\Unity\UnityExport\Android
3. 选择 "Release" 构建
4. 点击 Export
5. 等待完成

iOS（Mac 上）:
1. Flutter > Export iOS
2. 选择导出路径: ~/UnityExport/iOS
3. 点击 Export
4. 等待完成

验证:
- 导出目录包含 unityLibrary 文件夹
- 控制台显示 "Export completed successfully"
```

- [ ] ✅ Android 导出完成
- [ ] ✅ iOS 导出完成（如果需要）

---

## 🎨 第二步：创建 Flutter 项目（预计 5 分钟）

### 1. 创建 Flutter 项目
```
⏱️ 时间: 2 分钟

命令:
cd F:\FlutterProjects  # 或您喜欢的目录
flutter create star_falling_flutter
cd star_falling_flutter
```

- [ ] ✅ Flutter 项目已创建

### 2. 添加依赖
```
⏱️ 时间: 1 分钟

步骤:
1. 打开 pubspec.yaml
2. 在 dependencies 下添加:
   flutter_unity_widget: ^2022.2.0
3. 保存文件
4. 运行: flutter pub get
```

- [ ] ✅ 依赖已添加
- [ ] ✅ flutter pub get 成功

### 3. 复制示例代码
```
⏱️ 时间: 1 分钟

步骤:
1. 复制 FlutterExample/main.dart
2. 替换 Flutter 项目的 lib/main.dart
```

- [ ] ✅ main.dart 已替换

---

## 🔗 第三步：集成 Unity 到 Flutter（预计 10 分钟）

### Android 集成

#### 1. 复制 Unity 导出文件
```
⏱️ 时间: 1 分钟

步骤:
1. 复制 F:\Unity\UnityExport\Android\unityLibrary
2. 粘贴到 F:\FlutterProjects\star_falling_flutter\android\unityLibrary
```

- [ ] ✅ unityLibrary 已复制

#### 2. 修改 settings.gradle
```
⏱️ 时间: 2 分钟

文件: android/settings.gradle

在文件末尾添加:
include ':unityLibrary'
project(':unityLibrary').projectDir = file('./unityLibrary')
```

- [ ] ✅ settings.gradle 已修改

#### 3. 修改 app/build.gradle
```
⏱️ 时间: 2 分钟

文件: android/app/build.gradle

在 dependencies 块中添加:
implementation project(':unityLibrary')
```

- [ ] ✅ app/build.gradle 已修改

#### 4. 修改 AndroidManifest.xml
```
⏱️ 时间: 3 分钟

文件: android/app/src/main/AndroidManifest.xml

在 <application> 标签内添加 Unity Activity
（参考 FLUTTER_INTEGRATION_GUIDE.md）
```

- [ ] ✅ AndroidManifest.xml 已修改

### iOS 集成（Mac 上）

#### 1. 复制 Unity 导出文件
```
⏱️ 时间: 1 分钟

步骤:
1. 复制 ~/UnityExport/iOS/UnityFramework
2. 粘贴到 ios/UnityFramework/
```

- [ ] ✅ UnityFramework 已复制

#### 2. 在 Xcode 中配置
```
⏱️ 时间: 5 分钟

步骤:
1. 打开 ios/Runner.xcworkspace
2. 添加 UnityFramework.xcodeproj
3. 在 Frameworks 中添加 UnityFramework.framework
4. 设置为 "Embed & Sign"
```

- [ ] ✅ Xcode 配置完成

#### 3. 修改 Info.plist
```
⏱️ 时间: 1 分钟

添加:
<key>io.flutter.embedded_views_preview</key>
<true/>
```

- [ ] ✅ Info.plist 已修改

---

## 🏃 第四步：测试运行（预计 5 分钟）

### 1. 连接设备或启动模拟器
```
⏱️ 时间: 1 分钟

命令:
flutter devices

确认有可用设备
```

- [ ] ✅ 设备已连接

### 2. 运行 Flutter 应用
```
⏱️ 时间: 3-5 分钟（首次运行）

命令:
flutter run

等待编译和安装
```

- [ ] ✅ 应用已启动
- [ ] ✅ 无编译错误

### 3. 测试功能
```
⏱️ 时间: 2 分钟

测试项目:
- Unity 场景显示正常
- 可以看到星星动画
- "开始动画" 按钮有响应
- 价格更新功能正常
- 暂停/恢复功能正常
```

- [ ] ✅ Unity 场景显示
- [ ] ✅ 动画播放正常
- [ ] ✅ 按钮控制正常
- [ ] ✅ 消息通信正常

---

## 🐛 故障排查清单

### Unity 场景黑屏
- [ ] 检查场景是否在 Build Settings 中
- [ ] 检查 Player Settings 是否正确配置
- [ ] 重新导出 Unity 项目
- [ ] 查看 Unity 日志（Logcat / Xcode Console）

### 编译错误
- [ ] 运行 `flutter clean`
- [ ] 运行 `flutter pub get`
- [ ] 检查所有配置文件是否正确修改
- [ ] 查看错误消息，搜索解决方案

### 消息无法通信
- [ ] 检查 FlutterBridge GameObject 名称
- [ ] 检查 FlutterCommunication 脚本是否附加
- [ ] 查看 Flutter 和 Unity 日志
- [ ] 确认 Flutter 调用的方法名正确

### Android 特定问题
- [ ] 检查 Gradle 版本兼容性
- [ ] 确认 Android SDK 和 NDK 已安装
- [ ] 运行 `cd android && ./gradlew clean`

### iOS 特定问题
- [ ] 清理 Xcode 构建缓存
- [ ] 检查证书和配置文件
- [ ] 确认 UnityFramework 正确嵌入

---

## ✨ 完成！

恭喜！如果所有检查项都通过了，您的 Unity 项目已成功集成到 Flutter 中！

### 下一步

- [ ] 优化性能（降低场景复杂度）
- [ ] 添加更多交互功能
- [ ] 美化 Flutter UI
- [ ] 准备发布版本

---

## 📞 需要帮助？

如果遇到问题：

1. 查看详细指南: `FLUTTER_INTEGRATION_GUIDE.md`
2. 查看 Unity 控制台日志
3. 运行 `flutter logs` 查看 Flutter 日志
4. 检查 GitHub flutter_unity_widget 的 Issues

---

## ⏱️ 总时间估计

- Unity 设置: 10 分钟
- Flutter 创建: 5 分钟
- 集成配置: 10 分钟
- 测试运行: 5 分钟

**总计: 约 30 分钟**（首次集成）

熟练后可以在 10-15 分钟内完成！
