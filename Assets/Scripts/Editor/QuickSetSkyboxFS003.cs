using UnityEditor;
using UnityEngine;

/// <summary>
/// 快速设置 FS003_Day_Sunless 天空盒工具
/// </summary>
public class QuickSetSkyboxFS003
{
    [MenuItem("Tools/Star Falling Animation/Quick Set Skybox (FS003 Sunless)")]
    private static void SetSkyboxFS003()
    {
        // 加载 FS003_Day_Sunless 天空盒
        string skyboxPath = "Assets/Fantasy Skybox FREE/Panoramics/FS003/FS003_Day_Sunless.mat";
        Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(skyboxPath);
        
        if (skyboxMat != null)
        {
            RenderSettings.skybox = skyboxMat;
            DynamicGI.UpdateEnvironment();
            
            // 标记场景为已修改
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
            
            Debug.Log($"[QuickSetSkybox] ✅ 已设置天空盒: {skyboxMat.name}");
            
            EditorUtility.DisplayDialog(
                "天空盒设置完成",
                "已成功设置天空盒为 FS003_Day_Sunless！\n\n" +
                "这是一个无太阳的白天天空，光线柔和。\n\n" +
                "请保存场景: Ctrl+S",
                "确定"
            );
        }
        else
        {
            Debug.LogError($"[QuickSetSkybox] ❌ 未找到天空盒材质: {skyboxPath}");
            
            EditorUtility.DisplayDialog(
                "错误",
                "未找到 FS003_Day_Sunless 天空盒！\n\n" +
                "请确保已导入 Fantasy Skybox FREE 资源包。\n\n" +
                "路径: Assets/Fantasy Skybox FREE/Panoramics/FS003/FS003_Day_Sunless.mat",
                "确定"
            );
        }
    }
}
