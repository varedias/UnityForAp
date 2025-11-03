# Unity 到 Flutter 集成完整指南

## 📦 第一步：在 Unity 中设置

### 1.1 导入 Flutter Unity Integration 包

1. **在 Unity Editor 中打开您的项目**

2. **导入 Unity Package**
   - 方法1（推荐）：
     ```
     Assets > Import Package > Custom Package...
     选择: Assets/FlutterAssert/unitypackages/fuw-6000.0.2.unitypackage
     点击 Import All
     ```
   
   - 如果遇到问题，可以尝试旧版本：
     ```
     Assets/FlutterAssert/unitypackages/fuw-2022.3.2.unitypackage
     ```

3. **导入后会出现以下文件夹**
   ```
   Assets/
   └── FlutterUnityIntegration/
       ├── Editor/
       │   ├── Build.cs
       │   ├── XCodePostBuild.cs
       │   └── 其他编辑器脚本
       ├── JsonDotNet/
       │   └── Assemblies/
       └── Scripts/
           ├── NativeAPI.cs
           └── 其他运行时脚本
   ```

### 1.2 配置 Unity 项目设置

1. **打开 Player Settings**
   ```
   Edit > Project Settings > Player
   ```

2. **配置 Android 设置**（如果需要 Android）
   - Minimum API Level: 22 或更高
   - Target API Level: 33 或更高
   - Scripting Backend: IL2CPP（推荐）
   - Target Architectures: ARM64 ✓, ARMv7 ✓
   - 取消勾选 "Auto Graphics API"
   - Graphics APIs: OpenGLES3

3. **配置 iOS 设置**（如果需要 iOS）
   - Minimum iOS Version: 12.0 或更高
   - Target SDK: Device SDK
   - Architecture: ARM64

4. **其他重要设置**
   ```
   Player Settings > Other Settings:
   - Scripting Define Symbols: 添加 FLUTTER_UNITY_WIDGET
   - Strip Engine Code: 关闭（Disable）
   ```

### 1.3 导出 Unity 项目

1. **打开导出窗口**
   ```
   Flutter > Export Android (或 Export iOS)
   ```
   
   如果菜单没有出现，检查：
   - 确认 FlutterUnityIntegration 包已正确导入
   - 重启 Unity Editor

2. **选择导出路径**
   - Android: 选择一个临时文件夹，例如 `F:\Unity\UnityExport\Android`
   - iOS: 选择一个临时文件夹，例如 `F:\Unity\UnityExport\iOS`

3. **点击 Export**
   - 等待导出完成
   - 导出完成后会生成 `unityLibrary` 文件夹

---

## 🎨 第二步：创建或配置 Flutter 项目

### 2.1 创建新的 Flutter 项目

打开终端/命令提示符：

```bash
# 创建 Flutter 项目
flutter create star_falling_flutter

# 进入项目目录
cd star_falling_flutter
```

### 2.2 添加 flutter_unity_widget 依赖

编辑 `pubspec.yaml`：

```yaml
dependencies:
  flutter:
    sdk: flutter
  flutter_unity_widget: ^2022.2.0  # 使用最新稳定版本

dev_dependencies:
  flutter_test:
    sdk: flutter
```

然后运行：
```bash
flutter pub get
```

---

## 🔗 第三步：集成 Unity 导出文件到 Flutter

### 3.1 Android 集成

1. **复制 Unity 导出的文件**
   
   将 Unity 导出的 `unityLibrary` 文件夹复制到：
   ```
   star_falling_flutter/android/unityLibrary/
   ```

2. **修改 android/settings.gradle**
   
   添加以下内容：
   ```gradle
   include ':unityLibrary'
   project(':unityLibrary').projectDir = file('./unityLibrary')
   ```

3. **修改 android/app/build.gradle**
   
   在 `dependencies` 块中添加：
   ```gradle
   dependencies {
       implementation project(':unityLibrary')
       // ... 其他依赖
   }
   ```

4. **修改 AndroidManifest.xml**
   
   在 `android/app/src/main/AndroidManifest.xml` 中：
   ```xml
   <application
       android:name="${applicationName}"
       android:label="star_falling_flutter"
       android:icon="@mipmap/ic_launcher">
       
       <!-- 添加 Unity Activity -->
       <activity
           android:name="com.xraph.plugin.flutter_unity_widget.OverrideUnityActivity"
           android:theme="@style/UnityThemeSelector"
           android:screenOrientation="fullSensor"
           android:launchMode="singleTask"
           android:configChanges="mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density"
           android:hardwareAccelerated="false"
           android:process=":Unity">
       </activity>
   </application>
   ```

### 3.2 iOS 集成

1. **复制 Unity 导出的文件**
   
   将 Unity 导出的 `UnityFramework` 文件夹复制到：
   ```
   star_falling_flutter/ios/UnityFramework/
   ```

