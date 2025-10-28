using UnityEngine;

/// <summary>
/// 使用说明组件 - 在Inspector中显示帮助信息
/// </summary>
public class HelpBox : MonoBehaviour
{
    [Header("使用说明")]
    [TextArea(10, 30)]
    [SerializeField] private string helpText = 
        "=== StarFalling 动画系统使用说明 ===\n\n" +
        "【快速开始】\n" +
        "1. 菜单: Tools > Star Falling Animation > Scene Setup\n" +
        "2. 点击一键创建场景按钮\n" +
        "3. 按下播放按钮即可预览动画\n\n" +
        "【动画流程】\n" +
        "价签显示 (2秒) → 价签淡出 → 星星掉落 → 道路延伸\n\n" +
        "【组件说明】\n" +
        "• MasterController: 主控制器，控制整个动画流程\n" +
        "• PriceTagAnimator: 价签UI动画控制\n" +
        "• StarSpawner: 星星生成器\n" +
        "• StarController: 单个星星的动画控制\n" +
        "• RoadManager: 道路延伸管理\n\n" +
        "【参数调整】\n" +
        "在各个组件的Inspector面板中可以调整:\n" +
        "- 价签显示时间和移动速度\n" +
        "- 星星掉落速度和生成间隔\n" +
        "- 道路延伸速度和长度\n" +
        "- 摄像机位置和角度\n\n" +
        "【创建Prefab】\n" +
        "菜单: Tools > Star Falling Animation > Prefab Creator\n" +
        "可自定义星星和道路的外观\n\n" +
        "【快捷键】\n" +
        "Space键: 重新播放动画\n\n" +
        "【调试】\n" +
        "Console中会显示详细的动画流程日志\n\n" +
        "【扩展功能】\n" +
        "- 更改价格: PriceTagAnimator.UpdatePrice(新价格)\n" +
        "- 切换环境: EnvironmentSettings 组件\n" +
        "- 摄像机跟随: CameraController.SetFollowRoad(true)\n\n" +
        "【导出视频】\n" +
        "安装 Unity Recorder 包后可录制动画";

    [Header("版本信息")]
    [SerializeField] private string version = "1.0.0";
    [SerializeField] private string author = "Unity Animation Assistant";

    /// <summary>
    /// 在Inspector中显示帮助按钮
    /// </summary>
    [ContextMenu("显示完整帮助")]
    public void ShowFullHelp()
    {
        Debug.Log(helpText);
        Debug.Log($"\n版本: {version}\n作者: {author}");
    }

    /// <summary>
    /// 打开文档（如果有的话）
    /// </summary>
    [ContextMenu("打开在线文档")]
    public void OpenDocumentation()
    {
        Debug.Log("文档功能待实现");
        // Application.OpenURL("https://your-documentation-url.com");
    }
}
