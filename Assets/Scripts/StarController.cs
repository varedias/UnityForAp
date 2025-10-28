using UnityEngine;
using System.Collections;

/// <summary>
/// 星星控制器 - 控制单个星星的掉落、落地、淡出动画
/// </summary>
public class StarController : MonoBehaviour
{
    [Header("掉落参数")]
    [Tooltip("掉落高度（相对于地面）")]
    [SerializeField] private float dropHeight = 10f;
    
    [Tooltip("掉落持续时间（秒）")]
    [SerializeField] private float fallDuration = 1.5f;
    
    [Tooltip("掉落曲线（控制掉落速度变化）")]
    [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("落地参数")]
    [Tooltip("落地后停留时间（秒）")]
    [SerializeField] private float landDuration = 0.5f;
    
    [Tooltip("落地时的缩放效果")]
    [SerializeField] private float landScaleBounce = 1.2f;
    
    [Tooltip("落地缩放动画时间")]
    [SerializeField] private float landScaleDuration = 0.3f;

    [Header("淡出参数")]
    [Tooltip("淡出持续时间（秒）")]
    [SerializeField] private float fadeOutDuration = 1.0f;
    
    [Tooltip("淡出曲线")]
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("地面参数")]
    [Tooltip("地面高度（Y轴）")]
    [SerializeField] private float groundLevel = 0f;

    // 内部变量
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Material starMaterial;
    private Renderer starRenderer;
    private MasterController masterController;
    private bool isAnimating = false;

    // 颜色属性ID（优化性能）
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    void Awake()
    {
        // 获取渲染器和材质
        starRenderer = GetComponent<Renderer>();
        if (starRenderer != null)
        {
            // 创建材质实例（避免修改共享材质）
            starMaterial = starRenderer.material;
        }
        else
        {
            // 尝试获取子对象的渲染器
            starRenderer = GetComponentInChildren<Renderer>();
            if (starRenderer != null)
            {
                starMaterial = starRenderer.material;
            }
        }

        // 查找主控制器
        masterController = FindObjectOfType<MasterController>();
    }

    /// <summary>
    /// 初始化星星并开始动画
    /// </summary>
    public void Initialize(Vector3 spawnPosition)
    {
        // 设置起始位置（高空）
        startPosition = spawnPosition + new Vector3(0, dropHeight, 0);
        targetPosition = new Vector3(spawnPosition.x, groundLevel, spawnPosition.z);
        
        transform.position = startPosition;
        transform.localScale = Vector3.one;

        // 确保初始可见
        SetAlpha(1f);

        // 开始动画序列
        StartCoroutine(StarAnimationSequence());
    }

    /// <summary>
    /// 星星动画序列
    /// </summary>
    private IEnumerator StarAnimationSequence()
    {
        isAnimating = true;
        
        // 阶段1: 从高空掉落
        Debug.Log($"[StarController] {gameObject.name} - 阶段1: 掉落");
        yield return StartCoroutine(FallDown());

        // 阶段2: 落地效果（轻微弹跳）
        Debug.Log($"[StarController] {gameObject.name} - 阶段2: 落地");
        yield return StartCoroutine(LandEffect());

        // 阶段3: 停留
        Debug.Log($"[StarController] {gameObject.name} - 阶段3: 停留 {landDuration} 秒");
        yield return new WaitForSeconds(landDuration);

        // 阶段4: 淡出消失
        Debug.Log($"[StarController] {gameObject.name} - 阶段4: 淡出");
        yield return StartCoroutine(FadeOut());

        // 触发事件：通知主控制器星星已消失
        OnStarDisappeared();

        // 销毁对象
        Destroy(gameObject);
    }

    /// <summary>
    /// 掉落动画
    /// </summary>
    private IEnumerator FallDown()
    {
        float elapsed = 0f;
        
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fallDuration);
            
            // 根据曲线计算位置
            float curveValue = fallCurve.Evaluate(progress);
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
            
            transform.position = currentPosition;
            
            // 添加旋转效果（可选）
            transform.Rotate(Vector3.forward, 360f * Time.deltaTime);
            
            yield return null;
        }
        
        // 确保最终位置准确
        transform.position = targetPosition;
    }

    /// <summary>
    /// 落地效果（缩放弹跳）
    /// </summary>
    private IEnumerator LandEffect()
    {
        float elapsed = 0f;
        Vector3 originalScale = Vector3.one;
        Vector3 bounceScale = originalScale * landScaleBounce;
        
        // 放大
        while (elapsed < landScaleDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / (landScaleDuration / 2));
            
            transform.localScale = Vector3.Lerp(originalScale, bounceScale, progress);
            
            yield return null;
        }
        
        elapsed = 0f;
        
        // 缩回
        while (elapsed < landScaleDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / (landScaleDuration / 2));
            
            transform.localScale = Vector3.Lerp(bounceScale, originalScale, progress);
            
            yield return null;
        }
        
        transform.localScale = originalScale;
    }

    /// <summary>
    /// 淡出动画
    /// </summary>
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeOutDuration);
            
            // 根据曲线计算透明度
            float alpha = fadeOutCurve.Evaluate(progress);
            SetAlpha(alpha);
            
            yield return null;
        }
        
        SetAlpha(0f);
    }

    /// <summary>
    /// 设置材质透明度
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (starMaterial == null) return;

        Color color = starMaterial.color;
        
        // 尝试不同的材质属性
        if (starMaterial.HasProperty(BaseColorProperty))
        {
            color = starMaterial.GetColor(BaseColorProperty);
            color.a = alpha;
            starMaterial.SetColor(BaseColorProperty, color);
        }
        else if (starMaterial.HasProperty(ColorProperty))
        {
            color = starMaterial.GetColor(ColorProperty);
            color.a = alpha;
            starMaterial.SetColor(ColorProperty, color);
        }
        else
        {
            // 默认使用材质的颜色
            color.a = alpha;
            starMaterial.color = color;
        }
    }

    /// <summary>
    /// 星星消失时触发的事件
    /// </summary>
    private void OnStarDisappeared()
    {
        Debug.Log($"[StarController] {gameObject.name} - 已消失，触发道路延伸事件");
        
        // 通知主控制器
        if (masterController != null)
        {
            masterController.OnStarDisappeared();
        }
        else
        {
            Debug.LogWarning("[StarController] 未找到 MasterController，无法触发道路延伸！");
        }
    }

    /// <summary>
    /// 获取动画总时长
    /// </summary>
    public float GetTotalDuration()
    {
        return fallDuration + landScaleDuration + landDuration + fadeOutDuration;
    }

    void OnDestroy()
    {
        // 清理材质实例
        if (starMaterial != null)
        {
            Destroy(starMaterial);
        }
    }
}