2. **在 Xcode 中配置**
   
   - 打开 `ios/Runner.xcworkspace`
   - 右键点击项目 > Add Files to "Runner"
   - 选择 `UnityFramework.xcodeproj`
   - 在 Runner target > General > Frameworks, Libraries, and Embedded Content
   - 点击 + 添加 `UnityFramework.framework`，设置为 "Embed & Sign"

3. **修改 Info.plist**
   
   在 `ios/Runner/Info.plist` 中添加：
   ```xml
   <key>io.flutter.embedded_views_preview</key>
   <true/>
   ```

---

## 💻 第四步：在 Flutter 中使用 Unity

### 4.1 创建基本的 Flutter UI

创建文件 `lib/main.dart`：

```dart
import 'package:flutter/material.dart';
import 'package:flutter_unity_widget/flutter_unity_widget.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Star Falling Unity',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: const UnityDemoScreen(),
    );
  }
}

class UnityDemoScreen extends StatefulWidget {
  const UnityDemoScreen({Key? key}) : super(key: key);

  @override
  State<UnityDemoScreen> createState() => _UnityDemoScreenState();
}

class _UnityDemoScreenState extends State<UnityDemoScreen> {
  UnityWidgetController? _unityWidgetController;
  bool isUnityLoaded = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Star Falling Animation'),
      ),
      body: Column(
        children: [
          // Unity Widget - 显示您的 Unity 场景
          Expanded(
            flex: 3,
            child: UnityWidget(
              onUnityCreated: onUnityCreated,
              onUnityMessage: onUnityMessage,
              onUnitySceneLoaded: onUnitySceneLoaded,
              fullscreen: false,
            ),
          ),
          
          // 控制按钮
          Expanded(
            flex: 1,
            child: Container(
              color: Colors.grey[200],
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  if (!isUnityLoaded)
                    const CircularProgressIndicator()
                  else ...[
                    ElevatedButton(
                      onPressed: () {
                        // 发送消息到 Unity
                        _unityWidgetController?.postMessage(
                          'MasterController',
                          'StartAnimation',
                          '',
                        );
                      },
                      child: const Text('开始动画'),
                    ),
                    const SizedBox(height: 10),
                    ElevatedButton(
                      onPressed: () {
                        // 暂停 Unity
                        _unityWidgetController?.pause();
                      },
                      child: const Text('暂停'),
                    ),
                    const SizedBox(height: 10),
                    ElevatedButton(
                      onPressed: () {
                        // 恢复 Unity
                        _unityWidgetController?.resume();
                      },
                      child: const Text('继续'),
                    ),
                  ],
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Unity 创建完成回调
  void onUnityCreated(UnityWidgetController controller) {
    print('Unity 已创建');
    _unityWidgetController = controller;
  }

  // Unity 场景加载完成回调
  void onUnitySceneLoaded(SceneLoaded? scene) {
    print('Unity 场景已加载: ${scene?.name}');
    setState(() {
      isUnityLoaded = true;
    });
  }

  // 接收来自 Unity 的消息
  void onUnityMessage(message) {
    print('来自 Unity 的消息: $message');
    // 处理从 Unity 发来的消息
  }

  @override
  void dispose() {
    _unityWidgetController?.dispose();
    super.dispose();
  }
}
```

---

## 🔄 第五步：Unity 与 Flutter 双向通信

### 5.1 在 Unity 中添加通信脚本

在 Unity 项目中创建 `Assets/Scripts/FlutterCommunication.cs`：

```csharp
using UnityEngine;
using FlutterUnityIntegration;

public class FlutterCommunication : MonoBehaviour
{
    private MasterController masterController;

    void Start()
    {
        masterController = FindObjectOfType<MasterController>();
        
        // 通知 Flutter Unity 已准备就绪
        SendMessageToFlutter("Unity已加载");
    }

    // Flutter 调用此方法开始动画
    public void StartAnimation()
    {
        Debug.Log("收到来自 Flutter 的开始动画指令");
        if (masterController != null)
        {
            masterController.StartAnimation();
        }
        SendMessageToFlutter("动画已开始");
    }

    // Flutter 调用此方法更改价格
    public void UpdatePrice(string price)
    {
        Debug.Log($"收到来自 Flutter 的价格更新: {price}");
        PriceTagAnimator priceTag = FindObjectOfType<PriceTagAnimator>();
        if (priceTag != null)
        {
            // 假设您的 PriceTagAnimator 有更新价格的方法
            // priceTag.UpdatePrice(price);
        }
        SendMessageToFlutter($"价格已更新为: {price}");
    }

    // 发送消息到 Flutter
    private void SendMessageToFlutter(string message)
    {
        UnityMessageManager.Instance.SendMessageToFlutter(message);
    }

    // Unity 动画事件回调
    public void OnAnimationComplete()
    {
        SendMessageToFlutter("动画完成");
    }
}
```

### 5.2 在 Unity 场景中设置通信

1. 在 Unity 场景中创建一个空 GameObject，命名为 `FlutterBridge`
2. 添加 `FlutterCommunication` 脚本
3. 保存场景
4. 重新导出 Unity 项目

---

## 🏃 第六步：运行和测试

