using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// 自动创建中文 TextMeshPro 字体资源的编辑器工具
/// </summary>
[InitializeOnLoad]
public class CreateChineseFontAsset : AssetPostprocessor
{
    private static TMP_FontAsset cachedChineseFontAsset;
    
    // 静态构造函数 - Unity 启动时或脚本重新编译时自动执行
    static CreateChineseFontAsset()
    {
        // 延迟执行以确保 Unity 完全加载
        EditorApplication.delayCall += () => 
        {
            AutoApplyFontOnStartup();
        };
        
        // 监听场景加载
        EditorSceneManager.sceneOpened += OnSceneOpened;
        
        // 监听进入 Play 模式
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        // 场景打开时自动应用字体
        EditorApplication.delayCall += () => AutoApplyFontSilently();
    }
    
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 进入 Play 模式前自动应用字体
        if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingEditMode)
        {
            EditorApplication.delayCall += () => AutoApplyFontSilently();
        }
    }
    
    private static void AutoApplyFontOnStartup()
    {
        TMP_FontAsset fontAsset = FindChineseFontAsset();
        if (fontAsset != null)
        {
            Debug.Log($"<color=cyan>[自动字体] Unity 启动,检测到中文字体: {fontAsset.name}</color>");
            AutoApplyChineseFontToScene(fontAsset, showDialog: false);
        }
    }
    
    private static void AutoApplyFontSilently()
    {
        TMP_FontAsset fontAsset = FindChineseFontAsset();
        if (fontAsset != null)
        {
            AutoApplyChineseFontToScene(fontAsset, showDialog: false);
        }
    }
    
    private static TMP_FontAsset FindChineseFontAsset()
    {
        if (cachedChineseFontAsset != null)
        {
            return cachedChineseFontAsset;
        }
        
        // 查找 Assets/Font 目录下的字体资源
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Font" });
        if (guids.Length > 0)
        {
            string fontAssetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            cachedChineseFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
            return cachedChineseFontAsset;
        }
        
        return null;
    }
    
    // 监听资源导入,当有新的 TMP_FontAsset 创建时自动应用
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string path in importedAssets)
        {
            // 检查是否是在 Assets/Font 目录下新创建的 TMP_FontAsset
            if (path.Contains("Assets/Font") && path.EndsWith(".asset"))
            {
                TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                if (fontAsset != null)
                {
                    cachedChineseFontAsset = fontAsset; // 更新缓存
                    Debug.Log($"<color=green>检测到新的中文字体资源: {fontAsset.name},正在自动应用到场景...</color>");
                    EditorApplication.delayCall += () => AutoApplyChineseFontToScene(fontAsset, showDialog: true);
                }
            }
        }
    }
    
    /// <summary>
    /// 自动应用中文字体到场景
    /// </summary>
    private static void AutoApplyChineseFontToScene(TMP_FontAsset fontAsset, bool showDialog = true)
    {
        if (fontAsset == null) return;
        
        // 查找场景中所有的 TextMeshProUGUI 组件
        TMPro.TextMeshProUGUI[] allTexts = Object.FindObjectsOfType<TMPro.TextMeshProUGUI>();
        
        int count = 0;
        foreach (var text in allTexts)
        {
            // 如果字体已经是中文字体,跳过
            if (text.font == fontAsset)
            {
                continue;
            }
            
            // 更新所有文本组件
            Undo.RecordObject(text, "Apply Chinese Font");
            text.font = fontAsset;
            EditorUtility.SetDirty(text);
            count++;
            Debug.Log($"<color=green>✓</color> 已为 <color=yellow>{text.gameObject.name}</color> 应用中文字体");
        }
        
        if (count > 0)
        {
            Debug.Log($"<color=green>✓ 自动应用成功!</color> 已为 {count} 个文本组件应用中文字体: {fontAsset.name}");
            
            if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    "✓ 自动应用成功",
                    $"已自动为 {count} 个文本组件应用中文字体!\n\n" +
                    $"字体: {fontAsset.name}\n\n" +
                    "场景已自动保存。",
                    "确定"
                );
            }
            
            // 自动保存场景
            EditorSceneManager.SaveOpenScenes();
        }
    }
    [MenuItem("Tools/Create Chinese Font Asset")]
    public static void CreateFontAsset()
    {
        // 字体文件路径
        string fontPath = "Assets/Font/OTF/SimplifiedChinese/SourceHanSansSC-Regular.otf";
        
        // 检查字体文件是否存在
        if (!File.Exists(fontPath))
        {
            Debug.LogError($"字体文件不存在: {fontPath}");
            return;
        }
        
        // 加载字体
        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (sourceFont == null)
        {
            Debug.LogError($"无法加载字体: {fontPath}");
            return;
        }
        
        Debug.Log("正在生成中文字体资源,请稍候...");
        
        // 需要包含的字符(换行符也要包含)
        string characters = "0123456789元税款米的道路养护=该商品为¥\n";
        
        // 创建字体资源的设置
        // 注意: 这需要在 Project Settings 中手动完成,或者使用反射调用内部API
        // 这里提供手动步骤的提示
        
        EditorUtility.DisplayDialog(
            "创建 TextMeshPro 字体资源", 
            "字体文件已找到!\n\n请按照以下步骤操作:\n\n" +
            "1. 在菜单栏选择 Window > TextMeshPro > Font Asset Creator\n\n" +
            "2. 在 Font Asset Creator 窗口中:\n" +
            "   - Source Font Object: 拖入 SourceHanSansSC-Regular\n" +
            "   - Sampling Point Size: Auto Sizing\n" +
            "   - Padding: 5\n" +
            "   - Packing Method: Optimum\n" +
            "   - Atlas Resolution: 2048 x 2048\n" +
            "   - Character Set: Custom Characters\n" +
            "   - Custom Character List: 0123456789元税款米的道路养护=该商品为¥\n" +
            "   - Render Mode: SDFAA_HINTED\n" +
            "   - Font Style: Normal\n\n" +
            "3. 点击 'Generate Font Atlas'\n\n" +
            "4. 点击 'Save' 保存为 'ChineseFont SDF'\n" +
            "   建议保存路径: Assets/Font/\n\n" +
            "5. 保存后会自动应用到场景中的文本组件",
            "好的,我知道了"
        );
        
        // 选中字体文件,方便用户拖拽
        Selection.activeObject = sourceFont;
        EditorGUIUtility.PingObject(sourceFont);
        
        // 延迟检查是否有字体资源生成,如果有则自动应用
        EditorApplication.delayCall += () => CheckAndApplyFont();
    }
    
    private static void CheckAndApplyFont()
    {
        // 查找生成的字体资源
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Font" });
        
        if (guids.Length > 0)
        {
            // 自动应用字体
            ApplyChineseFontToScene();
        }
    }
    
    [MenuItem("Tools/Apply Chinese Font to Scene")]
    public static void ApplyChineseFontToScene()
    {
        // 查找生成的字体资源
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Font" });
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "未找到字体资源",
                "请先使用 Window > TextMeshPro > Font Asset Creator 生成字体资源!\n\n" +
                "或者运行菜单: Tools > Create Chinese Font Asset 查看详细步骤",
                "确定"
            );
            return;
        }
        
        // 使用第一个找到的字体资源
        string fontAssetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
        
        if (fontAsset == null)
        {
            Debug.LogError("无法加载字体资源!");
            return;
        }
        
        Debug.Log($"找到字体资源: {fontAsset.name}");
        
        // 查找场景中所有的 TextMeshProUGUI 组件
        TMPro.TextMeshProUGUI[] allTexts = Object.FindObjectsOfType<TMPro.TextMeshProUGUI>();
        
        int count = 0;
        foreach (var text in allTexts)
        {
            // 只更新包含中文的文本组件
            if (text.text.Contains("元") || text.text.Contains("税") || 
                text.text.Contains("款") || text.text.Contains("道") || 
                text.text.Contains("路") || text.text.Contains("养") || 
                text.text.Contains("护"))
            {
                Undo.RecordObject(text, "Apply Chinese Font");
                text.font = fontAsset;
                EditorUtility.SetDirty(text);
                count++;
                Debug.Log($"已为 {text.gameObject.name} 应用中文字体");
            }
        }
        
        if (count > 0)
        {
            EditorUtility.DisplayDialog(
                "应用成功",
                $"已为 {count} 个文本组件应用中文字体!\n\n" +
                "请保存场景以保留更改。",
                "确定"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "未找到文本组件",
                "场景中没有找到包含中文的 TextMeshPro 文本组件。\n\n" +
                "请确保:\n" +
                "1. RoadTaxText GameObject 已创建\n" +
                "2. 包含 TextMeshProUGUI 组件\n" +
                "3. 文本内容包含中文字符",
                "确定"
            );
        }
    }
}
