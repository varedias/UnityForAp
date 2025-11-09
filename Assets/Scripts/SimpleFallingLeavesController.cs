using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 简单落叶控制器 - 使用程序化生成的叶子
/// 不需要外部模型文件
/// </summary>
public class SimpleFallingLeavesController : MonoBehaviour
{
    [Header("落叶外观")]
    [Tooltip("落叶颜色（秋天的颜色）")]
    [SerializeField] private Color[] leafColors = new Color[] {
        new Color(1f, 0.8f, 0.2f),    // 金黄色
        new Color(1f, 0.5f, 0.2f),    // 橙色
        new Color(0.8f, 0.3f, 0.1f),  // 红棕色
        new Color(1f, 0.9f, 0.3f)     // 浅黄色
    };
    
    [Tooltip("落叶大小")]
    [SerializeField] private Vector2 leafSizeRange = new Vector2(0.1f, 0.3f);
    
    [Header("生成区域配置")]
    [Tooltip("生成区域的宽度（米）")]
    [SerializeField] private float spawnWidth = 20f;  // 从 10 增加到 20
    
    [Tooltip("生成区域的深度（米）")]
    [SerializeField] private float spawnDepth = 15f;  // 从 5 增加到 15
    
    [Tooltip("生成高度")]
    [SerializeField] private float spawnHeight = 10f;
    
    [Tooltip("相对于摄像机的前方距离")]
    [SerializeField] private float forwardDistance = 5f;
    
    [Header("落叶效果配置")]
    [Tooltip("同时存在的最大落叶数量")]
    [SerializeField] private int maxLeaves = 150;  // 从 50 增加到 150
    
    [Tooltip("每秒生成的落叶数量")]
    [SerializeField] private float spawnRate = 15f;  // 从 5 增加到 15
    
    [Tooltip("落叶下落速度（米/秒）")]
    [SerializeField] private float fallSpeed = 2f;
    
    [Tooltip("落叶旋转速度（度/秒）")]
    [SerializeField] private float rotationSpeed = 90f;
    
    [Tooltip("落叶左右摆动幅度")]
    [SerializeField] private float swayAmount = 0.5f;
    
    [Tooltip("落叶摆动频率")]
    [SerializeField] private float swayFrequency = 1f;
    
    [Tooltip("落叶消失的高度阈值（低于此高度销毁）")]
    [SerializeField] private float despawnHeight = -2f;
    
    [Tooltip("是否在游戏开始时自动播放（用于测试）")]
    [SerializeField] private bool autoPlayOnStart = false;

