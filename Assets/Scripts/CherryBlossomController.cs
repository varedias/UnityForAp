using UnityEngine;
using System.Collections;

/// <summary>
/// 樱花掉落控制器
/// 功能: 控制樱花粒子效果的播放和停止
/// </summary>
public class CherryBlossomController : MonoBehaviour
{
    [Header("樱花粒子系统")]
    [Tooltip("樱花粒子预制体")]
    [SerializeField] private GameObject cherryBlossomPrefab;
    
    [Tooltip("樱花生成的位置（留空则自动在摄像机前方生成）")]
    [SerializeField] private Transform spawnPosition;
    
    [Tooltip("粒子生成高度（如果没有设置 spawnPosition）")]
    [SerializeField] private float spawnHeight = 10f;
    
    [Tooltip("相对于摄像机的前方距离")]
    [SerializeField] private float forwardDistance = 5f;
    
    [Header("樱花效果配置")]
    [Tooltip("樱花掉落持续时间（秒）")]
    [SerializeField] private float duration = 5f;
    
    [Tooltip("樱花淡出时间（秒）")]
    [SerializeField] private float fadeOutDuration = 1f;
    
    [Tooltip("樱花掉落密度倍数（1 = 默认，2 = 双倍，3 = 三倍）")]
    [SerializeField] private float densityMultiplier = 3f;
    
    [Tooltip("最大粒子数量")]
    [SerializeField] private int maxParticles = 1000;
    
    [Tooltip("是否在游戏开始时自动播放（用于测试）")]
    [SerializeField] private bool autoPlayOnStart = false;

    private GameObject cherryBlossomInstance;
    private ParticleSystem particleSystem;
    private ParticleSystem[] allParticleSystems; // 支持多个粒子系统
    private bool isPlaying = false;
    private Camera mainCamera;

    void Awake()
    {
        // 获取主摄像机
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("[CherryBlossomController] 未找到主摄像机！");
        }
        
