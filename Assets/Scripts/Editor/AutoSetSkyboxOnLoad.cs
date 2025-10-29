using UnityEngine;
using UnityEditor;

/// <summary>
/// Unity 启动时自动设置天空盒
/// </summary>
[InitializeOnLoad]
public class AutoSetSkyboxOnLoad
{
    static AutoSetSkyboxOnLoad()
    {
        // 延迟执行，确保项目已完全加载
        EditorApplication.delayCall += SetDefaultSkybox;
    }

    private static void SetDefaultSkybox()
    {
        string skyboxPath = "Assets/Fantasy Skybox FREE/Panoramics/FS003/FS003_Day_Sunless.mat";
        Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(skyboxPath);
        
        if (skyboxMat != null)
        {
            // 检查当前天空盒是否已经是 FS003_Day_Sunless
            if (RenderSettings.skybox == null || RenderSettings.skybox.name != "FS003_Day_Sunless")
            {
                RenderSettings.skybox = skyboxMat;
                DynamicGI.UpdateEnvironment();
                
                // 标记场景为已修改
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
                );
                
                Debug.Log($"[AutoSetSkybox] ✅ 已自动设置天空盒为: {skyboxMat.name}");
            }
        }
        else
        {
            Debug.LogWarning($"[AutoSetSkybox] ⚠️ 未找到天空盒材质: {skyboxPath}");
        }
    }
}