    private Camera mainCamera;
    private bool isPlaying = false;
    private List<GameObject> activeLeaves = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private Material leafMaterial;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[SimpleFallingLeavesController] 未找到主摄像机！");
        }
        
        // 创建落叶材质
        CreateLeafMaterial();
        
        // 输出调试信息
        if (leafMaterial != null)
        {
            Debug.Log($"[SimpleFallingLeavesController] 材质着色器: {leafMaterial.shader.name}");
            Debug.Log($"[SimpleFallingLeavesController] 材质属性:");
            for (int i = 0; i < leafMaterial.shader.GetPropertyCount(); i++)
            {
                string propName = leafMaterial.shader.GetPropertyName(i);
                Debug.Log($"  - {propName}");
            }
        }
    }

    void Start()
    {
        if (autoPlayOnStart)
        {
            Debug.Log("[SimpleFallingLeavesController] 测试模式：自动播放落叶");
            StartFallingLeaves();
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            UpdateLeaves();
        }
    }

    /// <summary>
    /// 创建落叶材质
    /// </summary>
    private void CreateLeafMaterial()
    {
        // 尝试使用 URP/Unlit 着色器（避免光照问题）
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        
        // 如果找不到，尝试其他着色器
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        
        if (shader == null)
        {
            shader = Shader.Find("UI/Default");
        }
        
        if (shader == null)
        {
            // 最后的备选
            shader = Shader.Find("Sprites/Default");
        }
        
        leafMaterial = new Material(shader);
        leafMaterial.name = "LeafMaterial";
        
        // 设置默认颜色（金黄色）
        if (leafMaterial.HasProperty("_Color"))
        {
            leafMaterial.SetColor("_Color", new Color(1f, 0.8f, 0.2f));
        }
        
        if (leafMaterial.HasProperty("_BaseColor"))
        {
            leafMaterial.SetColor("_BaseColor", new Color(1f, 0.8f, 0.2f));
        }
        
        Debug.Log($"[SimpleFallingLeavesController] 使用着色器: {shader.name}");
    }

    /// <summary>
    /// 创建一片叶子的几何体（简单的四边形）
    /// </summary>
    private GameObject CreateLeafGeometry()
    {
        GameObject leaf = new GameObject("Leaf");
        
        // 添加 MeshFilter 和 MeshRenderer
        MeshFilter meshFilter = leaf.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = leaf.AddComponent<MeshRenderer>();
        
        // 创建叶子形状的网格（简化版，使用四边形）
        Mesh mesh = new Mesh();
        
        // 顶点（四边形稍微变形模拟叶子形状）
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, -0.7f, 0);
        vertices[1] = new Vector3(0.5f, -0.7f, 0);
        vertices[2] = new Vector3(-0.5f, 0.7f, 0);
        vertices[3] = new Vector3(0.5f, 0.7f, 0);
        
        // 三角形索引
        int[] triangles = new int[12];
        // 正面
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;
        // 背面（让叶子双面可见）
        triangles[6] = 0;
        triangles[7] = 1;
        triangles[8] = 2;
        triangles[9] = 2;
        triangles[10] = 1;
        triangles[11] = 3;
        
        // UV 坐标
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        
        // 法线（朝向摄像机）
        Vector3[] normals = new Vector3[4];
        normals[0] = Vector3.back;
        normals[1] = Vector3.back;
        normals[2] = Vector3.back;
        normals[3] = Vector3.back;
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.normals = normals;
        mesh.RecalculateBounds();
        
        meshFilter.mesh = mesh;
        meshRenderer.material = leafMaterial;
        
        return leaf;
    }

    /// <summary>
    /// 计算生成区域的中心位置
    /// </summary>
    private Vector3 CalculateSpawnCenter()
    {
        if (mainCamera == null)
        {
            return new Vector3(0, spawnHeight, 0);
        }
        
        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;
        
        return new Vector3(
            cameraPos.x + cameraForward.x * forwardDistance,
            spawnHeight,
            cameraPos.z + cameraForward.z * forwardDistance
        );
    }

    /// <summary>
    /// 获取随机生成位置
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = CalculateSpawnCenter();
        
        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float randomZ = Random.Range(-spawnDepth / 2f, spawnDepth / 2f);
        float randomY = Random.Range(0f, 2f);
        
        return new Vector3(
            center.x + randomX,
            center.y + randomY,
            center.z + randomZ
        );
    }

    /// <summary>
    /// 生成一片叶子
    /// </summary>
    private void SpawnLeaf()
    {
        if (activeLeaves.Count >= maxLeaves)
        {
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();
        Quaternion randomRotation = Random.rotation;

        GameObject leaf = CreateLeafGeometry();
        leaf.transform.position = spawnPos;
        leaf.transform.rotation = randomRotation;
        
        // 随机大小
        float size = Random.Range(leafSizeRange.x, leafSizeRange.y);
        leaf.transform.localScale = Vector3.one * size;
        
        // 随机颜色
        Color leafColor = leafColors[Random.Range(0, leafColors.Length)];
        MeshRenderer renderer = leaf.GetComponent<MeshRenderer>();
        
        // 创建材质实例（避免共享材质）
        Material leafMatInstance = new Material(leafMaterial);
        renderer.material = leafMatInstance;
        
        // 设置颜色（支持不同的着色器属性）
        if (leafMatInstance.HasProperty("_Color"))
        {
            leafMatInstance.SetColor("_Color", leafColor);
        }
        if (leafMatInstance.HasProperty("_BaseColor"))
        {
            leafMatInstance.SetColor("_BaseColor", leafColor);
        }
        
        // 确保材质是不透明的
        if (leafMatInstance.HasProperty("_Surface"))
        {
            leafMatInstance.SetFloat("_Surface", 0); // 0 = Opaque
        }
        
        // 添加运动组件
        LeafMotion motion = leaf.AddComponent<LeafMotion>();
        motion.fallSpeed = fallSpeed;
        motion.rotationSpeed = rotationSpeed;
        motion.swayAmount = swayAmount;
        motion.swayFrequency = swayFrequency;
        motion.swayPhase = Random.Range(0f, Mathf.PI * 2f);

        activeLeaves.Add(leaf);
    }

    /// <summary>
    /// 更新所有叶子
    /// </summary>
    private void UpdateLeaves()
    {
        for (int i = activeLeaves.Count - 1; i >= 0; i--)
        {
            GameObject leaf = activeLeaves[i];
            
            if (leaf == null)
            {
                activeLeaves.RemoveAt(i);
                continue;
            }

            if (leaf.transform.position.y < despawnHeight)
            {
                Destroy(leaf);
                activeLeaves.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 生成叶子的协程
    /// </summary>
    private IEnumerator SpawnLeavesCoroutine()
    {
        while (isPlaying)
        {
            SpawnLeaf();
            yield return new WaitForSeconds(1f / spawnRate);
        }
    }

    /// <summary>
    /// 开始落叶效果
    /// </summary>
    public void StartFallingLeaves()
    {
        if (isPlaying)
        {
            Debug.LogWarning("[SimpleFallingLeavesController] 落叶已在播放中！");
            return;
        }

        Debug.Log("[SimpleFallingLeavesController] 🍂 开始生成落叶");
        isPlaying = true;
        spawnCoroutine = StartCoroutine(SpawnLeavesCoroutine());
    }

    /// <summary>
    /// 停止落叶效果
    /// </summary>
    public void StopFallingLeaves()
    {
        if (!isPlaying)
        {
            return;
        }

        Debug.Log("[SimpleFallingLeavesController] 🍂 停止生成落叶");
        isPlaying = false;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    /// <summary>
    /// 重置落叶效果（立即清除所有叶子）
    /// </summary>
    public void ResetFallingLeaves()
    {
        Debug.Log("[SimpleFallingLeavesController] 重置落叶效果");
        StopFallingLeaves();
        ClearAllLeaves();
    }
    
    /// <summary>
    /// 淡出所有叶子（逐渐消失）
    /// </summary>
    /// <param name="duration">淡出持续时间（秒）</param>
    public void FadeOutLeaves(float duration = 2f)
    {
        Debug.Log($"[SimpleFallingLeavesController] 🍂 开始淡出叶子 ({duration} 秒)");
        
        // 停止生成新叶子
        StopFallingLeaves();
        
        // 对每片现有的叶子启动淡出
        foreach (GameObject leaf in activeLeaves)
        {
            if (leaf != null)
            {
                LeafFadeOut fadeOut = leaf.AddComponent<LeafFadeOut>();
                fadeOut.duration = duration;
            }
        }
        
        // 启动协程在淡出完成后清理列表
        StartCoroutine(ClearLeavesAfterFadeOut(duration));
    }
    
    /// <summary>
    /// 在淡出完成后清理叶子列表
    /// </summary>
    private IEnumerator ClearLeavesAfterFadeOut(float duration)
    {
        yield return new WaitForSeconds(duration + 0.5f);
        
        // 清理已销毁的叶子引用
        for (int i = activeLeaves.Count - 1; i >= 0; i--)
        {
            if (activeLeaves[i] == null)
            {
                activeLeaves.RemoveAt(i);
            }
        }
        
        Debug.Log("[SimpleFallingLeavesController] ✅ 淡出完成，叶子已清除");
    }

    /// <summary>
    /// 清除所有叶子
    /// </summary>
    private void ClearAllLeaves()
    {
        foreach (GameObject leaf in activeLeaves)
        {
            if (leaf != null)
            {
                Destroy(leaf);
            }
        }
        activeLeaves.Clear();
        Debug.Log("[SimpleFallingLeavesController] ✅ 所有落叶已清除");
    }

    void OnDestroy()
    {
        ClearAllLeaves();
    }
}

/// <summary>
/// 叶子运动组件
/// </summary>
public class LeafMotion : MonoBehaviour
{
    [HideInInspector] public float fallSpeed = 2f;
    [HideInInspector] public float rotationSpeed = 90f;
    [HideInInspector] public float swayAmount = 0.5f;
    [HideInInspector] public float swayFrequency = 1f;
    [HideInInspector] public float swayPhase = 0f;

    private float timeElapsed = 0f;

    void Update()
    {
        timeElapsed += Time.deltaTime;

        // 下落
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 左右摆动
        float swayOffset = Mathf.Sin(timeElapsed * swayFrequency * Mathf.PI * 2f + swayPhase) * swayAmount;
        transform.position += Vector3.right * swayOffset * Time.deltaTime;

        // 旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, rotationSpeed * 0.5f * Time.deltaTime, Space.Self);
    }
}

/// <summary>
/// 叶子淡出组件 - 让叶子逐渐变透明并销毁
/// </summary>
public class LeafFadeOut : MonoBehaviour
{
    [HideInInspector] public float duration = 2f;
    
    private float elapsedTime = 0f;
    private MeshRenderer meshRenderer;
    private Material material;
    private Color originalColor;
    private bool isTransparent = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.material != null)
        {
            material = meshRenderer.material;
            
            // 获取原始颜色
            if (material.HasProperty("_Color"))
            {
                originalColor = material.GetColor("_Color");
            }
            else if (material.HasProperty("_BaseColor"))
            {
                originalColor = material.GetColor("_BaseColor");
            }
            else
            {
                originalColor = Color.white;
            }
            
            // 设置材质为透明模式
            SetupTransparentMaterial();
        }
    }

    void Update()
    {
        if (material == null)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);
        
        // 计算当前透明度（从 1 到 0）
        float alpha = 1f - progress;
        
        // 更新颜色的透明度
        Color newColor = originalColor;
        newColor.a = alpha;
        
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", newColor);
        }
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", newColor);
        }

        // 淡出完成后销毁
        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 设置材质为透明模式
    /// </summary>
    private void SetupTransparentMaterial()
    {
        if (material == null) return;
        
        // URP 着色器设置
        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1); // 1 = Transparent
        }
        
        if (material.HasProperty("_Blend"))
        {
            material.SetFloat("_Blend", 0); // 0 = Alpha
        }
        
        // 设置渲染队列为透明
        material.renderQueue = 3000;
        
        // 启用透明度
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        
        // 设置关键字
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        
        isTransparent = true;
    }
}
