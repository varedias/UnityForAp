using UnityEngine;
using System.Collections;

/// <summary>
/// 星星生成器 - 自动生成和管理星星
/// </summary>
public class StarSpawner : MonoBehaviour
{
    [Header("星星预制体")]
    [Tooltip("星星预制体（包含StarController组件）")]
    [SerializeField] private GameObject starPrefab;

    [Header("生成配置")]
    [Tooltip("总共生成的星星数量（0表示无限）")]
    [SerializeField] private int totalStarsToSpawn = 10;
    
    [Tooltip("每颗星星之间的生成间隔（秒）")]
    [SerializeField] private float spawnInterval = 3.0f;
    
    [Tooltip("第一颗星星的额外延迟（秒）")]
    [SerializeField] private float firstStarDelay = 0.5f;

    [Header("生成位置")]
    [Tooltip("生成位置中心点")]
    [SerializeField] private Vector3 spawnCenter = new Vector3(0, 0, 5);
    
    [Tooltip("生成位置随机范围（X轴）")]
    [SerializeField] private float spawnRangeX = 3f;
    
    [Tooltip("生成位置随机范围（Z轴）")]
    [SerializeField] private float spawnRangeZ = 2f;

    [Header("高级选项")]
    [Tooltip("是否随机化生成间隔")]
    [SerializeField] private bool randomizeInterval = false;
    
    [Tooltip("随机间隔的最小值")]
    [SerializeField] private float minRandomInterval = 2.0f;
    
    [Tooltip("随机间隔的最大值")]
    [SerializeField] private float maxRandomInterval = 4.0f;

    // 内部变量
    private int spawnedStarCount = 0;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    void Awake()
    {
        // 如果未设置预制体，尝试创建默认星星
        if (starPrefab == null)
        {
            Debug.LogWarning("[StarSpawner] 未设置星星预制体，将使用程序化星星");
        }
    }

    /// <summary>
    /// 开始生成星星
    /// </summary>
    public void StartSpawning()
    {
        if (isSpawning)
        {
            Debug.LogWarning("[StarSpawner] 已在生成星星中！");
            return;
        }

        Debug.Log($"[StarSpawner] 开始生成星星 - 总数: {(totalStarsToSpawn == 0 ? "无限" : totalStarsToSpawn.ToString())}");
        spawnedStarCount = 0;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// 停止生成星星
    /// </summary>
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        isSpawning = false;
        Debug.Log("[StarSpawner] 已停止生成星星");
    }

    /// <summary>
    /// 生成星星的协程
    /// </summary>
    private IEnumerator SpawnRoutine()
    {
        isSpawning = true;

        // 第一颗星星的额外延迟
        if (firstStarDelay > 0)
        {
            Debug.Log($"[StarSpawner] 等待 {firstStarDelay} 秒后生成第一颗星星...");
            yield return new WaitForSeconds(firstStarDelay);
        }

        // 持续生成星星
        while (totalStarsToSpawn == 0 || spawnedStarCount < totalStarsToSpawn)
        {
            // 生成一颗星星
            SpawnStar();

            // 等待间隔
            float waitTime = GetSpawnInterval();
            Debug.Log($"[StarSpawner] 等待 {waitTime:F2} 秒后生成下一颗星星...");
            yield return new WaitForSeconds(waitTime);
        }

        Debug.Log($"[StarSpawner] 所有星星已生成完毕！总计: {spawnedStarCount}");
        isSpawning = false;
    }

    /// <summary>
    /// 生成单颗星星
    /// </summary>
    private void SpawnStar()
    {
        spawnedStarCount++;

        // 计算生成位置（带随机偏移）
        Vector3 spawnPosition = CalculateSpawnPosition();

        GameObject star;

        if (starPrefab != null)
        {
            // 使用预制体
            star = Instantiate(starPrefab, transform);
            star.transform.position = spawnPosition;
        }
        else
        {
            // 创建程序化星星
            star = CreateProceduralStar(spawnPosition);
        }

        // 命名
        star.name = $"Star_{spawnedStarCount:D3}";

        // 初始化星星控制器
        StarController controller = star.GetComponent<StarController>();
        if (controller == null)
        {
            controller = star.AddComponent<StarController>();
        }
        controller.Initialize(spawnPosition);

        Debug.Log($"[StarSpawner] 已生成第 {spawnedStarCount} 颗星星于位置: {spawnPosition}");
    }

    /// <summary>
    /// 计算星星生成位置
    /// </summary>
    private Vector3 CalculateSpawnPosition()
    {
        // 在指定范围内随机生成位置
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float randomZ = Random.Range(-spawnRangeZ, spawnRangeZ);

        Vector3 position = spawnCenter + new Vector3(randomX, 0, randomZ);
        
        // 转换为世界坐标
        return transform.TransformPoint(position);
    }

    /// <summary>
    /// 获取生成间隔
    /// </summary>
    private float GetSpawnInterval()
    {
        if (randomizeInterval)
        {
            return Random.Range(minRandomInterval, maxRandomInterval);
        }
        else
        {
            return spawnInterval;
        }
    }

    /// <summary>
    /// 创建程序化星星（当没有预制体时）
    /// </summary>
    private GameObject CreateProceduralStar(Vector3 position)
    {
        GameObject star = new GameObject("Star_Procedural");
        star.transform.position = position;
        star.transform.parent = transform;

        // 创建星星模型（使用球体）
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = star.transform;
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localScale = Vector3.one * 0.5f;

        // 创建发光效果（可选：添加多个小球体）
        for (int i = 0; i < 5; i++)
        {
            GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spike.transform.parent = star.transform;
            spike.transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
            spike.transform.localRotation = Quaternion.Euler(0, 0, i * 72f);
        }

        // 设置材质
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(1f, 0.9f, 0.3f); // 金黄色
            renderer.material = mat;

            // 设置为透明模式
            mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
            mat.SetFloat("_Blend", 0); // 0 = Alpha, 1 = Premultiply, 2 = Additive, 3 = Multiply
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = 3000;
        }

        // 添加发光效果（设置子对象材质）
        foreach (Transform child in star.transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null && child != sphere.transform)
            {
                childRenderer.material = renderer.material;
            }
        }

        return star;
    }

    /// <summary>
    /// 手动生成一颗星星（测试用）
    /// </summary>
    [ContextMenu("Spawn Single Star")]
    public void SpawnSingleStar()
    {
        if (Application.isPlaying)
        {
            SpawnStar();
        }
        else
        {
            Debug.LogWarning("请在运行时生成星星！");
        }
    }

    /// <summary>
    /// 重置生成器
    /// </summary>
    public void Reset()
    {
        StopSpawning();
        spawnedStarCount = 0;
    }

    // 编辑器辅助功能
    void OnDrawGizmosSelected()
    {
        // 绘制生成范围
        Gizmos.color = Color.cyan;
        Vector3 worldCenter = transform.TransformPoint(spawnCenter);
        
        // 绘制生成中心点
        Gizmos.DrawWireSphere(worldCenter, 0.2f);

        // 绘制生成范围框
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Vector3 size = new Vector3(spawnRangeX * 2, 0.1f, spawnRangeZ * 2);
        Gizmos.DrawWireCube(worldCenter, size);
    }
}
