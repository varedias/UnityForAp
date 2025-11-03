using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 道路管理器 - 管理道路的生成、拼接和延伸
/// </summary>
public class RoadManager : MonoBehaviour
{
    [Header("道路预制体")]
    [Tooltip("道路段预制体（从 KajamansRoads 资源包中拖入，或留空使用程序化生成）")]
    [SerializeField] private GameObject roadSegmentPrefab;
    
    [Tooltip("如果使用 KajamansRoads 资源包，是否需要旋转模型")]
    [SerializeField] private bool rotateRoadPrefab = true;
    
    [Tooltip("道路预制体的旋转角度（Y轴）")]
    [SerializeField] private float roadPrefabRotation = 90f;

    [Header("道路配置")]
    [Tooltip("初始道路段数量")]
    [SerializeField] private int initialRoadSegments = 1;
    
    [Tooltip("每段道路的长度（Z轴）- 必须与预制体实际长度匹配！")]
    [SerializeField] private float segmentLength = 20f;
    
    [Tooltip("道路宽度（X轴）")]
    [SerializeField] private float roadWidth = 10f;

    [Header("延伸参数")]
    [Tooltip("是否启用道路延伸（取消勾选=只显示初始道路，不延伸）")]
    [SerializeField] private bool enableRoadExtension = false;
    
    [Tooltip("道路延伸动画时间")]
    [SerializeField] private float extendDuration = 0.5f;
    
