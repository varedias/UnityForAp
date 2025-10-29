using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 打开场景时自动设置天空盒
/// </summary>
[InitializeOnLoad]
public class AutoSetSkyboxOnSceneOpen
{
    static AutoSetSkyboxOnSceneOpen()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
    {
        SetCorrectSkybox();
    }

    private static void SetCorrectSkybox()
    {
        string skyboxPath = "Assets/Fantasy Skybox FREE/Panoramics/FS003/FS003_Day_Sunless.mat";
        Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(skyboxPath);
        
        if (skyboxMat != null)
        {
            // 检查当前天空盒
            bool needsChange = RenderSettings.skybox == null || 
                              RenderSettings.skybox.name != "FS003_Day_Sunless";
            
            if (needsChange)
            {
                RenderSettings.skybox = skyboxMat;
                DynamicGI.UpdateEnvironment();
                
                // 标记场景为已修改
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                
                Debug.Log($"[SceneOpen] ✅ 场景打开时自动设置天空盒为: {skyboxMat.name}");
            }
        }
        else
        {
            Debug.LogWarning($"[SceneOpen] ⚠️ 未找到天空盒材质: {skyboxPath}");
        }
    }
}
