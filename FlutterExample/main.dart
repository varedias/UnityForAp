import 'package:flutter/material.dart';
import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'dart:convert';

void main() {
  runApp(const StarFallingApp());
}

class StarFallingApp extends StatelessWidget {
  const StarFallingApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Star Falling Animation',
      theme: ThemeData(
        primarySwatch: Colors.blue,
        useMaterial3: true,
      ),
      home: const UnityDemoScreen(),
      debugShowCheckedModeBanner: false,
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
  bool isPlaying = true;
  String statusMessage = '正在加载 Unity...';
  String currentPrice = '¥99';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Star Falling Animation'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        actions: [
          IconButton(
            icon: Icon(isPlaying ? Icons.pause : Icons.play_arrow),
            onPressed: isUnityLoaded ? _togglePlayPause : null,
            tooltip: isPlaying ? '暂停' : '播放',
          ),
        ],
      ),
      body: Column(
        children: [
          // Unity Widget - 显示您的 Unity 场景
          Expanded(
            flex: 3,
            child: Container(
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey),
              ),
              child: UnityWidget(
                onUnityCreated: _onUnityCreated,
                onUnityMessage: _onUnityMessage,
                onUnitySceneLoaded: _onUnitySceneLoaded,
                fullscreen: false,
                enablePlaceholder: true,
                placeholder: const Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      CircularProgressIndicator(),
                      SizedBox(height: 20),
                      Text('正在加载 Unity 场景...'),
                    ],
                  ),
                ),
              ),
            ),
          ),

          // 状态显示区域
          Container(
            padding: const EdgeInsets.all(8.0),
            color: Colors.grey[100],
            child: Row(
              children: [
                Icon(
                  isUnityLoaded ? Icons.check_circle : Icons.hourglass_empty,
                  color: isUnityLoaded ? Colors.green : Colors.orange,
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    statusMessage,
                    style: const TextStyle(fontSize: 14),
                  ),
                ),
              ],
            ),
          ),

          // 控制面板
          Expanded(
            flex: 2,
            child: Container(
              color: Colors.white,
              padding: const EdgeInsets.all(16.0),
              child: isUnityLoaded
                  ? _buildControlPanel()
                  : const Center(
                      child: CircularProgressIndicator(),
                    ),
            ),
          ),
        ],
      ),
    );
  }

  /// 构建控制面板
  Widget _buildControlPanel() {
    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const Text(
            '动画控制',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),

          // 基本控制按钮
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: [
              ElevatedButton.icon(
                onPressed: _startAnimation,
                icon: const Icon(Icons.play_arrow),
                label: const Text('开始动画'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.green,
                  foregroundColor: Colors.white,
                ),
              ),
              ElevatedButton.icon(
                onPressed: _resetAnimation,
                icon: const Icon(Icons.refresh),
                label: const Text('重置动画'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.orange,
                  foregroundColor: Colors.white,
                ),
              ),
              ElevatedButton.icon(
                onPressed: _getStatus,
                icon: const Icon(Icons.info),
                label: const Text('获取状态'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                ),
              ),
            ],
          ),

          const SizedBox(height: 24),
          const Divider(),
          const SizedBox(height: 16),

          // 价格设置
          const Text(
            '价格设置',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),

          Row(
            children: [
              Expanded(
                child: TextField(
                  decoration: const InputDecoration(
                    labelText: '输入价格',
                    hintText: '例如: ¥99 或 $99',
                    border: OutlineInputBorder(),
                    prefixIcon: Icon(Icons.attach_money),
                  ),
                  onChanged: (value) {
                    setState(() {
                      currentPrice = value;
                    });
                  },
                ),
              ),
              const SizedBox(width: 8),
              ElevatedButton(
                onPressed: () => _updatePrice(currentPrice),
                child: const Text('更新'),
              ),
            ],
          ),

          const SizedBox(height: 16),

          // 快速价格选择
          const Text('快速选择:'),
          const SizedBox(height: 8),
          Wrap(
            spacing: 8,
            children: [
              _buildPriceChip('¥99'),
              _buildPriceChip('¥199'),
              _buildPriceChip('¥299'),
              _buildPriceChip('\$9.99'),
              _buildPriceChip('\$19.99'),
            ],
          ),

          const SizedBox(height: 24),
          const Divider(),
          const SizedBox(height: 16),

          // 高级设置
          const Text(
            '高级设置',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),

          ElevatedButton.icon(
            onPressed: _updateSettings,
            icon: const Icon(Icons.settings),
            label: const Text('更新完整设置'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.purple,
              foregroundColor: Colors.white,
            ),
          ),
        ],
      ),
    );
  }

  /// 价格选择芯片
  Widget _buildPriceChip(String price) {
    return ActionChip(
      label: Text(price),
      onPressed: () => _updatePrice(price),
      backgroundColor: Colors.blue[50],
    );
  }

  // ==================== Unity 回调方法 ====================

  /// Unity 创建完成回调
  void _onUnityCreated(UnityWidgetController controller) {
    print('Unity 已创建');
    _unityWidgetController = controller;
    setState(() {
      statusMessage = 'Unity 控制器已创建';
    });
  }

  /// Unity 场景加载完成回调
  void _onUnitySceneLoaded(SceneLoaded? scene) {
    print('Unity 场景已加载: ${scene?.name}');
    setState(() {
      isUnityLoaded = true;
      statusMessage = '场景已加载: ${scene?.name ?? "未知"}';
    });

    // 获取初始状态
    _getStatus();
  }

  /// 接收来自 Unity 的消息
  void _onUnityMessage(message) {
    print('来自 Unity 的消息: $message');

    try {
      // 尝试解析 JSON 消息
      final data = jsonDecode(message);
      final type = data['type'] ?? 'unknown';
      final msg = data['message'] ?? message.toString();

      setState(() {
        statusMessage = msg;
      });

      // 根据消息类型显示不同的通知
      if (type == 'error') {
        _showSnackBar(msg, Colors.red);
      } else if (type == 'animation_complete') {
        _showSnackBar('🎉 $msg', Colors.green);
      } else if (type == 'ready') {
        _showSnackBar('✅ $msg', Colors.blue);
      } else {
        _showSnackBar(msg, Colors.grey);
      }
    } catch (e) {
      // 如果不是 JSON，直接显示消息
      setState(() {
        statusMessage = message.toString();
      });
      _showSnackBar(message.toString(), Colors.grey);
    }
  }

  // ==================== Flutter 调用 Unity 方法 ====================

  /// 开始动画
  void _startAnimation() {
    _unityWidgetController?.postMessage(
      'FlutterBridge',
      'StartAnimation',
      '',
    );
    _showSnackBar('已发送开始动画指令', Colors.blue);
  }

  /// 暂停/恢复动画
  void _togglePlayPause() {
    if (isPlaying) {
      _unityWidgetController?.pause();
      _unityWidgetController?.postMessage(
        'FlutterBridge',
        'PauseAnimation',
        '',
      );
    } else {
      _unityWidgetController?.resume();
      _unityWidgetController?.postMessage(
        'FlutterBridge',
        'ResumeAnimation',
        '',
      );
    }

    setState(() {
      isPlaying = !isPlaying;
    });
  }

  /// 重置动画
  void _resetAnimation() {
    _unityWidgetController?.postMessage(
      'FlutterBridge',
      'ResetAnimation',
      '',
    );
    _showSnackBar('已发送重置动画指令', Colors.orange);
  }

  /// 更新价格
  void _updatePrice(String price) {
    _unityWidgetController?.postMessage(
      'FlutterBridge',
      'UpdatePrice',
      price,
    );
    setState(() {
      currentPrice = price;
    });
    _showSnackBar('已更新价格: $price', Colors.green);
  }

  /// 获取状态
  void _getStatus() {
    _unityWidgetController?.postMessage(
      'FlutterBridge',
      'GetStatus',
      '',
    );
  }

  /// 更新完整设置
  void _updateSettings() {
    final settings = {
      'price': currentPrice,
      'duration': 2.0,
      'autoPlay': true,
    };

    _unityWidgetController?.postMessage(
      'FlutterBridge',
      'UpdateSettings',
      jsonEncode(settings),
    );
    _showSnackBar('已发送设置更新', Colors.purple);
  }

  // ==================== 辅助方法 ====================

  /// 显示 SnackBar 消息
  void _showSnackBar(String message, Color color) {
    if (!mounted) return;

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: color,
        duration: const Duration(seconds: 2),
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  @override
  void dispose() {
    _unityWidgetController?.dispose();
    super.dispose();
  }
}
