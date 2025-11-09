using UnityEngine;
using System.Collections;

/// <summary>
/// 主控制器 - 控制整个动画流程
/// 流程: 启动 → UI价签显示 → UI淡出 → 星星掉落 → 马路延伸
/// </summary>
public class MasterController : MonoBehaviour
{
    [Header("引用组件")]
    [SerializeField] private PriceTagAnimator priceTagAnimator;
    [SerializeField] private StarSpawner starSpawner;
    [SerializeField] private RoadManager roadManager;
    [SerializeField] private RoadTaxTextAnimator roadTaxTextAnimator;
    [SerializeField] private SimpleFallingLeavesController fallingLeavesController;

    [Header("动画配置")]
    [Tooltip("是否在启动时自动播放动画")]
    [SerializeField] private bool autoPlayOnStart = true;
    
    [Tooltip("动画开始前的延迟时间（秒）")]
    [SerializeField] private float startDelay = 0.5f;

    [Header("网页交互")]
    [Tooltip("目标价格 - 从网页接收")]
    private float targetPrice = 0f;

    private bool isAnimationPlaying = false;

    void Start()
    {
        // 自动查找组件（如果未手动分配）
        if (priceTagAnimator == null)
            priceTagAnimator = FindObjectOfType<PriceTagAnimator>();
        
        if (starSpawner == null)
            starSpawner = FindObjectOfType<StarSpawner>();
        
        if (roadManager == null)
            roadManager = FindObjectOfType<RoadManager>();
        
        if (roadTaxTextAnimator == null)
            roadTaxTextAnimator = FindObjectOfType<RoadTaxTextAnimator>();
        
        if (fallingLeavesController == null)
            fallingLeavesController = FindObjectOfType<SimpleFallingLeavesController>();

        // 验证所有组件
        if (!ValidateComponents())
        {
            Debug.LogError("[MasterController] 缺少必要组件，无法启动动画！");
            return;
        }

        // 自动播放
        if (autoPlayOnStart)
        {
            StartAnimation();
        }
    }

    /// <summary>
    /// 验证所有必要组件是否存在
    /// </summary>
    private bool ValidateComponents()
    {
        bool isValid = true;

        if (priceTagAnimator == null)
        {
            Debug.LogError("[MasterController] 未找到 PriceTagAnimator 组件！");
            isValid = false;
        }

        if (starSpawner == null)
        {
            Debug.LogError("[MasterController] 未找到 StarSpawner 组件！");
            isValid = false;
        }

        if (roadManager == null)
        {
            Debug.LogError("[MasterController] 未找到 RoadManager 组件！");
            isValid = false;
        }

        return isValid;
    }

    /// <summary>
    /// 启动整个动画流程
    /// </summary>
    public void StartAnimation()
    {
        if (isAnimationPlaying)
        {
            Debug.LogWarning("[MasterController] 动画已在播放中，请勿重复调用！");
            return;
        }

        StartCoroutine(AnimationSequence());
    }

