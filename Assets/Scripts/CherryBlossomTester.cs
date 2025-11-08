using UnityEngine;

/// <summary>
/// 樱花测试脚本 - 用于快速测试樱花效果
/// 按 C 键开始樱花，按 S 键停止樱花
/// </summary>
public class CherryBlossomTester : MonoBehaviour
{
    [Header("测试控制")]
    [SerializeField] private CherryBlossomController cherryController;
    
    [Header("快捷键")]
    [Tooltip("按此键开始樱花（默认 C）")]
    [SerializeField] private KeyCode startKey = KeyCode.C;
    
    [Tooltip("按此键停止樱花（默认 S）")]
    [SerializeField] private KeyCode stopKey = KeyCode.S;
    
    void Start()
    {
        if (cherryController == null)
        {
            cherryController = FindObjectOfType<CherryBlossomController>();
        }
        
        if (cherryController == null)
        {
            Debug.LogError("[CherryBlossomTester] 未找到 CherryBlossomController！");
        }
        else
        {
            Debug.Log($"[CherryBlossomTester] 已准备就绪！按 {startKey} 开始樱花，按 {stopKey} 停止樱花");
        }
    }
    
    void Update()
    {
        if (cherryController == null) return;
        
        // 按 C 键开始樱花
        if (Input.GetKeyDown(startKey))
        {
            Debug.Log($"[CherryBlossomTester] 按下 {startKey} 键 - 开始樱花");
            cherryController.StartCherryBlossom();
        }
        
        // 按 S 键停止樱花
        if (Input.GetKeyDown(stopKey))
        {
            Debug.Log($"[CherryBlossomTester] 按下 {stopKey} 键 - 停止樱花");
            cherryController.StopCherryBlossom();
        }
    }
}
