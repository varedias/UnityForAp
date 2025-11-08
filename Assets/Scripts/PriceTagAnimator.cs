using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// 价签UI动画控制器
/// 功能: 显示价格 → 停留 → 向上移动并渐隐消失
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PriceTagAnimator : MonoBehaviour
{
    [Header("UI组件引用")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("价格配置")]
    [Tooltip("显示的价格文本")]
    [SerializeField] private string priceString = "¥99.00";
    
    [Tooltip("文字颜色")]
    [SerializeField] private Color textColor = Color.black;
    
    [Tooltip("实际价格数值（从网页接收）")]
    private float priceValue = 99f;

    [Header("动画参数")]
    [Tooltip("初始位置偏移（相对于屏幕中央，Y轴负值表示偏下）")]
    [SerializeField] private Vector2 initialPosition = new Vector2(0, -200f);
    
    [Tooltip("停留时间（秒）")]
    [SerializeField] private float displayDuration = 2.0f;
    
    [Tooltip("向上移动距离")]
    [SerializeField] private float moveUpDistance = 300f;
    
    [Tooltip("移动和淡出的持续时间（秒）")]
    [SerializeField] private float fadeOutDuration = 1.5f;

    [Header("动画曲线")]
    [Tooltip("移动曲线（控制上升速度）")]
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Tooltip("透明度渐变曲线")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isAnimating = false;

    void Awake()
    {
        // 自动查找组件
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (priceText == null)
            priceText = GetComponentInChildren<TextMeshProUGUI>();

        rectTransform = GetComponent<RectTransform>();
        
        // 初始化UI
        InitializeUI();
    }

    /// <summary>
    /// 初始化UI状态
    /// </summary>
    private void InitializeUI()
    {
        // 设置价格文本和颜色
        if (priceText != null)
        {
            priceText.text = priceString;
            priceText.color = textColor;
        }

        // 设置初始位置
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = initialPosition;
            startPosition = initialPosition;
            endPosition = initialPosition + new Vector2(0, moveUpDistance);
        }

        // 设置初始透明度（隐藏）
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation()
    {
        if (isAnimating)
        {
            Debug.LogWarning("[PriceTagAnimator] 动画已在播放中！");
            return;
        }

        StartCoroutine(AnimationRoutine());
    }

    /// <summary>
    /// 动画协程
    /// </summary>
    private IEnumerator AnimationRoutine()
    {
        isAnimating = true;
        Debug.Log("[PriceTagAnimator] 开始价签动画");

        // 阶段 1: 淡入显示
        Debug.Log("[PriceTagAnimator] 阶段1: 淡入显示");
        yield return StartCoroutine(FadeIn(0.5f));

        // 阶段 2: 停留显示
        Debug.Log($"[PriceTagAnimator] 阶段2: 停留 {displayDuration} 秒");
        yield return new WaitForSeconds(displayDuration);

        // 阶段 3: 向上移动并淡出
        Debug.Log($"[PriceTagAnimator] 阶段3: 向上移动并淡出 ({fadeOutDuration} 秒)");
        yield return StartCoroutine(MoveUpAndFadeOut());

        Debug.Log("[PriceTagAnimator] 动画完成");
        isAnimating = false;
    }

    /// <summary>
    /// 淡入效果
    /// </summary>
    private IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = progress;
            }
            
            yield return null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    /// <summary>
    /// 向上移动并淡出
    /// </summary>
    private IEnumerator MoveUpAndFadeOut()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeOutDuration);
            
            // 根据曲线计算位置
            float moveProgress = movementCurve.Evaluate(progress);
            Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, moveProgress);
            
            // 根据曲线计算透明度
            float fadeProgress = fadeCurve.Evaluate(progress);
            
            // 应用变换
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = currentPosition;
            }
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = fadeProgress;
            }
            
            yield return null;
        }
        
        // 确保最终状态
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = endPosition;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// 获取动画总时长
    /// </summary>
    public float GetTotalDuration()
    {
        return 0.5f + displayDuration + fadeOutDuration; // 淡入 + 停留 + 淡出
    }
    
    /// <summary>
    /// 获取价格金额（提取数字部分）
    /// </summary>
    public float GetPriceAmount()
    {
        // 使用正则表达式提取第一个数字（支持整数和小数）
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(priceString, @"[¥$]?\s*(\d+\.?\d*)");
        
        if (match.Success && float.TryParse(match.Groups[1].Value, out float amount))
        {
            return amount;
        }
        
        Debug.LogWarning($"[PriceTagAnimator] 无法从 '{priceString}' 中提取数字，返回默认值0");
        return 0f;
    }

    /// <summary>
    /// 重置动画
    /// </summary>
    public void ResetAnimation()
    {
        Debug.Log("[PriceTagAnimator] 重置动画状态");
        
        StopAllCoroutines();
        isAnimating = false;
        InitializeUI();
    }

    /// <summary>
    /// 更新价格文本（运行时调用）
    /// </summary>
    public void UpdatePrice(string newPrice)
    {
        priceString = newPrice;
        if (priceText != null)
        {
            priceText.text = newPrice;
        }
        Debug.Log($"[PriceTagAnimator] 价格已更新为: {newPrice}");
    }
    
    /// <summary>
    /// 设置价格数值（从网页接收）
    /// </summary>
    public void SetPrice(float price)
    {
        priceValue = price;
        priceString = $"¥{price:F2}";  // 只显示价格，不要多余文字
        
        if (priceText != null)
        {
            priceText.text = priceString;
        }
        
        Debug.Log($"[PriceTagAnimator] 💰 价格已设置为: ¥{price:F2}");
    }

    /// <summary>
    /// 预览效果（仅编辑器中使用）
    /// </summary>
    [ContextMenu("Preview Animation")]
    private void PreviewAnimation()
    {
        if (Application.isPlaying)
        {
            ResetAnimation();
            PlayAnimation();
        }
        else
        {
            Debug.LogWarning("请在运行时预览动画！");
        }
    }
}