    [Tooltip("延伸动画曲线")]
    [SerializeField] private AnimationCurve extendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Tooltip("最大道路段数量（防止无限延伸）")]
    [SerializeField] private int maxRoadSegments = 10;

    [Header("视觉效果")]
    [Tooltip("是否启用平滑过渡")]
    [SerializeField] private bool smoothTransition = true;

    // 内部变量
    private List<GameObject> roadSegments = new List<GameObject>();
    private Vector3 nextSegmentPosition;
    private int segmentCounter = 0;
    private bool isExtending = false;

    /// <summary>
    /// 初始化道路系统
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[RoadManager] 初始化道路系统");

        // 自动检测预制体尺寸
        if (roadSegmentPrefab != null)
        {
            AutoDetectPrefabSize();
        }

        // 清空现有道路
        ClearAllRoads();

        // 生成初始道路 - 使用渐变出现
        // WebGL 中使用更近的起始位置以便摄像机能看到
        #if UNITY_WEBGL && !UNITY_EDITOR
        nextSegmentPosition = new Vector3(0, 0, 0); // 从原点开始
        Debug.Log("[RoadManager] WebGL 模式 - 使用缩放动画生成道路");
        #else
        nextSegmentPosition = transform.position;
        #endif
        
        // 所有平台都使用缩放动画
        StartCoroutine(FadeInInitialRoads());
        
        // 如果使用外部预制体，添加自动修复组件
        if (roadSegmentPrefab != null)
        {
            RuntimeMaterialFixer fixer = gameObject.GetComponent<RuntimeMaterialFixer>();
            if (fixer == null)
            {
                fixer = gameObject.AddComponent<RuntimeMaterialFixer>();
                Debug.Log("[RoadManager] 已添加运行时材质自动修复器");
            }
        }
    }
    
    /// <summary>
    /// 渐变出现初始道路
    /// </summary>
    private IEnumerator FadeInInitialRoads()
    {
        for (int i = 0; i < initialRoadSegments; i++)
        {
            // 创建道路段
            GameObject segment = CreateRoadSegment(nextSegmentPosition, false);
            
            // 渐变出现动画
            yield return StartCoroutine(FadeInRoadSegment(segment));
            
            // 更新下一段位置
            nextSegmentPosition += new Vector3(0, 0, segmentLength);
        }
        
        Debug.Log($"[RoadManager] 已生成 {initialRoadSegments} 段初始道路（渐变出现）");
    }
    
    /// <summary>
    /// 单个道路段的渐变出现动画（使用缩放）
    /// </summary>
    private IEnumerator FadeInRoadSegment(GameObject segment)
    {
        // 保存原始缩放
        Vector3 originalScale = segment.transform.localScale;
        
        // 设置初始缩放为0
        segment.transform.localScale = Vector3.zero;
        
        // 渐变动画
        float duration = 3f; // 渐变持续时间（3秒）
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            
            // 使用平滑曲线
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            
            // 更新缩放
            segment.transform.localScale = originalScale * smoothProgress;
            
            yield return null;
        }
        
        // 确保最终缩放正确
        segment.transform.localScale = originalScale;
    }
    
    /// <summary>
    /// 自动检测预制体尺寸
    /// </summary>
    private void AutoDetectPrefabSize()
    {
        // 获取预制体的渲染器边界
        Renderer[] renderers = roadSegmentPrefab.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length > 0)
        {
            Bounds combinedBounds = renderers[0].bounds;
            
            foreach (Renderer r in renderers)
            {
                combinedBounds.Encapsulate(r.bounds);
            }
            
            // 获取尺寸（考虑旋转）
            Vector3 size = combinedBounds.size;
            float detectedLength = rotateRoadPrefab ? size.x : size.z;
            float detectedWidth = rotateRoadPrefab ? size.z : size.x;
            
            // 只在差异较大时更新
            if (Mathf.Abs(detectedLength - segmentLength) > 1f)
            {
                Debug.Log($"[RoadManager] 自动检测到道路长度: {detectedLength:F1} (当前设置: {segmentLength})");
                segmentLength = detectedLength;
            }
            
            if (Mathf.Abs(detectedWidth - roadWidth) > 1f)
            {
                Debug.Log($"[RoadManager] 自动检测到道路宽度: {detectedWidth:F1} (当前设置: {roadWidth})");
                roadWidth = detectedWidth;
            }
        }
    }

    /// <summary>
    /// 延伸道路（由星星消失时触发）
    /// </summary>
    public void ExtendRoad()
    {
        // 检查是否启用道路延伸
        if (!enableRoadExtension)
        {
            Debug.Log("[RoadManager] 道路延伸已禁用，不会添加新道路段");
            return;
        }
        
        if (roadSegments.Count >= maxRoadSegments)
        {
            Debug.LogWarning($"[RoadManager] 已达到最大道路段数 ({maxRoadSegments})，停止延伸");
            return;
        }

        if (isExtending && smoothTransition)
        {
            Debug.LogWarning("[RoadManager] 道路正在延伸中，请稍候...");
            return;
        }

        Debug.Log($"[RoadManager] 延伸道路 - 当前段数: {roadSegments.Count}");
        
        if (smoothTransition)
        {
            StartCoroutine(ExtendRoadAnimated());
        }
        else
        {
            CreateRoadSegment(nextSegmentPosition, true);
        }
    }

    /// <summary>
    /// 带动画的道路延伸
    /// </summary>
    private IEnumerator ExtendRoadAnimated()
    {
        isExtending = true;

        // 创建新道路段（初始缩放为0）
        GameObject newSegment = CreateRoadSegment(nextSegmentPosition, false);
        Vector3 originalScale = newSegment.transform.localScale;
        newSegment.transform.localScale = new Vector3(originalScale.x, originalScale.y, 0);

        float elapsed = 0f;

        // 动画延伸
        while (elapsed < extendDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / extendDuration);
            float curveValue = extendCurve.Evaluate(progress);

            // 沿Z轴缩放
            Vector3 scale = new Vector3(
                originalScale.x,
                originalScale.y,
                originalScale.z * curveValue
            );
            newSegment.transform.localScale = scale;

            yield return null;
        }

        // 确保最终缩放正确
        newSegment.transform.localScale = originalScale;

        isExtending = false;
    }

    /// <summary>
    /// 创建单个道路段
    /// </summary>
    private GameObject CreateRoadSegment(Vector3 position, bool updateNextPosition)
    {
        GameObject segment;

        if (roadSegmentPrefab != null)
        {
            // 使用预制体（KajamansRoads 或自定义）
            Quaternion rotation = rotateRoadPrefab ? 
                Quaternion.Euler(0, roadPrefabRotation, 0) : 
                Quaternion.identity;
            
            segment = Instantiate(roadSegmentPrefab, position, rotation, transform);
            
            #if UNITY_WEBGL && !UNITY_EDITOR
            // WebGL 中强制修复材质
            FixSegmentMaterialsForWebGL(segment);
            #endif
            
            Debug.Log($"[RoadManager] 使用自定义道路预制体: {roadSegmentPrefab.name}");
        }
        else
        {
            // 如果没有预制体，创建程序化道路段
            segment = CreateProceduralRoadSegment(position);
            Debug.Log("[RoadManager] 使用程序化道路生成");
        }

        // 命名
        segmentCounter++;
        segment.name = $"RoadSegment_{segmentCounter:D3}";

        // 添加到列表
        roadSegments.Add(segment);

        // 更新下一段的位置
        if (updateNextPosition)
        {
            nextSegmentPosition += new Vector3(0, 0, segmentLength);
        }

        return segment;
    }

    /// <summary>
    /// 创建程序化道路段（当没有预制体时）
    /// </summary>
    private GameObject CreateProceduralRoadSegment(Vector3 position)
    {
        GameObject segment = new GameObject("RoadSegment_Procedural");
        segment.transform.position = position;
        segment.transform.parent = transform;

        // 创建地面（柏油路）
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "Road";
        road.transform.parent = segment.transform;
        road.transform.localPosition = Vector3.zero;
        road.transform.localScale = new Vector3(roadWidth * 0.6f, 0.1f, segmentLength);

        // 创建中央绿化带
        GameObject greenBelt = GameObject.CreatePrimitive(PrimitiveType.Cube);
        greenBelt.name = "GreenBelt";
        greenBelt.transform.parent = segment.transform;
        greenBelt.transform.localPosition = Vector3.zero;
        greenBelt.transform.localScale = new Vector3(roadWidth * 0.1f, 0.15f, segmentLength);

        // 创建路边草地（左侧）
        GameObject grassLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grassLeft.name = "Grass_Left";
        grassLeft.transform.parent = segment.transform;
        grassLeft.transform.localPosition = new Vector3(-roadWidth * 0.4f, 0, 0);
        grassLeft.transform.localScale = new Vector3(roadWidth * 0.2f, 0.05f, segmentLength);

        // 创建路边草地（右侧）
        GameObject grassRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grassRight.name = "Grass_Right";
        grassRight.transform.parent = segment.transform;
        grassRight.transform.localPosition = new Vector3(roadWidth * 0.4f, 0, 0);
        grassRight.transform.localScale = new Vector3(roadWidth * 0.2f, 0.05f, segmentLength);

        // 设置默认材质颜色
        SetDefaultColors(road, greenBelt, grassLeft, grassRight);

        return segment;
    }

    /// <summary>
    /// 设置默认颜色
    /// </summary>
    private void SetDefaultColors(GameObject road, GameObject greenBelt, GameObject grassLeft, GameObject grassRight)
    {
        // 柏油路 - 深灰色 (使用URP兼容材质)
        Renderer roadRenderer = road.GetComponent<Renderer>();
        if (roadRenderer != null)
        {
            Material roadMat = CreateURPCompatibleMaterial(new Color(0.2f, 0.2f, 0.2f));
            if (roadMat != null)
            {
                roadRenderer.material = roadMat;
            }
        }

        // 绿化带 - 绿色
        Renderer beltRenderer = greenBelt.GetComponent<Renderer>();
        if (beltRenderer != null)
        {
            Material beltMat = CreateURPCompatibleMaterial(new Color(0.2f, 0.6f, 0.2f));
            if (beltMat != null)
            {
                beltRenderer.material = beltMat;
            }
        }

        // 草地 - 浅绿色
        Renderer grassLeftRenderer = grassLeft.GetComponent<Renderer>();
        if (grassLeftRenderer != null)
        {
            Material grassMat = CreateURPCompatibleMaterial(new Color(0.3f, 0.7f, 0.3f));
            if (grassMat != null)
            {
                grassLeftRenderer.material = grassMat;
            }
        }

        Renderer grassRightRenderer = grassRight.GetComponent<Renderer>();
        if (grassRightRenderer != null)
        {
            Material grassMat = CreateURPCompatibleMaterial(new Color(0.3f, 0.7f, 0.3f));
            if (grassMat != null)
            {
                grassRightRenderer.material = grassMat;
            }
        }
    }

    /// <summary>
    /// 创建URP兼容的材质
    /// </summary>
    private Material CreateURPCompatibleMaterial(Color color)
    {
        Material mat = null;
        
        // 尝试使用URP Unlit着色器（最兼容）
        Shader urpUnlitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (urpUnlitShader != null)
        {
            mat = new Material(urpUnlitShader);
            mat.SetColor("_BaseColor", color);
            Debug.Log($"[RoadManager] 使用 URP/Unlit 着色器创建材质: {color}");
            return mat;
        }
        
        // 尝试使用URP Lit着色器
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader != null)
        {
            mat = new Material(urpShader);
            mat.SetColor("_BaseColor", color);
            Debug.Log($"[RoadManager] 使用 URP/Lit 着色器创建材质: {color}");
            return mat;
        }
        
        // 如果找不到URP着色器，尝试使用Unlit/Color
        Shader unlitShader = Shader.Find("Unlit/Color");
        if (unlitShader != null)
        {
            mat = new Material(unlitShader);
            mat.SetColor("_Color", color);
            Debug.Log($"[RoadManager] 使用 Unlit/Color 着色器创建材质: {color}");
            return mat;
        }
        
        // 尝试使用标准着色器
        Shader standardShader = Shader.Find("Standard");
        if (standardShader != null)
        {
            mat = new Material(standardShader);
            mat.color = color;
            Debug.Log($"[RoadManager] 使用 Standard 着色器创建材质: {color}");
            return mat;
        }
        
        Debug.LogError("[RoadManager] 无法找到任何合适的着色器！道路将显示为紫色。");
        Debug.LogError("[RoadManager] 请检查: 1) URP包是否已安装 2) 项目设置是否正确");
        
        return mat;
    }

    /// <summary>
    /// 清空所有道路
    /// </summary>
    private void ClearAllRoads()
    {
        foreach (GameObject segment in roadSegments)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }

        roadSegments.Clear();
        segmentCounter = 0;
    }

    /// <summary>
    /// 重置道路管理器
    /// </summary>
    public void Reset()
    {
        Debug.Log("[RoadManager] 重置道路系统");
        
        StopAllCoroutines();
        isExtending = false;
        ClearAllRoads();
        nextSegmentPosition = transform.position;
    }

    /// <summary>
    /// 获取当前道路总长度
    /// </summary>
    public float GetTotalRoadLength()
    {
        return roadSegments.Count * segmentLength;
    }

    /// <summary>
    /// 获取道路中心点位置
    /// </summary>
    public Vector3 GetRoadCenter()
    {
        if (roadSegments.Count == 0)
            return transform.position;

        float halfLength = GetTotalRoadLength() / 2f;
        return transform.position + new Vector3(0, 0, halfLength);
    }

    /// <summary>
    /// WebGL 材质修复 - 将 Standard 着色器替换为 URP
    /// </summary>
    private void FixSegmentMaterialsForWebGL(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            bool changed = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                
                if (mat != null && mat.shader != null)
                {
                    string shaderName = mat.shader.name;
                    
                    // 如果是 Standard 或不兼容的着色器
                    if (shaderName.Contains("Standard") || 
                        shaderName.Contains("Legacy") ||
                        shaderName.Contains("Mobile/"))
                    {
                        // 保存原始贴图和颜色
                        Texture mainTex = mat.GetTexture("_MainTex");
                        Color color = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
                        
                        // 尝试使用 URP Lit
                        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
                        if (urpShader == null)
                        {
                            urpShader = Shader.Find("Universal Render Pipeline/Simple Lit");
                        }
                        if (urpShader == null)
                        {
                            urpShader = Shader.Find("Unlit/Texture");
                        }
                        
                        if (urpShader != null)
                        {
                            Material newMat = new Material(urpShader);
                            newMat.name = mat.name + "_WebGL";
                            
                            // 恢复贴图和颜色
                            if (mainTex != null)
                            {
                                if (newMat.HasProperty("_BaseMap"))
                                    newMat.SetTexture("_BaseMap", mainTex);
                                else if (newMat.HasProperty("_MainTex"))
                                    newMat.SetTexture("_MainTex", mainTex);
                            }
                            
                            if (newMat.HasProperty("_BaseColor"))
                                newMat.SetColor("_BaseColor", color);
                            else if (newMat.HasProperty("_Color"))
                                newMat.SetColor("_Color", color);
                            
                            materials[i] = newMat;
                            changed = true;
                            
                            Debug.Log($"[RoadManager] WebGL修复: {shaderName} -> {urpShader.name}");
                        }
                    }
                }
            }
            
            if (changed)
            {
                renderer.materials = materials;
            }
        }
    }

    // 编辑器辅助功能
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 绘制道路区域
        Gizmos.color = Color.yellow;
        Vector3 center = GetRoadCenter();
        Gizmos.DrawWireCube(center, new Vector3(roadWidth, 0.5f, GetTotalRoadLength()));

        // 绘制下一段道路位置
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(nextSegmentPosition + new Vector3(0, 0, segmentLength / 2), 
                           new Vector3(roadWidth, 0.5f, segmentLength));
    }
}