    /// <summary>
    /// 动画流程序列
    /// </summary>
    private IEnumerator AnimationSequence()
    {
        isAnimationPlaying = true;
        Debug.Log("[MasterController] ========== 动画流程开始 ==========");

        // 1. 延迟启动
        if (startDelay > 0)
        {
            Debug.Log($"[MasterController] 等待 {startDelay} 秒后开始...");
            yield return new WaitForSeconds(startDelay);
        }

        // 2. 启动价签UI动画
        Debug.Log("[MasterController] 步骤 1: 启动价签UI动画");
        priceTagAnimator.PlayAnimation();

        // 3. 等待价签动画播放完毕
        float priceTagDuration = priceTagAnimator.GetTotalDuration();
        Debug.Log($"[MasterController] 等待价签动画完成 ({priceTagDuration} 秒)...");
        yield return new WaitForSeconds(priceTagDuration);

        // 4. 价签消失后，启动落叶效果
        Debug.Log("[MasterController] 步骤 2: 启动落叶效果");
        if (fallingLeavesController != null)
        {
            fallingLeavesController.StartFallingLeaves();
        }
        
        // 等待 5 秒让落叶单独下落
        Debug.Log("[MasterController] 等待 5 秒让落叶下落...");
        yield return new WaitForSeconds(5f);
        
        // 5. 开始淡出所有落叶（2秒淡出）
        Debug.Log("[MasterController] 步骤 3: 开始淡出落叶");
        if (fallingLeavesController != null)
        {
            fallingLeavesController.FadeOutLeaves(2f);
        }
        
        // 等待淡出完成（2秒淡出 + 0.5秒缓冲）
        yield return new WaitForSeconds(2.5f);
        
        // 6. 初始化道路系统
        Debug.Log("[MasterController] 步骤 4: 初始化道路系统");
        roadManager.Initialize();
        
        // 7. 等待道路渐变完成
        float roadFadeInDuration = 2f;
        Debug.Log($"[MasterController] 等待道路渐变完成 ({roadFadeInDuration} 秒)...");
        yield return new WaitForSeconds(roadFadeInDuration);
        
        // 8. 显示税款文字
        if (roadTaxTextAnimator != null)
        {
            Debug.Log("[MasterController] 步骤 3: 显示税款文字");
            // 获取价签的金额
            float taxAmount = priceTagAnimator.GetPriceAmount();
            roadTaxTextAnimator.ShowTaxText(taxAmount);
        }

        Debug.Log("[MasterController] ========== 动画流程启动完成 ==========");
    }

    /// <summary>
    /// 重置动画（用于测试）
    /// </summary>
    public void ResetAnimation()
    {
        Debug.Log("[MasterController] 重置动画状态");
        
        StopAllCoroutines();
        isAnimationPlaying = false;

        // 重置各个组件
        if (priceTagAnimator != null)
            priceTagAnimator.ResetAnimation();
        
        if (starSpawner != null)
            starSpawner.StopSpawning();
        
        if (fallingLeavesController != null)
            fallingLeavesController.ResetFallingLeaves();
        
        if (roadManager != null)
            roadManager.Reset();
    }

    /// <summary>
    /// 手动触发道路延伸（由星星消失时调用）
    /// </summary>
    public void OnStarDisappeared()
    {
        if (roadManager != null)
        {
            roadManager.ExtendRoad();
        }
    }

    // 编辑器测试按钮
    void Update()
    {
        // 按下空格键重新开始动画（仅用于测试）
        // 使用 InputManager 兼容新旧输入系统
        if (InputManager.GetSpaceKeyDown())
        {
            Debug.Log("[MasterController] 按下空格键 - 重新启动动画");
            ResetAnimation();
            StartAnimation();
        }
    }

    // ========== 网页交互接口 ==========
    
    /// <summary>
    /// 网页调用此方法设置价格（必须是这个名字！）
    /// </summary>
    public void SetPrice(string price)
    {
        Debug.Log($"[MasterController] 🌐 收到网页传来的价格: {price}");
        
        if (float.TryParse(price, out float parsedPrice))
        {
            targetPrice = parsedPrice;
            
            // 更新 PriceTagAnimator 的价格
            if (priceTagAnimator != null)
            {
                priceTagAnimator.SetPrice(parsedPrice);
            }
            
            Debug.Log($"[MasterController] ✅ 价格已设置: ¥{targetPrice:F2}");
        }
        else
        {
            Debug.LogError($"[MasterController] ❌ 价格格式错误: {price}");
        }
    }
    
    /// <summary>
    /// 网页调用此方法播放动画（必须是这个名字！）
    /// </summary>
    public void PlayAnimation()
    {
        Debug.Log($"[MasterController] 🌐 收到网页指令: 播放动画 (价格: ¥{targetPrice:F2})");
        StartAnimation();
    }
}