        // 如果没有设置生成位置，将在摄像机前方生成
        if (spawnPosition == null)
        {
            // 创建一个临时的生成点
            GameObject spawnPoint = new GameObject("CherryBlossomSpawnPoint");
            spawnPoint.transform.SetParent(transform);
            spawnPosition = spawnPoint.transform;
        }
    }

    void Start()
    {
        // 用于测试：自动播放
        if (autoPlayOnStart)
        {
            Debug.Log("[CherryBlossomController] 测试模式：自动播放樱花");
            StartCherryBlossom();
        }
    }
    
    /// <summary>
    /// 计算樱花生成位置（在摄像机前方）
    /// </summary>
    private Vector3 CalculateSpawnPosition()
    {
        if (mainCamera == null)
        {
            // 如果没有摄像机，使用默认位置
            return new Vector3(0, spawnHeight, 0);
        }
        
        // 获取摄像机位置
        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;
        
        // 在摄像机前方 forwardDistance 米的位置，高度为 spawnHeight
        Vector3 spawnPos = new Vector3(
            cameraPos.x + cameraForward.x * forwardDistance,
            spawnHeight,  // 固定高度
            cameraPos.z + cameraForward.z * forwardDistance
        );
        
        Debug.Log($"[CherryBlossomController] 摄像机位置: {cameraPos}");
        Debug.Log($"[CherryBlossomController] 计算的樱花位置: {spawnPos}");
        
        return spawnPos;
    }
    
    /// <summary>
    /// 调整粒子密度
    /// </summary>
    private void AdjustParticleDensity()
    {
        if (allParticleSystems == null || allParticleSystems.Length == 0)
            return;
        
        foreach (var ps in allParticleSystems)
        {
            if (ps == null) continue;
            
            // 1. 增加发射速率
            var emission = ps.emission;
            var rateOverTime = emission.rateOverTime;
            
            // 获取原始发射速率并乘以倍数
            float originalRate = rateOverTime.constant;
            float newRate = originalRate * densityMultiplier;
            
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(newRate);
            
            Debug.Log($"[CherryBlossomController] {ps.name} - 发射速率: {originalRate} → {newRate}");
            
            // 2. 增加最大粒子数
            var main = ps.main;
            main.maxParticles = maxParticles;
            
            // 3. 可选：增加粒子系统的范围
            var shape = ps.shape;
            if (shape.enabled)
            {
                // 如果是锥形或球形，扩大范围
                if (shape.shapeType == ParticleSystemShapeType.Cone ||
                    shape.shapeType == ParticleSystemShapeType.ConeVolume)
                {
                    shape.radius *= 1.5f; // 扩大 50%
                    Debug.Log($"[CherryBlossomController] {ps.name} - 扩大发射范围");
                }
                else if (shape.shapeType == ParticleSystemShapeType.Box ||
                         shape.shapeType == ParticleSystemShapeType.Rectangle)
                {
                    shape.scale *= 1.5f;
                    Debug.Log($"[CherryBlossomController] {ps.name} - 扩大发射区域");
                }
            }
        }
        
        Debug.Log($"[CherryBlossomController] ✅ 樱花密度已调整为 {densityMultiplier}x");
    }

    /// <summary>
    /// 开始樱花掉落（与马路出现同时开始）
    /// </summary>
    public void StartCherryBlossom()
    {
        if (isPlaying)
        {
            Debug.LogWarning("[CherryBlossomController] 樱花已在播放中！");
            return;
        }

        if (cherryBlossomPrefab == null)
        {
            Debug.LogError("[CherryBlossomController] ❌ 未设置樱花预制体！请在 Inspector 中设置 Cherry Blossom Prefab");
            return;
        }

        Debug.Log("[CherryBlossomController] 🌸 樱花开始掉落");
        
        // 如果还没有实例化樱花预制体
        if (cherryBlossomInstance == null)
        {
            // 计算生成位置（在摄像机前方）
            Vector3 position = CalculateSpawnPosition();
            Quaternion rotation = Quaternion.identity; // 不旋转
            
            Debug.Log($"[CherryBlossomController] 樱花生成位置: {position}");
            
            cherryBlossomInstance = Instantiate(cherryBlossomPrefab, position, rotation);
            
            // 确保樱花对象激活
            cherryBlossomInstance.SetActive(true);
            
            // 查找所有粒子系统（包括子对象）
            allParticleSystems = cherryBlossomInstance.GetComponentsInChildren<ParticleSystem>();
            
            if (allParticleSystems != null && allParticleSystems.Length > 0)
            {
                Debug.Log($"[CherryBlossomController] 找到 {allParticleSystems.Length} 个粒子系统");
                particleSystem = allParticleSystems[0]; // 主粒子系统
                
                // 调整粒子密度
                AdjustParticleDensity();
            }
            else
            {
                Debug.LogError("[CherryBlossomController] ❌ 预制体中未找到粒子系统组件！");
                return;
            }
        }

        // 播放所有粒子系统
        if (allParticleSystems != null)
        {
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    // 确保发射已启用
                    var emission = ps.emission;
                    emission.enabled = true;
                    
                    // 播放粒子系统
                    ps.Play();
                    Debug.Log($"[CherryBlossomController] 播放粒子系统: {ps.gameObject.name}");
                }
            }
            isPlaying = true;
        }
        else
        {
            Debug.LogError("[CherryBlossomController] ❌ 未找到粒子系统组件！");
        }
    }

    /// <summary>
    /// 停止樱花掉落（马路完全出现后停止）
    /// </summary>
    public void StopCherryBlossom()
    {
        if (!isPlaying)
        {
            return;
        }

        Debug.Log("[CherryBlossomController] 🌸 樱花停止掉落");
        StartCoroutine(FadeOutCherryBlossom());
    }

    /// <summary>
    /// 渐进式停止樱花（不立即停止，而是逐渐减少）
    /// </summary>
    private IEnumerator FadeOutCherryBlossom()
    {
        if (allParticleSystems != null)
        {
            // 停止发射新的粒子
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.enabled = false;
                    Debug.Log($"[CherryBlossomController] 停止发射: {ps.gameObject.name}");
                }
            }

            Debug.Log($"[CherryBlossomController] 樱花淡出中 ({fadeOutDuration} 秒)...");
            
            // 等待现有粒子消散
            yield return new WaitForSeconds(fadeOutDuration);

            // 完全停止所有粒子系统
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Stop();
                    ps.Clear();
                }
            }
        }

        isPlaying = false;
        Debug.Log("[CherryBlossomController] ✅ 樱花已完全停止");
    }

    /// <summary>
    /// 重置樱花效果（用于动画重播）
    /// </summary>
    public void ResetCherryBlossom()
    {
        Debug.Log("[CherryBlossomController] 重置樱花效果");
        
        StopAllCoroutines();
        isPlaying = false;

        if (allParticleSystems != null)
        {
            foreach (var ps in allParticleSystems)
            {
                if (ps != null)
                {
                    ps.Stop();
                    ps.Clear();
                    
                    // 重新启用发射
                    var emission = ps.emission;
                    emission.enabled = true;
                }
            }
        }

        // 销毁实例
        if (cherryBlossomInstance != null)
        {
            Destroy(cherryBlossomInstance);
            cherryBlossomInstance = null;
            particleSystem = null;
            allParticleSystems = null;
        }
    }

    /// <summary>
    /// 获取樱花效果持续时间
    /// </summary>
    public float GetDuration()
    {
        return duration;
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    void OnDestroy()
    {
        if (cherryBlossomInstance != null)
        {
            Destroy(cherryBlossomInstance);
        }
    }

    // 编辑器测试
    [ContextMenu("Test Start Cherry Blossom")]
    private void TestStart()
    {
        if (Application.isPlaying)
        {
            StartCherryBlossom();
        }
    }

    [ContextMenu("Test Stop Cherry Blossom")]
    private void TestStop()
    {
        if (Application.isPlaying)
        {
            StopCherryBlossom();
        }
    }
}
