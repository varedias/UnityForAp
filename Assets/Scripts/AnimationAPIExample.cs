using UnityEngine;
using System.Collections;

/// <summary>
/// API 使用示例 - 展示如何通过代码控制动画系统
/// </summary>
public class AnimationAPIExample : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private MasterController masterController;
    [SerializeField] private PriceTagAnimator priceTagAnimator;
    [SerializeField] private StarSpawner starSpawner;
    [SerializeField] private RoadManager roadManager;
    [SerializeField] private CameraController cameraController;

    void Start()
    {
        // 自动查找组件
        if (masterController == null)
            masterController = FindObjectOfType<MasterController>();
        
        if (priceTagAnimator == null)
            priceTagAnimator = FindObjectOfType<PriceTagAnimator>();
        
        if (starSpawner == null)
            starSpawner = FindObjectOfType<StarSpawner>();
        
        if (roadManager == null)
            roadManager = FindObjectOfType<RoadManager>();
        
        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();
    }

    // ==================== 基础控制示例 ====================

    /// <summary>
    /// 示例1: 播放完整动画
    /// </summary>
    [ContextMenu("Example 1: Play Full Animation")]
    public void Example1_PlayFullAnimation()
    {
        if (masterController != null)
        {
            masterController.StartAnimation();
            Debug.Log("[Example] 完整动画已启动");
        }
    }

    /// <summary>
    /// 示例2: 重置并重新播放
    /// </summary>
    [ContextMenu("Example 2: Reset And Replay")]
    public void Example2_ResetAndReplay()
    {
        if (masterController != null)
        {
            masterController.ResetAnimation();
            StartCoroutine(DelayedStart(1f));
        }
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        masterController.StartAnimation();
        Debug.Log("[Example] 动画已重新启动");
    }

    // ==================== 价签控制示例 ====================

    /// <summary>
    /// 示例3: 更改价格并播放动画
    /// </summary>
    [ContextMenu("Example 3: Change Price")]
    public void Example3_ChangePrice()
    {
        if (priceTagAnimator != null)
        {
            // 动态设置价格
            string[] prices = { "¥99", "¥199", "¥299", "¥9.9" };
            string randomPrice = prices[Random.Range(0, prices.Length)];
            
            priceTagAnimator.UpdatePrice(randomPrice);
            priceTagAnimator.ResetAnimation();
            priceTagAnimator.PlayAnimation();
            
            Debug.Log($"[Example] 价格已更改为: {randomPrice}");
        }
    }

    /// <summary>
    /// 示例4: 自定义价签动画
    /// </summary>
    public void Example4_CustomPriceAnimation(string price, float displayTime)
    {
        if (priceTagAnimator != null)
        {
            priceTagAnimator.UpdatePrice(price);
            // 注意: 需要修改PriceTagAnimator以支持运行时更改displayDuration
            priceTagAnimator.PlayAnimation();
        }
    }

    // ==================== 星星控制示例 ====================

    /// <summary>
    /// 示例5: 手动生成单颗星星
    /// </summary>
    [ContextMenu("Example 5: Spawn Single Star")]
    public void Example5_SpawnSingleStar()
    {
        if (starSpawner != null)
        {
            starSpawner.SpawnSingleStar();
            Debug.Log("[Example] 已生成单颗星星");
        }
    }

    /// <summary>
    /// 示例6: 控制星星生成
    /// </summary>
    [ContextMenu("Example 6: Control Star Spawning")]
    public void Example6_ControlStarSpawning()
    {
        if (starSpawner != null)
        {
            // 开始生成
            starSpawner.StartSpawning();
            
            // 5秒后停止
            StartCoroutine(StopSpawningAfterDelay(5f));
        }
    }

    private IEnumerator StopSpawningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (starSpawner != null)
        {
            starSpawner.StopSpawning();
            Debug.Log("[Example] 星星生成已停止");
        }
    }

    // ==================== 道路控制示例 ====================

    /// <summary>
    /// 示例7: 手动延伸道路
    /// </summary>
    [ContextMenu("Example 7: Extend Road Manually")]
    public void Example7_ExtendRoadManually()
    {
        if (roadManager != null)
        {
            roadManager.ExtendRoad();
            Debug.Log("[Example] 道路已手动延伸");
        }
    }

    /// <summary>
    /// 示例8: 连续延伸多段道路
    /// </summary>
    [ContextMenu("Example 8: Extend Multiple Road Segments")]
    public void Example8_ExtendMultipleSegments()
    {
        if (roadManager != null)
        {
            StartCoroutine(ExtendRoadMultipleTimes(5, 0.5f));
        }
    }

    private IEnumerator ExtendRoadMultipleTimes(int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            roadManager.ExtendRoad();
            Debug.Log($"[Example] 延伸第 {i + 1} 段道路");
            yield return new WaitForSeconds(interval);
        }
    }

    // ==================== 摄像机控制示例 ====================

    /// <summary>
    /// 示例9: 切换摄像机模式
    /// </summary>
    [ContextMenu("Example 9: Toggle Camera Mode")]
    public void Example9_ToggleCameraMode()
    {
        if (cameraController != null)
        {
            // 启用道路跟随
            cameraController.SetFollowRoad(true);
            Debug.Log("[Example] 摄像机已切换到跟随模式");
        }
    }

    /// <summary>
    /// 示例10: 设置自定义摄像机位置
    /// </summary>
    [ContextMenu("Example 10: Set Custom Camera Position")]
    public void Example10_SetCustomCameraPosition()
    {
        if (cameraController != null)
        {
            Vector3 newPos = new Vector3(0, 15, -8);
            Vector3 newRot = new Vector3(60, 0, 0);
            
            cameraController.SetCameraPosition(newPos, newRot);
            Debug.Log($"[Example] 摄像机位置已设置为: {newPos}, 旋转: {newRot}");
        }
    }

    // ==================== 组合示例 ====================

    /// <summary>
    /// 示例11: 自定义动画序列
    /// </summary>
    [ContextMenu("Example 11: Custom Animation Sequence")]
    public void Example11_CustomSequence()
    {
        StartCoroutine(CustomAnimationSequence());
    }

    private IEnumerator CustomAnimationSequence()
    {
        Debug.Log("[Example] 开始自定义动画序列");

        // 1. 显示价格
        if (priceTagAnimator != null)
        {
            priceTagAnimator.UpdatePrice("¥限时优惠");
            priceTagAnimator.PlayAnimation();
            yield return new WaitForSeconds(priceTagAnimator.GetTotalDuration());
        }

        // 2. 快速生成3颗星星
        if (starSpawner != null)
        {
            for (int i = 0; i < 3; i++)
            {
                starSpawner.SpawnSingleStar();
                yield return new WaitForSeconds(1f);
            }
        }

        // 3. 延伸道路
        if (roadManager != null)
        {
            for (int i = 0; i < 3; i++)
            {
                roadManager.ExtendRoad();
                yield return new WaitForSeconds(0.8f);
            }
        }

        Debug.Log("[Example] 自定义动画序列完成");
    }

    // ==================== 数据驱动示例 ====================

    /// <summary>
    /// 示例12: 从数据驱动动画
    /// </summary>
    public void Example12_DataDrivenAnimation(AnimationData data)
    {
        StartCoroutine(DataDrivenSequence(data));
    }

    private IEnumerator DataDrivenSequence(AnimationData data)
    {
        // 设置价格
        if (priceTagAnimator != null && !string.IsNullOrEmpty(data.price))
        {
            priceTagAnimator.UpdatePrice(data.price);
            priceTagAnimator.PlayAnimation();
            yield return new WaitForSeconds(priceTagAnimator.GetTotalDuration());
        }

        // 生成指定数量的星星
        if (starSpawner != null)
        {
            for (int i = 0; i < data.starCount; i++)
            {
                starSpawner.SpawnSingleStar();
                yield return new WaitForSeconds(data.starInterval);
            }
        }

        Debug.Log($"[Example] 数据驱动动画完成 - 价格:{data.price}, 星星数:{data.starCount}");
    }

    /// <summary>
    /// 动画数据结构
    /// </summary>
    [System.Serializable]
    public class AnimationData
    {
        public string price = "¥99";
        public int starCount = 5;
        public float starInterval = 2f;
        public int roadSegments = 5;
    }

    // ==================== 测试所有功能 ====================

    /// <summary>
    /// 测试所有功能
    /// </summary>
    [ContextMenu("Test All Features")]
    public void TestAllFeatures()
    {
        Debug.Log("========== 开始测试所有功能 ==========");
        StartCoroutine(TestAllFeaturesRoutine());
    }

    private IEnumerator TestAllFeaturesRoutine()
    {
        yield return new WaitForSeconds(1f);
        Example3_ChangePrice();
        
        yield return new WaitForSeconds(3f);
        Example5_SpawnSingleStar();
        
        yield return new WaitForSeconds(2f);
        Example7_ExtendRoadManually();
        
        yield return new WaitForSeconds(2f);
        Example9_ToggleCameraMode();
        
        Debug.Log("========== 功能测试完成 ==========");
    }
}
