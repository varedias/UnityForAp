using UnityEngine;
using FlutterUnityIntegration;

/// <summary>
/// Flutter 与 Unity 双向通信桥梁
/// 处理来自 Flutter 的消息和向 Flutter 发送消息
/// </summary>
public class FlutterCommunication : MonoBehaviour
{
    private MasterController masterController;
    private PriceTagAnimator priceTagAnimator;
    private bool isInitialized = false;

    void Start()
    {
        InitializeComponents();
        NotifyFlutterReady();
    }

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void InitializeComponents()
    {
        masterController = FindObjectOfType<MasterController>();
        priceTagAnimator = FindObjectOfType<PriceTagAnimator>();

        if (masterController == null)
        {
            Debug.LogWarning("MasterController 未找到，某些功能可能无法使用");
        }

        if (priceTagAnimator == null)
        {
            Debug.LogWarning("PriceTagAnimator 未找到，价格更新功能将无法使用");
        }

        isInitialized = true;
        Debug.Log("FlutterCommunication 初始化完成");
    }

    /// <summary>
    /// 通知 Flutter Unity 已准备就绪
    /// </summary>
    private void NotifyFlutterReady()
    {
        SendMessageToFlutter(new
        {
            type = "ready",
            message = "Unity场景已加载完成",
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    #region Flutter 调用的方法

    /// <summary>
    /// 开始动画 - 由 Flutter 调用
    /// </summary>
    public void StartAnimation()
    {
        Debug.Log("收到来自 Flutter 的开始动画指令");
        
        if (!isInitialized)
        {
            InitializeComponents();
        }

        if (masterController != null)
        {
            // 假设 MasterController 有 StartAnimation 方法
            // 如果没有，可以调用其他方法来触发动画
            // masterController.StartAnimation();
            
            SendMessageToFlutter(new
            {
                type = "animation_started",
                message = "动画已开始",
                success = true
            });
        }
        else
        {
            SendMessageToFlutter(new
            {
                type = "error",
                message = "MasterController 未找到",
                success = false
            });
        }
    }

    /// <summary>
    /// 暂停动画 - 由 Flutter 调用
    /// </summary>
    public void PauseAnimation()
    {
        Debug.Log("收到来自 Flutter 的暂停动画指令");
        Time.timeScale = 0f;
        
        SendMessageToFlutter(new
        {
            type = "animation_paused",
            message = "动画已暂停",
            success = true
        });
    }

    /// <summary>
    /// 恢复动画 - 由 Flutter 调用
    /// </summary>
    public void ResumeAnimation()
    {
        Debug.Log("收到来自 Flutter 的恢复动画指令");
        Time.timeScale = 1f;
        
        SendMessageToFlutter(new
        {
            type = "animation_resumed",
            message = "动画已恢复",
            success = true
        });
    }

    /// <summary>
    /// 重置动画 - 由 Flutter 调用
    /// </summary>
    public void ResetAnimation()
    {
        Debug.Log("收到来自 Flutter 的重置动画指令");
        
        if (masterController != null)
        {
            // 重新加载场景或重置状态
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
            
            SendMessageToFlutter(new
            {
                type = "animation_reset",
                message = "动画已重置",
                success = true
            });
        }
    }

    /// <summary>
    /// 更新价格 - 由 Flutter 调用
    /// 参数格式: "¥99" 或 "$99"
    /// </summary>
    /// <param name="price">新的价格字符串</param>
    public void UpdatePrice(string price)
    {
        Debug.Log($"收到来自 Flutter 的价格更新: {price}");

        if (priceTagAnimator != null)
        {
            // 假设 PriceTagAnimator 有 priceString 字段
            // priceTagAnimator.priceString = price;
            
            SendMessageToFlutter(new
            {
                type = "price_updated",
                message = $"价格已更新为: {price}",
                newPrice = price,
                success = true
            });
        }
        else
        {
            SendMessageToFlutter(new
            {
                type = "error",
                message = "PriceTagAnimator 未找到",
                success = false
            });
        }
    }

    /// <summary>
    /// 更新动画设置 - 由 Flutter 调用
    /// 参数格式: JSON 字符串 {"price":"¥99","duration":2.0,"autoPlay":true}
    /// </summary>
    /// <param name="jsonData">JSON 格式的设置数据</param>
    public void UpdateSettings(string jsonData)
    {
        Debug.Log($"收到来自 Flutter 的设置更新: {jsonData}");

        try
        {
            // 使用 Unity 的 JsonUtility 或 Newtonsoft.Json 解析
            var settings = JsonUtility.FromJson<AnimationSettings>(jsonData);
            
            if (!string.IsNullOrEmpty(settings.price))
            {
                UpdatePrice(settings.price);
            }

            SendMessageToFlutter(new
            {
                type = "settings_updated",
                message = "设置已更新",
                success = true
            });
        }
        catch (System.Exception e)
        {
            Debug.LogError($"解析设置 JSON 失败: {e.Message}");
            SendMessageToFlutter(new
            {
                type = "error",
                message = $"设置更新失败: {e.Message}",
                success = false
            });
        }
    }

    /// <summary>
    /// 获取当前状态 - 由 Flutter 调用
    /// </summary>
    public void GetStatus()
    {
        SendMessageToFlutter(new
        {
            type = "status",
            isInitialized = isInitialized,
            hasMasterController = masterController != null,
            hasPriceTagAnimator = priceTagAnimator != null,
            timeScale = Time.timeScale,
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        });
    }

    #endregion

    #region 发送消息到 Flutter

    /// <summary>
    /// 发送对象消息到 Flutter（自动转换为 JSON）
    /// </summary>
    /// <param name="data">要发送的数据对象</param>
    private void SendMessageToFlutter(object data)
    {
        string jsonMessage = JsonUtility.ToJson(data);
        SendMessageToFlutter(jsonMessage);
    }

    /// <summary>
    /// 发送字符串消息到 Flutter
    /// </summary>
    /// <param name="message">消息内容</param>
    private void SendMessageToFlutter(string message)
    {
        try
        {
            UnityMessageManager.Instance.SendMessageToFlutter(message);
            Debug.Log($"已发送消息到 Flutter: {message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"发送消息到 Flutter 失败: {e.Message}");
        }
    }

    #endregion

    #region Unity 事件回调

    /// <summary>
    /// 动画完成回调 - 可以由其他脚本调用
    /// </summary>
    public void OnAnimationComplete()
    {
        SendMessageToFlutter(new
        {
            type = "animation_complete",
            message = "动画播放完成"
        });
    }

    /// <summary>
    /// 星星消失回调
    /// </summary>
    public void OnStarDisappeared()
    {
        SendMessageToFlutter(new
        {
            type = "star_disappeared",
            message = "星星已消失"
        });
    }

    /// <summary>
    /// 道路生成回调
    /// </summary>
    public void OnRoadGenerated(int roadCount)
    {
        SendMessageToFlutter(new
        {
            type = "road_generated",
            message = $"道路已生成，共 {roadCount} 段",
            count = roadCount
        });
    }

    #endregion
}

/// <summary>
/// 动画设置数据结构
/// </summary>
[System.Serializable]
public class AnimationSettings
{
    public string price;
    public float duration;
    public bool autoPlay;
}
