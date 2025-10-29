using UnityEngine;
using UnityEditor;

/// <summary>
/// 快速修复摄像机视角
/// </summary>
public class QuickFixCamera : EditorWindow
{
    [MenuItem("Tools/Star Falling Animation/Quick Fix Camera")]
    public static void ShowWindow()
    {
        GetWindow<QuickFixCamera>("修复摄像机");
    }

    void OnGUI()
    {
        GUILayout.Label("摄像机视角修复", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "点击按钮修复摄像机视角，确保：\n" +
            "• 上方显示天空\n" +
            "• 下方显示道路",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("🔧 修复摄像机视角", GUILayout.Height(40)))
        {
            FixCamera();
        }
    }

    private void FixCamera()
    {
        Camera mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            EditorUtility.DisplayDialog("错误", "找不到 Main Camera！", "确定");
            return;
        }

        // 设置位置和旋转
        mainCamera.transform.position = new Vector3(0, 8, -12);
        mainCamera.transform.eulerAngles = new Vector3(5, 3, 0);
        
        // 设置FOV
        mainCamera.fieldOfView = 60f;
        
        // 设置Clear Flags
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        
        // 标记场景为已修改
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
        
        Debug.Log("[QuickFixCamera] ✅ 摄像机已修复:");
        Debug.Log($"  位置: {mainCamera.transform.position}");
        Debug.Log($"  旋转: {mainCamera.transform.eulerAngles}");
        Debug.Log($"  FOV: {mainCamera.fieldOfView}");
        Debug.Log($"  Clear Flags: {mainCamera.clearFlags}");
        
        EditorUtility.DisplayDialog(
            "修复完成",
            "摄像机视角已修复！\n\n" +
            "位置: (0, 8, -12)\n" +
            "旋转: (5, 3, 0)\n" +
            "FOV: 60\n\n" +
            "请保存场景并测试。",
            "确定"
        );
    }
}
