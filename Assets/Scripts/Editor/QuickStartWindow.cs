using UnityEngine;
using UnityEditor;

/// <summary>
/// 快速启动窗口 - 在首次使用时自动显示
/// </summary>
public class QuickStartWindow : EditorWindow
{
    private const string SHOWN_KEY = "StarFalling_QuickStartShown";
    private Vector2 scrollPosition;

    [InitializeOnLoadMethod]
    private static void OnProjectLoadedInEditor()
    {
        EditorApplication.delayCall += () =>
        {
            // 检查是否是第一次打开
            if (!EditorPrefs.GetBool(SHOWN_KEY, false))
            {
                ShowWindow();
                EditorPrefs.SetBool(SHOWN_KEY, true);
            }
        };
    }

    [MenuItem("Tools/Star Falling Animation/Quick Start Guide")]
    public static void ShowWindow()
    {
        QuickStartWindow window = GetWindow<QuickStartWindow>("快速启动指南");
        window.minSize = new Vector2(500, 600);
        window.Show();
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 标题
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 18;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("🌟 StarFalling 动画系统", titleStyle);
        EditorGUILayout.LabelField("快速启动指南", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(20);

        // 步骤1
        DrawStep(1, "自动设置场景", 
            "使用场景设置工具一键创建所有必要的GameObject和组件。",
            () => SceneSetupTool.ShowWindow());

        EditorGUILayout.Space(10);

        // 步骤2
        DrawStep(2, "创建预制体（可选）", 
            "创建自定义的星星和道路预制体，或使用默认的程序化生成。",
            () => PrefabCreator.ShowWindow());

        EditorGUILayout.Space(10);

        // 步骤3
        DrawInfoBox(3, "调整参数", 
            "在各个组件的Inspector中调整动画参数：\n" +
            "• 价签显示时间\n" +
            "• 星星掉落速度\n" +
            "• 道路延伸效果\n" +
            "• 摄像机角度");

        EditorGUILayout.Space(10);

        // 步骤4
        DrawInfoBox(4, "播放预览", 
            "点击Unity编辑器的播放按钮，动画将自动开始。\n" +
            "按Space键可以重新播放动画。");

        EditorGUILayout.Space(20);

        // 功能说明
        DrawFeatureSection();

        EditorGUILayout.Space(20);

        // 底部按钮
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("📁 打开场景设置工具", GUILayout.Height(40)))
        {
            SceneSetupTool.ShowWindow();
        }
        
        if (GUILayout.Button("🎨 打开Prefab创建工具", GUILayout.Height(40)))
        {
            PrefabCreator.ShowWindow();
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("✅ 我已了解，关闭此窗口", GUILayout.Height(30)))
        {
            Close();
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 绘制步骤（带按钮）
    /// </summary>
    private void DrawStep(int number, string title, string description, System.Action buttonAction)
    {
        EditorGUILayout.BeginVertical("box");
        
        GUIStyle stepTitleStyle = new GUIStyle(EditorStyles.boldLabel);
        stepTitleStyle.fontSize = 12;
        EditorGUILayout.LabelField($"步骤 {number}: {title}", stepTitleStyle);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button($"打开 {title}", GUILayout.Height(30)))
        {
            buttonAction?.Invoke();
        }
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制信息框
    /// </summary>
    private void DrawInfoBox(int number, string title, string description)
    {
        EditorGUILayout.BeginVertical("box");
        
        GUIStyle stepTitleStyle = new GUIStyle(EditorStyles.boldLabel);
        stepTitleStyle.fontSize = 12;
        EditorGUILayout.LabelField($"步骤 {number}: {title}", stepTitleStyle);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制功能说明部分
    /// </summary>
    private void DrawFeatureSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        GUIStyle featureTitleStyle = new GUIStyle(EditorStyles.boldLabel);
        featureTitleStyle.fontSize = 12;
        EditorGUILayout.LabelField("✨ 主要功能", featureTitleStyle);
        
        EditorGUILayout.Space(5);
        
        EditorGUILayout.LabelField("🎬 价签UI动画", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("   显示 → 停留 → 向上淡出", EditorStyles.wordWrappedMiniLabel);
        
        EditorGUILayout.Space(3);
        
        EditorGUILayout.LabelField("⭐ 星星掉落系统", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("   高空掉落 → 落地弹跳 → 渐隐消失", EditorStyles.wordWrappedMiniLabel);
        
        EditorGUILayout.Space(3);
        
        EditorGUILayout.LabelField("🛣️ 道路延伸动画", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("   星星消失触发道路逐段延伸", EditorStyles.wordWrappedMiniLabel);
        
        EditorGUILayout.Space(3);
        
        EditorGUILayout.LabelField("📹 摄像机系统", EditorStyles.miniLabel);
        EditorGUILayout.LabelField("   固定角度或跟随道路模式", EditorStyles.wordWrappedMiniLabel);
        
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 重置首次显示标记
    /// </summary>
    [MenuItem("Tools/Star Falling Animation/Reset Quick Start")]
    private static void ResetQuickStart()
    {
        EditorPrefs.DeleteKey(SHOWN_KEY);
        Debug.Log("[QuickStart] 快速启动窗口将在下次打开项目时显示");
    }
}
