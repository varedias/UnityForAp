# StarFalling - Unity Animation System

Unity星星下落动画系统，包含价格标签UI、星星消失和道路延伸动画。

**🎉 NEW: 现已支持 Flutter 集成！** 查看 [Flutter 集成指南](#-flutter-集成)

## ✨ 功能特性

- 🌟 **星星下落动画** - 平滑的下落轨迹和消失效果
- 💰 **价格标签UI** - 跟随星星的动态价格显示
- 🛣️ **道路延伸** - 星星消失时触发的道路生成
- 🎨 **自动材质修复** - 运行时自动修复紫色材质问题
- 📹 **优化摄像机** - 完美视角同时看到天空和道路
- 📱 **Flutter 支持** - 可嵌入 Flutter 应用（NEW!）

## 🎮 快速开始

1. 打开Unity项目
2. 打开场景：`Assets/Scenes/SampleScene.unity`
3. 点击播放 ▶

或使用一键创建工具：
```
Tools > Star Falling Animation > Scene Setup
点击 "一键创建场景"
```

## 📦 核心组件

- **MasterController** - 主控制器
- **StarController** - 星星动画控制
- **PriceTagAnimator** - 价格标签动画
- **RoadManager** - 道路管理
- **CameraController** - 摄像机控制
- **RuntimeMaterialFixer** - 自动材质修复

## 🔧 外部资源支持

- Fantasy Skybox FREE
- KajamansRoads
- 自动材质转换（Built-in → URP）

## 📝 文档

### Unity 使用文档
- `QUICK_START.txt` - 快速入门指南
- `USAGE_GUIDE.txt` - 详细使用说明
- `MANUAL_SETUP_GUIDE.txt` - 手动配置指南

### Flutter 集成文档 🆕
- `FLUTTER_INTEGRATION_INDEX.md` - 📚 **开始这里** - 总索引和快速导航
- `FLUTTER_INTEGRATION_GUIDE.md` - 📖 完整的技术集成指南
- `FLUTTER_INTEGRATION_CHECKLIST.md` - ✅ 快速检查清单（30分钟完成）
- `FlutterExample/` - 💻 示例代码和说明
- `flutter_integration_helper.ps1` - 🛠️ 自动化辅助脚本

## 🛠️ 技术栈

- Unity 2021.3+
- Universal Render Pipeline (URP)
- TextMeshPro
- C# .NET Standard 2.1

## 📸 效果展示

- 摄像机视角优化：同时显示天空和道路
- 真实道路纹理：自动修复材质
- 流畅动画：价格标签跟随星星下落

## 🎯 使用方法

详见 `QUICK_START.txt` 和 `USAGE_GUIDE.txt`

---

## 📱 Flutter 集成

### 快速开始

将这个 Unity 动画集成到您的 Flutter 应用只需 3 步：

1. **阅读索引文档**
   ```
   打开 FLUTTER_INTEGRATION_INDEX.md
   了解所有可用资源
   ```

2. **选择您的路径**
   - 🚀 **快速集成**: 使用 `FLUTTER_INTEGRATION_CHECKLIST.md`（30分钟）
   - 📖 **详细学习**: 阅读 `FLUTTER_INTEGRATION_GUIDE.md`（完整教程）
   - 🤖 **自动化**: 运行 `flutter_integration_helper.ps1`（部分自动化）

3. **开始集成**
   ```powershell
   # 运行自动化辅助脚本
   .\flutter_integration_helper.ps1
   ```

### 特性

- ✅ **双向通信** - Flutter ↔ Unity 无缝消息传递
- ✅ **完整控制** - 从 Flutter 控制动画、价格等
- ✅ **示例代码** - 开箱即用的 Flutter UI
- ✅ **自动化脚本** - PowerShell 脚本加速集成
- ✅ **详细文档** - 每一步都有说明

### 支持平台

| 平台 | 状态 | 最低版本 |
|------|------|----------|
| 🤖 Android | ✅ 完全支持 | API 22 |
| 🍎 iOS | ✅ 完全支持 | iOS 12.0 |
| 🌐 Web | ⚠️ 实验性 | - |

### 集成架构

```
Flutter App (UI + 控制)
    ↕ 消息通信
Unity Scene (动画 + 逻辑)
```

### 文档结构

```
FLUTTER_INTEGRATION_INDEX.md        ← 从这里开始
├── FLUTTER_INTEGRATION_GUIDE.md    (详细教程)
├── FLUTTER_INTEGRATION_CHECKLIST.md (快速清单)
├── FlutterExample/
│   ├── main.dart                   (示例代码)
│   ├── pubspec.yaml                (依赖配置)
│   └── README.md                   (说明文档)
├── Assets/Scripts/
│   └── FlutterCommunication.cs     (Unity通信脚本)
└── flutter_integration_helper.ps1  (自动化脚本)
```

### 需要帮助？

1. 📚 查看 `FLUTTER_INTEGRATION_INDEX.md` 找到合适的文档
2. ✅ 使用 `FLUTTER_INTEGRATION_CHECKLIST.md` 跟踪进度
3. 🔧 运行 `flutter_integration_helper.ps1` 自动化设置
4. 💬 查看常见问题部分

---

Made with ❤️ for Unity and Flutter