### 6.1 运行 Flutter 应用

```bash
# Android
flutter run

# iOS（需要在 Mac 上）
flutter run -d ios

# 或者在模拟器/真机上运行
flutter devices  # 查看可用设备
flutter run -d <device-id>
```

### 6.2 调试技巧

1. **查看日志**
   ```bash
   flutter logs
   ```

2. **Unity 日志**
   - Android: 使用 Android Studio 的 Logcat
   - iOS: 使用 Xcode 的 Console

3. **常见问题排查**
   - Unity 场景黑屏：检查导出的场景是否正确
   - 崩溃：检查 Unity Player Settings 中的配置
   - 性能问题：考虑降低 Unity 场景的复杂度

---

## 🎨 第七步：优化和增强

### 7.1 处理 Unity 场景切换

```dart
// 加载特定场景
_unityWidgetController?.postMessage(
  'GameManager',
  'LoadScene',
  'SceneName',
);
```

### 7.2 传递复杂数据

```dart
import 'dart:convert';

// 发送 JSON 数据到 Unity
void sendDataToUnity() {
  final data = {
    'price': '¥99',
    'duration': 2.0,
    'autoPlay': true,
  };
  
  _unityWidgetController?.postMessage(
    'MasterController',
    'UpdateSettings',
    jsonEncode(data),
  );
}
```

在 Unity 中接收：

```csharp
using Newtonsoft.Json;

public void UpdateSettings(string jsonData)
{
    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
    string price = data["price"].ToString();
    float duration = float.Parse(data["duration"].ToString());
    bool autoPlay = bool.Parse(data["autoPlay"].ToString());
    
    // 使用这些数据...
}
```

### 7.3 性能优化建议

1. **Unity 端**
   - 使用对象池管理星星生成
   - 降低粒子效果复杂度
   - 优化材质和着色器
   - 使用 IL2CPP 编译

2. **Flutter 端**
   - 在不需要时暂停 Unity
   - 使用合适的 Unity Widget 大小
   - 避免频繁的消息传递

---

## 📱 第八步：构建发布版本

### Android APK/AAB

```bash
# 构建 APK
flutter build apk --release

# 构建 AAB（用于 Google Play）
flutter build appbundle --release
```

### iOS IPA

```bash
# 构建 iOS
flutter build ios --release

# 然后在 Xcode 中打包 IPA
```

---

## ⚠️ 常见问题和解决方案

### 问题 1: Newtonsoft.Json 冲突
**错误**: `Multiple precompiled assemblies`

**解决**:
- 重命名 `Newtonsoft.Json.dll` 为 `Newtonsoft.Json.dll.txt`
- 文件位置: `Assets/FlutterUnityIntegration/JsonDotNet/Assemblies/AOT/`

### 问题 2: Unity 黑屏
**原因**: 场景未正确导出

**解决**:
1. 确保场景在 Build Settings 中已添加
2. 检查 Player Settings 配置
3. 重新导出 Unity 项目

### 问题 3: Android 编译错误
**错误**: Gradle 版本不兼容

**解决**:
- 更新 `android/gradle/wrapper/gradle-wrapper.properties`
- 使用 Gradle 7.5 或更高版本
- 更新 Android Gradle Plugin 到 7.x

### 问题 4: iOS 编译错误
**错误**: Framework not found

**解决**:
1. 确认 UnityFramework 已正确添加到 Xcode 项目
2. 检查 Framework Search Paths
3. 清理 Xcode 构建缓存：`Product > Clean Build Folder`

---

## 🎯 项目结构总览

最终的项目结构应该是这样的：

```
star_falling_flutter/
├── android/
│   ├── app/
│   ├── unityLibrary/          # Unity Android 导出
│   ├── build.gradle
│   └── settings.gradle
├── ios/
│   ├── Runner/
│   ├── UnityFramework/        # Unity iOS 导出
│   └── Runner.xcworkspace
├── lib/
│   └── main.dart              # Flutter 主代码
├── pubspec.yaml
└── README.md
```

---

## 📚 参考资源

- [flutter_unity_widget GitHub](https://github.com/juicycleff/flutter-unity-view-widget)
- [Unity 文档](https://docs.unity3d.com/)
- [Flutter 文档](https://flutter.dev/docs)

---

## ✅ 检查清单

完成所有步骤后，请确认：

- [ ] Unity Package 已导入
- [ ] Unity 项目已正确配置
- [ ] Unity 项目已导出（Android/iOS）
- [ ] Flutter 项目已创建
- [ ] flutter_unity_widget 依赖已添加
- [ ] Unity 导出文件已复制到 Flutter 项目
- [ ] Android/iOS 配置已完成
- [ ] Flutter 应用可以正常运行
- [ ] Unity 场景在 Flutter 中正确显示
- [ ] Flutter 和 Unity 可以双向通信

---

## 🎉 完成！

现在您的 StarFalling Unity 动画已经成功集成到 Flutter 应用中了！

您可以在 Flutter 中控制 Unity 动画，并将其作为移动应用的一部分发布。
