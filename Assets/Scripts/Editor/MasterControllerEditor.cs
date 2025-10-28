using UnityEngine;
using UnityEditor;

/// <summary>
/// MasterController 自定义编辑器
/// </summary>
[CustomEditor(typeof(MasterController))]
public class MasterControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MasterController controller = (MasterController)target;

        // 绘制标题
        EditorGUILayout.Space();
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("StarFalling 主控制器", titleStyle);
        EditorGUILayout.Space();

        // 显示状态信息
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("动画正在运行中...\n按Space键可重新播放动画", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("按下播放按钮开始预览动画", MessageType.Info);
        }

        EditorGUILayout.Space();

        // 绘制默认Inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // 快捷按钮
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("▶ 重新播放动画", GUILayout.Height(35)))
            {
                controller.ResetAnimation();
                controller.StartAnimation();
            }
            
            if (GUILayout.Button("⏹ 重置", GUILayout.Height(35)))
            {
                controller.ResetAnimation();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("在运行时可使用控制按钮", MessageType.None);
        }

        EditorGUILayout.Space();

        // 帮助按钮
        if (GUILayout.Button("📖 查看完整使用说明"))
        {
            ShowHelpWindow();
        }
    }

    /// <summary>
    /// 显示帮助窗口
    /// </summary>
    private void ShowHelpWindow()
    {
        string helpMessage = 
            "=== StarFalling 使用说明 ===\n\n" +
            "1. 确保所有组件引用已正确设置\n" +
            "2. 按下播放按钮启动动画\n" +
            "3. 动画会自动按顺序执行\n\n" +
            "【组件检查】\n" +
            "• PriceTagAnimator: 价签动画控制器\n" +
            "• StarSpawner: 星星生成器\n" +
            "• RoadManager: 道路管理器\n\n" +
            "【快捷键】\n" +
            "Space: 重新播放动画\n\n" +
            "如果组件未自动分配，请手动拖拽到对应的字段。";

        EditorUtility.DisplayDialog("使用说明", helpMessage, "确定");
    }
}
