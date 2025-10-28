using UnityEngine;
using UnityEditor;

/// <summary>
/// 快速修复道路扭曲问题
/// </summary>
public class QuickFixRoad : EditorWindow
{
    [MenuItem("Tools/Star Falling Animation/Quick Fix Road")]
    public static void ShowWindow()
    {
        GetWindow<QuickFixRoad>("修复道路");
    }

    void OnGUI()
    {
        GUILayout.Label("道路扭曲修复", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "如果道路显示扭曲，点击按钮修复：\n" +
            "• 自动旋转道路预制体 90°\n" +
            "• 清除现有道路并重新生成",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("🔧 修复道路扭曲", GUILayout.Height(40)))
        {
            FixRoad();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("🔄 重新生成道路", GUILayout.Height(30)))
        {
            RegenerateRoad();
        }
    }

    private void FixRoad()
    {
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        
        if (roadManager == null)
        {
            EditorUtility.DisplayDialog("错误", "找不到 RoadManager！", "确定");
            return;
        }

        // 使用反射设置私有字段
        var type = roadManager.GetType();
        
        var rotateField = type.GetField("rotateRoadPrefab", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
            
        var rotationField = type.GetField("roadPrefabRotation", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (rotateField != null && rotationField != null)
        {
            rotateField.SetValue(roadManager, true);
            rotationField.SetValue(roadManager, 90f);
            
            EditorUtility.SetDirty(roadManager);
            
            Debug.Log("[QuickFixRoad] ✅ 道路设置已修复:");
            Debug.Log("  Rotate Road Prefab: true");
            Debug.Log("  Road Prefab Rotation: 90°");
            
            EditorUtility.DisplayDialog(
                "修复完成",
                "道路旋转设置已修复！\n\n" +
                "Rotate Road Prefab: ✅\n" +
                "Rotation: 90°\n\n" +
                "请重新生成道路或播放场景测试。",
                "确定"
            );
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "无法访问 RoadManager 字段！", "确定");
        }
    }
    
    private void RegenerateRoad()
    {
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        
        if (roadManager == null)
        {
            EditorUtility.DisplayDialog("错误", "找不到 RoadManager！", "确定");
            return;
        }

        // 调用 Initialize 方法重新生成道路
        var type = roadManager.GetType();
        var initMethod = type.GetMethod("Initialize");
        
        if (initMethod != null)
        {
            initMethod.Invoke(roadManager, null);
            
            Debug.Log("[QuickFixRoad] ✅ 道路已重新生成");
            
            EditorUtility.DisplayDialog(
                "重新生成完成",
                "道路已重新生成！\n\n请检查 Scene 窗口。",
                "确定"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "提示",
                "请在播放模式下重新生成道路，\n或使用 Scene Setup 工具。",
                "确定"
            );
        }
    }
}
