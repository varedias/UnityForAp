using UnityEngine;

/// <summary>
/// WebGL 调试助手 - 显示场景中的对象信息
/// </summary>
public class WebGLDebugger : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool enableDebug = true;
    [SerializeField] private float updateInterval = 2f;
    
    private float nextUpdate = 0f;

    void Start()
    {
        if (enableDebug)
        {
            Debug.Log("=== WebGL 调试助手已启动 ===");
            LogSceneInfo();
        }
    }

    void Update()
    {
        if (!enableDebug) return;
        
        if (Time.time >= nextUpdate)
        {
            nextUpdate = Time.time + updateInterval;
            LogRoadInfo();
        }
    }

    void LogSceneInfo()
    {
        Debug.Log("--- 场景信息 ---");
        
        // 查找 RoadManager
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager != null)
        {
            Debug.Log($"✓ 找到 RoadManager: {roadManager.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("✗ 未找到 RoadManager");
        }
        
        // 查找 MasterController
        MasterController master = FindObjectOfType<MasterController>();
        if (master != null)
        {
            Debug.Log($"✓ 找到 MasterController: {master.gameObject.name}");
        }
        
        // 查找所有 Renderer
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        Debug.Log($"场景中共有 {renderers.Length} 个 Renderer");
    }

    void LogRoadInfo()
    {
        // 查找所有名称包含 "Road" 的对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int roadCount = 0;
        int visibleRoadCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("road") || 
                obj.name.Contains("km_") || 
                obj.name.Contains("l10"))
            {
                roadCount++;
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null && renderer.enabled)
                {
                    visibleRoadCount++;
                    Debug.Log($"道路对象: {obj.name}, 位置: {obj.transform.position}, 激活: {obj.activeInHierarchy}, 可见: {renderer.enabled}");
                    
                    // 检查材质
                    if (renderer.sharedMaterial != null)
                    {
                        Debug.Log($"  材质: {renderer.sharedMaterial.name}, 着色器: {renderer.sharedMaterial.shader.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"  ✗ 没有材质！");
                    }
                }
            }
        }
        
        Debug.Log($"--- 道路统计: 找到 {roadCount} 个道路对象，其中 {visibleRoadCount} 个可见 ---");
    }
}
