using UnityEngine;

/// <summary>
/// 录制助手 - 帮助设置和控制动画录制
/// </summary>
public class RecordingHelper : MonoBehaviour
{
    [Header("录制设置")]
    [Tooltip("录制帧率")]
    [SerializeField] private int targetFrameRate = 60;
    
    [Tooltip("录制开始前的延迟（秒）")]
    [SerializeField] private float recordingStartDelay = 1f;
    
    [Tooltip("是否在录制时固定时间步长")]
    [SerializeField] private bool useFixedDeltaTime = true;

    [Header("信息")]
    [SerializeField] private bool isRecording = false;

    private float originalFixedDeltaTime;
    private int originalTargetFrameRate;

    void Awake()
    {
        // 保存原始设置
        originalFixedDeltaTime = Time.fixedDeltaTime;
        originalTargetFrameRate = Application.targetFrameRate;
    }

    /// <summary>
    /// 开始录制准备
    /// </summary>
    [ContextMenu("Prepare For Recording")]
    public void PrepareForRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("[RecordingHelper] 已经在录制模式中！");
            return;
        }

        Debug.Log("[RecordingHelper] 准备录制...");

        // 设置目标帧率
        Application.targetFrameRate = targetFrameRate;

        // 设置固定时间步长（确保录制平滑）
        if (useFixedDeltaTime)
        {
            Time.fixedDeltaTime = 1f / targetFrameRate;
        }

        isRecording = true;

        Debug.Log($"[RecordingHelper] 录制设置完成 - 帧率: {targetFrameRate} FPS");
        Debug.Log("[RecordingHelper] 提示: 使用 Unity Recorder 包进行录制");
        Debug.Log("[RecordingHelper] Window > General > Recorder > Recorder Window");
    }

    /// <summary>
    /// 停止录制
    /// </summary>
    [ContextMenu("Stop Recording")]
    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("[RecordingHelper] 未在录制模式中！");
            return;
        }

        Debug.Log("[RecordingHelper] 停止录制，恢复原始设置...");

        // 恢复原始设置
        Application.targetFrameRate = originalTargetFrameRate;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        isRecording = false;

        Debug.Log("[RecordingHelper] 已恢复原始设置");
    }

    /// <summary>
    /// 自动录制流程
    /// </summary>
    [ContextMenu("Auto Record Animation")]
    public void AutoRecordAnimation()
    {
        StartCoroutine(AutoRecordRoutine());
    }

    private System.Collections.IEnumerator AutoRecordRoutine()
    {
        Debug.Log("[RecordingHelper] 开始自动录制流程...");

        // 准备录制
        PrepareForRecording();

        // 等待延迟
        yield return new WaitForSeconds(recordingStartDelay);

        // 查找主控制器
        MasterController masterController = FindObjectOfType<MasterController>();
        if (masterController != null)
        {
            Debug.Log("[RecordingHelper] 启动动画...");
            masterController.ResetAnimation();
            masterController.StartAnimation();
        }
        else
        {
            Debug.LogError("[RecordingHelper] 未找到 MasterController！");
        }

        Debug.Log("[RecordingHelper] 动画已启动，请手动开始Unity Recorder录制");
    }

    void OnDestroy()
    {
        // 确保恢复设置
        if (isRecording)
        {
            StopRecording();
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// 显示录制提示
    /// </summary>
    [ContextMenu("Show Recording Guide")]
    private void ShowRecordingGuide()
    {
        string guide = 
            "=== 录制动画指南 ===\n\n" +
            "1. 安装 Unity Recorder 包\n" +
            "   Window > Package Manager > Unity Recorder\n\n" +
            "2. 打开 Recorder 窗口\n" +
            "   Window > General > Recorder > Recorder Window\n\n" +
            "3. 添加录制器\n" +
            "   点击 'Add Recorder' > Movie\n\n" +
            "4. 配置录制设置\n" +
            "   - 输出格式: MP4\n" +
            "   - 质量: High\n" +
            "   - 帧率: " + targetFrameRate + " FPS\n\n" +
            "5. 在本组件上点击 'Prepare For Recording'\n\n" +
            "6. 在 Recorder 窗口点击 'START RECORDING'\n\n" +
            "7. 播放场景，录制将自动进行\n\n" +
            "8. 完成后点击 'STOP RECORDING'\n\n" +
            "9. 在本组件上点击 'Stop Recording' 恢复设置";

        Debug.Log(guide);
        
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog("录制指南", guide, "确定");
        #endif
    }

    /// <summary>
    /// 打开Package Manager
    /// </summary>
    [ContextMenu("Open Package Manager (Install Recorder)")]
    private void OpenPackageManager()
    {
        UnityEditor.PackageManager.UI.Window.Open("Unity Recorder");
        Debug.Log("[RecordingHelper] 已打开 Package Manager，请搜索并安装 'Unity Recorder'");
    }
    #endif
}
