using UnityEngine;

/// <summary>
/// 运行时自动设置天空盒
/// 在场景启动时确保使用正确的天空盒
/// </summary>
[DefaultExecutionOrder(-200)]
public class AutoSetSkyboxRuntime : MonoBehaviour
{
    [Header("天空盒设置")]
    [Tooltip("自动设置的天空盒材质路径")]
    private const string SKYBOX_PATH = "Fantasy Skybox FREE/Panoramics/FS003/FS003_Day_Sunless";

    void Awake()
    {
        SetDefaultSkybox();
    }

    private void SetDefaultSkybox()
    {
        // 尝试从 Resources 加载（如果素材在 Resources 文件夹）
        Material skyboxMat = Resources.Load<Material>(SKYBOX_PATH);
        
        if (skyboxMat != null)
        {
            RenderSettings.skybox = skyboxMat;
            DynamicGI.UpdateEnvironment();
            Debug.Log($"[AutoSetSkyboxRuntime] ✅ 已自动设置天空盒为: {skyboxMat.name}");
        }
        else
        {
            // 如果不在 Resources 文件夹，使用预设的材质引用
            Debug.LogWarning("[AutoSetSkyboxRuntime] 未找到天空盒材质，请确保天空盒在场景的 Lighting 设置中已配置");
        }
    }
}
