using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// 道路税款文字动画器
/// 显示 "税款=XX米道路养护" 的文字
/// </summary>
public class RoadTaxTextAnimator : MonoBehaviour
{
    [Header("UI组件")]
    [Tooltip("TextMeshPro文本组件")]
    [SerializeField] private TextMeshProUGUI taxText;
    
    [Header("文字样式")]
    [Tooltip("文字颜色")]
    [SerializeField] private Color textColor = Color.black;
    
    [Header("动画配置")]
    [Tooltip("文字显示延迟（道路稳定后多久显示）")]
    [SerializeField] private float displayDelay = 0.5f;
    
    [Tooltip("淡入持续时间")]
    [SerializeField] private float fadeInDuration = 1f;
    
    [Tooltip("文字停留时间")]
    [SerializeField] private float displayDuration = 3f;
    
    [Tooltip("淡出持续时间")]
    [SerializeField] private float fadeOutDuration = 1f;

    private CanvasGroup canvasGroup;
    private float taxAmount = 0f; // 税款金额

    void Awake()
    {
        // 自动查找TextMeshPro组件
        if (taxText == null)
        {
            taxText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // 获取或添加CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 初始隐藏
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
    }

    /// <summary>
    /// 设置税款金额并显示文字
    /// </summary>
    public void ShowTaxText(float amount)
    {
        taxAmount = amount;
        
        // 计算道路长度（2倍关系）
        float roadLength = amount * 2f;
        
        // 更新文字内容 - 格式：换行显示避免超出屏幕
        // 第一行：99元税款
        // 第二行：198米的道路养护
        if (taxText != null)
        {
            taxText.text = $"{taxAmount:F0}元税款\n={roadLength:F0}米的道路养护";
            taxText.color = textColor; // 应用文字颜色
        }
        
        // 启动动画
        StartCoroutine(TextAnimationSequence());
    }

    /// <summary>
    /// 文字动画序列
    /// </summary>
    private IEnumerator TextAnimationSequence()
    {
        // 1. 延迟显示
        yield return new WaitForSeconds(displayDelay);
        
        // 2. 淡入
        yield return StartCoroutine(FadeIn());
        
        // 3. 停留
        yield return new WaitForSeconds(displayDuration);
        
        // 4. 淡出
        yield return StartCoroutine(FadeOut());
    }

    /// <summary>
    /// 淡入动画
    /// </summary>
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            
            yield return null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        Debug.Log("[RoadTaxText] 文字已淡入显示");
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
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeOutDuration);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            
            yield return null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        Debug.Log("[RoadTaxText] 文字已淡出");
    }

    /// <summary>
    /// 获取总动画时间
    /// </summary>
    public float GetTotalDuration()
    {
        return displayDelay + fadeInDuration + displayDuration + fadeOutDuration;
    }

    /// <summary>
    /// 重置动画
    /// </summary>
    public void ResetAnimation()
    {
        StopAllCoroutines();
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
        
        if (taxText != null)
        {
            taxText.text = "";
        }
    }
}
