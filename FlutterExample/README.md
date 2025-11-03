# Star Falling Flutter 集成示例

这个文件夹包含了将 Unity 项目集成到 Flutter 的示例代码。

## 📁 文件说明

- `main.dart` - Flutter 主应用代码，包含完整的 UI 和 Unity 通信逻辑
- `pubspec.yaml` - Flutter 项目依赖配置文件

## 🚀 使用步骤

### 1. 创建 Flutter 项目

```bash
flutter create star_falling_flutter
cd star_falling_flutter
```

### 2. 复制示例代码

将以下文件复制到您的 Flutter 项目中：

- 复制 `main.dart` 到 `lib/main.dart`（替换原文件）
- 复制 `pubspec.yaml` 的内容到您项目的 `pubspec.yaml`

### 3. 安装依赖

```bash
flutter pub get
```

### 4. 集成 Unity 导出文件

按照 `FLUTTER_INTEGRATION_GUIDE.md` 中的步骤：

1. 在 Unity 中导入 `flutter_unity_widget` 包
2. 导出 Unity 项目（Android/iOS）
3. 将导出的 `unityLibrary` 复制到 Flutter 项目的 `android/` 目录
4. 配置 `settings.gradle` 和 `build.gradle`

### 5. 添加 Flutter 通信脚本

在 Unity 项目中：

1. 将 `Assets/Scripts/FlutterCommunication.cs` 添加到场景中
2. 创建一个空 GameObject，命名为 `FlutterBridge`
3. 将 `FlutterCommunication` 脚本附加到 `FlutterBridge`
4. 重新导出 Unity 项目

### 6. 运行应用

```bash
flutter run
```

## 📱 功能特性

示例应用包含以下功能：

- ✅ Unity 场景嵌入显示
- ✅ 开始/暂停/重置动画控制
- ✅ 动态更新价格标签
- ✅ 实时状态显示
- ✅ 双向消息通信
- ✅ JSON 数据传递
- ✅ 友好的 UI 界面

## 🎮 控制方法

### 从 Flutter 控制 Unity

```dart
// 开始动画
_unityWidgetController?.postMessage('FlutterBridge', 'StartAnimation', '');

// 更新价格
_unityWidgetController?.postMessage('FlutterBridge', 'UpdatePrice', '¥99');

// 暂停
_unityWidgetController?.pause();

// 恢复
_unityWidgetController?.resume();
```

### 从 Unity 发送消息到 Flutter

```csharp
// 在 FlutterCommunication.cs 中
SendMessageToFlutter(new {
    type = "animation_complete",
    message = "动画完成"
});
```

## 📚 更多文档

详细的集成指南请查看项目根目录的 `FLUTTER_INTEGRATION_GUIDE.md`

## ⚠️ 注意事项

1. 确保 Unity 版本与导出设置兼容
2. Android 最低 API 级别为 22
3. iOS 最低版本为 12.0
4. 需要正确配置 Unity Player Settings
5. GameObject 名称 `FlutterBridge` 必须与代码中一致

## 🐛 常见问题

### Unity 场景黑屏
- 检查场景是否在 Build Settings 中
- 确认 Player Settings 配置正确

### 消息无法发送
- 检查 GameObject 名称是否正确
- 确认 FlutterCommunication 脚本已附加

### 编译错误
- 确认所有依赖已正确安装
- 清理并重新构建项目

## 📞 获取帮助

遇到问题？请查看：
- Unity 控制台日志
- Flutter 日志 (`flutter logs`)
- GitHub Issues
