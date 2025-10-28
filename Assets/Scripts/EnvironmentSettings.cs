using UnityEngine;

/// <summary>
/// 环境配置管理器 - 管理天空盒、光照等环境设置
/// </summary>
public class EnvironmentSettings : MonoBehaviour
{
    [Header("天空盒设置")]
    [Tooltip("天空盒材质 - 从 Fantasy Skybox 资源包中拖入")]
    [SerializeField] private Material skyboxMaterial;
    
    [Tooltip("使用程序化天空（如果已设置 skyboxMaterial，此项将被忽略）")]
    [SerializeField] private bool useProceduralSky = false;
    
    [Tooltip("天空颜色（顶部）")]
    [SerializeField] private Color skyTopColor = new Color(0.4f, 0.7f, 1f);
    
    [Tooltip("天空颜色（中部）")]
    [SerializeField] private Color skyMiddleColor = new Color(0.8f, 0.9f, 1f);
    
    [Tooltip("天空颜色（底部）")]
    [SerializeField] private Color skyBottomColor = new Color(1f, 1f, 1f);

    [Header("环境光设置")]
    [Tooltip("环境光模式")]
    [SerializeField] private UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
    
    [Tooltip("环境光强度")]
    [SerializeField] private float ambientIntensity = 1.0f;
    
    [Tooltip("天空环境光颜色")]
    [SerializeField] private Color ambientSkyColor = new Color(0.5f, 0.7f, 1f);
    
    [Tooltip("赤道环境光颜色")]
    [SerializeField] private Color ambientEquatorColor = new Color(0.8f, 0.8f, 0.8f);
    
    [Tooltip("地面环境光颜色")]
    [SerializeField] private Color ambientGroundColor = new Color(0.3f, 0.3f, 0.3f);

    [Header("雾效设置")]
    [Tooltip("启用雾效")]
    [SerializeField] private bool enableFog = false;
    
    [Tooltip("雾效颜色")]
    [SerializeField] private Color fogColor = new Color(0.8f, 0.9f, 1f);
    
    [Tooltip("雾效模式")]
    [SerializeField] private FogMode fogMode = FogMode.Linear;
    
    [Tooltip("雾效起始距离")]
    [SerializeField] private float fogStartDistance = 10f;
    
    [Tooltip("雾效结束距离")]
    [SerializeField] private float fogEndDistance = 50f;

    [Header("后处理")]
    [Tooltip("启用抗锯齿")]
    [SerializeField] private bool enableAntiAliasing = true;

    void Start()
    {
        ApplyEnvironmentSettings();
    }

    /// <summary>
    /// 应用所有环境设置
    /// </summary>
    public void ApplyEnvironmentSettings()
    {
        ApplySkybox();
        ApplyAmbientLight();
        ApplyFog();
        
        Debug.Log("[EnvironmentSettings] 已应用环境设置");
    }

    /// <summary>
    /// 应用天空盒设置
    /// </summary>
    private void ApplySkybox()
    {
        // 优先使用导入的天空盒材质
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            Debug.Log($"[EnvironmentSettings] 使用自定义天空盒: {skyboxMaterial.name}");
        }
        else if (useProceduralSky)
        {
            // 创建程序化天空材质
            Material proceduralSky = CreateProceduralSkybox();
            RenderSettings.skybox = proceduralSky;
            Debug.Log("[EnvironmentSettings] 使用程序化天空盒");
        }
        else
        {
            // 使用默认天空盒来避免紫色背景
            CreateDefaultSkybox();
            Debug.LogWarning("[EnvironmentSettings] 未设置天空盒材质！已创建默认天空盒。请从 Fantasy Skybox 资源包中拖入材质以获得更好效果。");
        }

