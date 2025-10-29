using UnityEngine;

/// <summary>
/// 摄像机控制器 - 固定拍摄地面
/// </summary>
[RequireComponent(typeof(Camera))]
[DefaultExecutionOrder(-100)] // 确保在其他脚本之前执行
public class CameraController : MonoBehaviour
{
    [Header("摄像机模式")]
    [Tooltip("摄像机视角模式")]
    [SerializeField] private CameraMode cameraMode = CameraMode.FixedAngle;

    [Header("固定角度设置")]
    [Tooltip("摄像机位置")]
    [SerializeField] private Vector3 cameraPosition = new Vector3(0, 8, -12);
    
    [Tooltip("摄像机旋转角度")]
    [SerializeField] private Vector3 cameraRotation = new Vector3(-1, 3, 0);
    
    [Header("强制设置")]
    [Tooltip("忽略场景中保存的值，强制使用代码中的默认值")]
    [SerializeField] private bool forceUseDefaultValues = true;

    [Header("跟随道路设置")]
    [Tooltip("是否跟随道路延伸")]
    [SerializeField] private bool followRoad = false;
    
    [Tooltip("道路管理器引用")]
    [SerializeField] private RoadManager roadManager;
    
    [Tooltip("跟随平滑度")]
    [SerializeField] private float followSmoothness = 5f;
    
    [Tooltip("相对道路中心的偏移")]
    [SerializeField] private Vector3 roadOffset = new Vector3(0, 8, -5);

    [Header("后处理效果")]
    [Tooltip("视野范围（FOV）- 更大的值可以看到更多天空")]
    [SerializeField] private float fieldOfView = 60f;

    private Camera cam;
    private Vector3 targetPosition;
    
    private bool hasAppliedSettings = false; // 防止重复应用

    public enum CameraMode
    {
        FixedAngle,      // 固定角度
        FollowRoad       // 跟随道路
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        
        // 如果启用强制使用默认值，重置为代码中的默认值
        if (forceUseDefaultValues)
        {
            cameraPosition = new Vector3(0, 8, -12);
            cameraRotation = new Vector3(-1, 3, 0);
            fieldOfView = 60f;
        }
        
        // 最优先：立即强制应用正确的设置
        Debug.Log($"[CameraController] Awake - 应用设置: 位置={cameraPosition}, 旋转={cameraRotation}");
        ForceApplyCameraSettings();
        
        // 初始化摄像机设置
        InitializeCamera();
    }

    void Start()
    {
        // 查找道路管理器
        if (roadManager == null && followRoad)
        {
            roadManager = FindObjectOfType<RoadManager>();
        }

        // 再次确保摄像机设置正确（以防其他脚本修改了）
        if (!hasAppliedSettings)
        {
            Debug.Log($"[CameraController] Start - 再次检查Transform: 位置={transform.position}, 旋转={transform.eulerAngles}");
            ForceApplyCameraSettings();
        }
    }
    
    /// <summary>
    /// 强制应用摄像机设置（确保覆盖场景中的旧设置）
    /// </summary>
    private void ForceApplyCameraSettings()
    {
        // 强制设置位置和旋转
        transform.position = cameraPosition;
        transform.eulerAngles = cameraRotation;
        
        // 强制设置FOV和Clear Flags
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
            cam.clearFlags = CameraClearFlags.Skybox;
        }
        
        hasAppliedSettings = true;
        
        Debug.Log($"[CameraController] ✅ 摄像机强制设置完成: 位置={transform.position}, 旋转={transform.eulerAngles}, FOV={fieldOfView}");
    }

    void LateUpdate()
    {
        // 如果是固定角度模式，持续检查并锁定摄像机角度
        if (cameraMode == CameraMode.FixedAngle)
        {
            // 检测是否有其他代码修改了摄像机角度
            if (Vector3.Distance(transform.eulerAngles, cameraRotation) > 0.1f)
            {
                Debug.LogWarning($"[CameraController] 检测到摄像机角度被修改! 从 {transform.eulerAngles} 恢复到 {cameraRotation}");
                transform.eulerAngles = cameraRotation;
            }
            
            // 检测位置是否被修改
            if (Vector3.Distance(transform.position, cameraPosition) > 0.1f)
            {
                Debug.LogWarning($"[CameraController] 检测到摄像机位置被修改! 从 {transform.position} 恢复到 {cameraPosition}");
                transform.position = cameraPosition;
            }
        }
        else if (cameraMode == CameraMode.FollowRoad && followRoad)
        {
            UpdateFollowRoad();
        }
    }

    /// <summary>
    /// 初始化摄像机
    /// </summary>
    private void InitializeCamera()
    {
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
            
            // 确保使用天空盒
            cam.clearFlags = CameraClearFlags.Skybox;
            
            Debug.Log($"[CameraController] 摄像机初始化: FOV={fieldOfView}, ClearFlags={cam.clearFlags}");
        }
    }

    /// <summary>
    /// 应用摄像机设置
    /// </summary>
    private void ApplyCameraSettings()
    {
        switch (cameraMode)
        {
            case CameraMode.FixedAngle:
                transform.position = cameraPosition;
                transform.rotation = Quaternion.Euler(cameraRotation);
                break;

            case CameraMode.FollowRoad:
                if (roadManager != null)
                {
                    Vector3 roadCenter = roadManager.GetRoadCenter();
                    targetPosition = roadCenter + roadOffset;
                    transform.position = targetPosition;
                }
                transform.rotation = Quaternion.Euler(cameraRotation);
                break;
        }
    }

    /// <summary>
    /// 更新跟随道路
    /// </summary>
    private void UpdateFollowRoad()
    {
        if (roadManager == null) return;

        // 计算目标位置
        Vector3 roadCenter = roadManager.GetRoadCenter();
        targetPosition = roadCenter + roadOffset;

        // 平滑移动
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * followSmoothness
        );

        // 保持旋转
        transform.rotation = Quaternion.Euler(cameraRotation);
    }

    /// <summary>
    /// 设置摄像机模式
    /// </summary>
    public void SetCameraMode(CameraMode mode)
    {
        cameraMode = mode;
        ApplyCameraSettings();
    }

    /// <summary>
    /// 手动设置摄像机位置
    /// </summary>
    public void SetCameraPosition(Vector3 position, Vector3 rotation)
    {
        cameraPosition = position;
        cameraRotation = rotation;
        ApplyCameraSettings();
    }

    /// <summary>
    /// 启用/禁用道路跟随
    /// </summary>
    public void SetFollowRoad(bool enable)
    {
        followRoad = enable;
        if (enable)
        {
            cameraMode = CameraMode.FollowRoad;
        }
        else
        {
            cameraMode = CameraMode.FixedAngle;
        }
    }

    // 编辑器辅助功能
    void OnDrawGizmos()
    {
        // 绘制摄像机视锥
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // 绘制朝向
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }

    #if UNITY_EDITOR
    /// <summary>
    /// 在编辑器中预览摄像机位置
    /// </summary>
    [ContextMenu("Apply Camera Settings")]
    private void ApplyCameraSettingsInEditor()
    {
        ApplyCameraSettings();
        Debug.Log($"[CameraController] 已应用摄像机设置: 位置={transform.position}, 旋转={transform.rotation.eulerAngles}");
    }
    #endif
}
