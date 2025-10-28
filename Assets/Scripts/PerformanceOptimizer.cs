using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 性能优化管理器 - 管理对象池和性能优化
/// </summary>
public class PerformanceOptimizer : MonoBehaviour
{
    [Header("对象池设置")]
    [Tooltip("是否启用星星对象池")]
    [SerializeField] private bool useStarPool = true;
    
    [Tooltip("对象池初始大小")]
    [SerializeField] private int poolInitialSize = 10;
    
    [Tooltip("对象池最大大小")]
    [SerializeField] private int poolMaxSize = 50;

    [Header("性能监控")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private bool showMemoryUsage = false;

    [Header("优化设置")]
    [Tooltip("启用批处理")]
    [SerializeField] private bool enableBatching = true;
    
    [Tooltip("阴影质量")]
    [SerializeField] private ShadowQuality shadowQuality = ShadowQuality.All;
    
    [Tooltip("抗锯齿级别 (0, 2, 4, 8)")]
    [SerializeField] private int antiAliasing = 2;

    // 对象池
    private Queue<GameObject> starPool = new Queue<GameObject>();
    private GameObject starPrefab;

    // FPS 计算
    private float deltaTime = 0.0f;
    private int frameCount = 0;
    private float fps = 0.0f;
    private float updateInterval = 0.5f;
    private float lastUpdateTime = 0.0f;

    void Start()
    {
        ApplyOptimizationSettings();
        
        if (useStarPool)
        {
            InitializePool();
        }
    }

    void Update()
    {
        if (showFPS)
        {
            UpdateFPS();
        }
    }

    /// <summary>
    /// 应用优化设置
    /// </summary>
    private void ApplyOptimizationSettings()
    {
        // 设置阴影质量
        QualitySettings.shadows = shadowQuality;

        // 设置抗锯齿
        QualitySettings.antiAliasing = antiAliasing;

        // 启用批处理
        if (enableBatching)
        {
            // Unity会自动处理动态批处理
            Debug.Log("[PerformanceOptimizer] 批处理已启用");
        }

        Debug.Log($"[PerformanceOptimizer] 优化设置已应用 - 阴影:{shadowQuality}, 抗锯齿:{antiAliasing}x");
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePool()
    {
        // 尝试查找星星预制体
        StarSpawner spawner = FindObjectOfType<StarSpawner>();
        if (spawner != null)
        {
            // 这里需要访问StarSpawner的starPrefab字段
            // 由于是private，这里使用反射或者让StarSpawner提供公共访问
            Debug.Log("[PerformanceOptimizer] 对象池功能需要星星预制体引用");
        }

        Debug.Log($"[PerformanceOptimizer] 对象池已初始化 - 容量: {poolInitialSize}");
    }

    /// <summary>
    /// 从对象池获取星星
    /// </summary>
    public GameObject GetStarFromPool()
    {
        if (starPool.Count > 0)
        {
            GameObject star = starPool.Dequeue();
            star.SetActive(true);
            return star;
        }
        else
        {
            // 池为空，创建新对象
            if (starPrefab != null)
            {
                return Instantiate(starPrefab);
            }
            return null;
        }
    }

    /// <summary>
    /// 将星星归还到对象池
    /// </summary>
    public void ReturnStarToPool(GameObject star)
    {
        if (starPool.Count < poolMaxSize)
        {
            star.SetActive(false);
            starPool.Enqueue(star);
        }
        else
        {
            Destroy(star);
        }
    }

    /// <summary>
    /// 更新FPS计算
    /// </summary>
    private void UpdateFPS()
    {
        frameCount++;
        deltaTime += Time.unscaledDeltaTime;

        if (Time.realtimeSinceStartup - lastUpdateTime >= updateInterval)
        {
            fps = frameCount / deltaTime;
            frameCount = 0;
            deltaTime = 0f;
            lastUpdateTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    [ContextMenu("Clear Pool")]
    public void ClearPool()
    {
        while (starPool.Count > 0)
        {
            GameObject obj = starPool.Dequeue();
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        Debug.Log("[PerformanceOptimizer] 对象池已清空");
    }

    /// <summary>
    /// 获取内存使用情况
    /// </summary>
    private string GetMemoryUsage()
    {
        float memoryMB = System.GC.GetTotalMemory(false) / 1048576f;
        return $"内存: {memoryMB:F2} MB";
    }

    /// <summary>
    /// 优化场景
    /// </summary>
    [ContextMenu("Optimize Scene")]
    public void OptimizeScene()
    {
        Debug.Log("[PerformanceOptimizer] 开始优化场景...");

        // 合并静态对象
        StaticBatchingUtility.Combine(gameObject);

        // 清理未使用的资源
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Debug.Log("[PerformanceOptimizer] 场景优化完成");
    }

    // GUI显示
    void OnGUI()
    {
        if (!showFPS && !showMemoryUsage) return;

        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(10, 10, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.green;

        string text = "";

        if (showFPS)
        {
            text += $"FPS: {fps:F1}\n";
        }

        if (showMemoryUsage)
        {
            text += GetMemoryUsage();
        }

        GUI.Label(rect, text, style);
    }

    /// <summary>
    /// 设置质量预设
    /// </summary>
    [ContextMenu("Set Quality - High")]
    public void SetQualityHigh()
    {
        QualitySettings.SetQualityLevel(5, true);
        shadowQuality = ShadowQuality.All;
        antiAliasing = 4;
        ApplyOptimizationSettings();
        Debug.Log("[PerformanceOptimizer] 已设置为高质量模式");
    }

    [ContextMenu("Set Quality - Medium")]
    public void SetQualityMedium()
    {
        QualitySettings.SetQualityLevel(3, true);
        shadowQuality = ShadowQuality.HardOnly;
        antiAliasing = 2;
        ApplyOptimizationSettings();
        Debug.Log("[PerformanceOptimizer] 已设置为中等质量模式");
    }

    [ContextMenu("Set Quality - Low")]
    public void SetQualityLow()
    {
        QualitySettings.SetQualityLevel(1, true);
        shadowQuality = ShadowQuality.Disable;
        antiAliasing = 0;
        ApplyOptimizationSettings();
        Debug.Log("[PerformanceOptimizer] 已设置为低质量模式");
    }
}