        // 更新天空盒
        DynamicGI.UpdateEnvironment();
    }

    /// <summary>
    /// 创建默认天空盒（避免紫色背景）
    /// </summary>
    private void CreateDefaultSkybox()
    {
        // 尝试使用 Unity 内置的 Default-Skybox
        Material defaultSky = RenderSettings.skybox;
        
        if (defaultSky == null)
        {
            // 创建一个简单的纯色天空盒
            Material simpleSky = new Material(Shader.Find("Skybox/Procedural"));
            if (simpleSky != null && simpleSky.shader != null)
            {
                simpleSky.SetColor("_SkyTint", new Color(0.5f, 0.7f, 1f, 1f));
                simpleSky.SetColor("_GroundColor", new Color(0.4f, 0.4f, 0.4f, 1f));
                simpleSky.SetFloat("_SunSize", 0.04f);
                simpleSky.SetFloat("_SunSizeConvergence", 5f);
                simpleSky.SetFloat("_AtmosphereThickness", 1.0f);
                simpleSky.SetFloat("_Exposure", 1.3f);
                RenderSettings.skybox = simpleSky;
            }
            else
            {
                Debug.LogError("[EnvironmentSettings] 无法创建默认天空盒！请确保安装了 URP 或使用 Built-in 渲染管线。");
            }
        }
    }

    /// <summary>
    /// 创建程序化天空盒
    /// </summary>
    private Material CreateProceduralSkybox()
    {
        // 使用渐变天空
        Material skyMaterial = new Material(Shader.Find("Skybox/Gradient"));
        
        if (skyMaterial.shader.name == "Skybox/Gradient")
        {
            skyMaterial.SetColor("_TopColor", skyTopColor);
            skyMaterial.SetColor("_MiddleColor", skyMiddleColor);
            skyMaterial.SetColor("_BottomColor", skyBottomColor);
            skyMaterial.SetFloat("_Exponent", 1.5f);
        }
        else
        {
            // 如果没有渐变着色器，使用纯色天空
            skyMaterial = new Material(Shader.Find("Skybox/Procedural"));
            skyMaterial.SetColor("_SkyTint", skyTopColor);
            skyMaterial.SetColor("_GroundColor", skyBottomColor);
        }

        return skyMaterial;
    }

    /// <summary>
    /// 应用环境光设置
    /// </summary>
    private void ApplyAmbientLight()
    {
        RenderSettings.ambientMode = ambientMode;
        RenderSettings.ambientIntensity = ambientIntensity;

        switch (ambientMode)
        {
            case UnityEngine.Rendering.AmbientMode.Trilight:
                RenderSettings.ambientSkyColor = ambientSkyColor;
                RenderSettings.ambientEquatorColor = ambientEquatorColor;
                RenderSettings.ambientGroundColor = ambientGroundColor;
                break;

            case UnityEngine.Rendering.AmbientMode.Flat:
                RenderSettings.ambientSkyColor = ambientSkyColor;
                break;

            case UnityEngine.Rendering.AmbientMode.Skybox:
                // 使用天空盒颜色
                break;
        }
    }

    /// <summary>
    /// 应用雾效设置
    /// </summary>
    private void ApplyFog()
    {
        RenderSettings.fog = enableFog;
        
        if (enableFog)
        {
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = fogMode;
            
            if (fogMode == FogMode.Linear)
            {
                RenderSettings.fogStartDistance = fogStartDistance;
                RenderSettings.fogEndDistance = fogEndDistance;
            }
        }
    }

    /// <summary>
    /// 设置白天环境
    /// </summary>
    [ContextMenu("Set Daytime Environment")]
    public void SetDaytimeEnvironment()
    {
        skyTopColor = new Color(0.4f, 0.7f, 1f);
        skyMiddleColor = new Color(0.8f, 0.9f, 1f);
        skyBottomColor = new Color(1f, 1f, 1f);
        
        ambientSkyColor = new Color(0.5f, 0.7f, 1f);
        ambientEquatorColor = new Color(0.8f, 0.8f, 0.8f);
        ambientGroundColor = new Color(0.3f, 0.3f, 0.3f);
        
        ApplyEnvironmentSettings();
        Debug.Log("[EnvironmentSettings] 已切换到白天环境");
    }

    /// <summary>
    /// 设置黄昏环境
    /// </summary>
    [ContextMenu("Set Sunset Environment")]
    public void SetSunsetEnvironment()
    {
        skyTopColor = new Color(1f, 0.5f, 0.3f);
        skyMiddleColor = new Color(1f, 0.7f, 0.4f);
        skyBottomColor = new Color(1f, 0.9f, 0.6f);
        
        ambientSkyColor = new Color(1f, 0.6f, 0.4f);
        ambientEquatorColor = new Color(0.8f, 0.6f, 0.4f);
        ambientGroundColor = new Color(0.4f, 0.3f, 0.2f);
        
        ApplyEnvironmentSettings();
        Debug.Log("[EnvironmentSettings] 已切换到黄昏环境");
    }

    /// <summary>
    /// 设置夜晚环境
    /// </summary>
    [ContextMenu("Set Night Environment")]
    public void SetNightEnvironment()
    {
        skyTopColor = new Color(0.05f, 0.05f, 0.2f);
        skyMiddleColor = new Color(0.1f, 0.1f, 0.3f);
        skyBottomColor = new Color(0.15f, 0.15f, 0.35f);
        
        ambientSkyColor = new Color(0.1f, 0.1f, 0.3f);
        ambientEquatorColor = new Color(0.2f, 0.2f, 0.3f);
        ambientGroundColor = new Color(0.05f, 0.05f, 0.1f);
        
        ApplyEnvironmentSettings();
        Debug.Log("[EnvironmentSettings] 已切换到夜晚环境");
    }

    #if UNITY_EDITOR
    /// <summary>
    /// 编辑器中实时更新
    /// </summary>
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyEnvironmentSettings();
        }
    }
    #endif
}
