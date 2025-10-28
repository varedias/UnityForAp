using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Prefab 创建工具 - 快速生成星星和道路段预制体
/// </summary>
public class PrefabCreator : EditorWindow
{
    private GameObject starPreview;
    private GameObject roadPreview;

    private Color starColor = new Color(1f, 0.9f, 0.3f);
    private float starSize = 0.5f;
    private int starSpikes = 5;

    private float roadLength = 10f;
    private float roadWidth = 8f;
    private Color roadColor = new Color(0.2f, 0.2f, 0.2f);
    private Color greenBeltColor = new Color(0.2f, 0.6f, 0.2f);
    private Color grassColor = new Color(0.3f, 0.7f, 0.3f);

    [MenuItem("Tools/Star Falling Animation/Prefab Creator")]
    public static void ShowWindow()
    {
        GetWindow<PrefabCreator>("Prefab 创建工具");
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab 创建工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 星星预制体部分
        DrawStarSection();
        
        GUILayout.Space(20);
        
        // 道路预制体部分
        DrawRoadSection();
    }

    /// <summary>
    /// 绘制星星部分
    /// </summary>
    private void DrawStarSection()
    {
        GUILayout.Label("星星 Prefab", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("创建一个带有StarController组件的星星预制体", MessageType.Info);

        starColor = EditorGUILayout.ColorField("星星颜色", starColor);
        starSize = EditorGUILayout.Slider("星星大小", starSize, 0.1f, 2f);
        starSpikes = EditorGUILayout.IntSlider("星星尖角数量", starSpikes, 3, 8);

        GUILayout.Space(5);

        if (GUILayout.Button("创建星星 Prefab", GUILayout.Height(30)))
        {
            CreateStarPrefab();
        }
    }

    /// <summary>
    /// 绘制道路部分
    /// </summary>
    private void DrawRoadSection()
    {
        GUILayout.Label("道路段 Prefab", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("创建一个包含路面、绿化带和草地的道路段预制体", MessageType.Info);

        roadLength = EditorGUILayout.Slider("道路长度", roadLength, 5f, 20f);
        roadWidth = EditorGUILayout.Slider("道路宽度", roadWidth, 4f, 15f);
        roadColor = EditorGUILayout.ColorField("路面颜色", roadColor);
        greenBeltColor = EditorGUILayout.ColorField("绿化带颜色", greenBeltColor);
        grassColor = EditorGUILayout.ColorField("草地颜色", grassColor);

        GUILayout.Space(5);

        if (GUILayout.Button("创建道路段 Prefab", GUILayout.Height(30)))
        {
            CreateRoadPrefab();
        }
    }

    /// <summary>
    /// 创建星星预制体
    /// </summary>
    private void CreateStarPrefab()
    {
        // 创建星星对象
        GameObject star = new GameObject("Star");

        // 创建主体（球体）
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        core.name = "Core";
        core.transform.SetParent(star.transform);
        core.transform.localPosition = Vector3.zero;
        core.transform.localScale = Vector3.one * starSize;

        // 创建尖角
        for (int i = 0; i < starSpikes; i++)
        {
            GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spike.name = $"Spike_{i + 1}";
            spike.transform.SetParent(star.transform);
            spike.transform.localPosition = Vector3.zero;
            spike.transform.localScale = new Vector3(0.05f * starSize, 0.3f * starSize, 0.05f * starSize);
            spike.transform.localRotation = Quaternion.Euler(0, 0, i * (360f / starSpikes));
        }

        // 创建材质
        Material starMat = CreateStarMaterial();
        
        // 应用材质到所有渲染器
        foreach (Renderer renderer in star.GetComponentsInChildren<Renderer>())
        {
            renderer.material = starMat;
        }

        // 添加StarController组件
        star.AddComponent<StarController>();

        // 保存为预制体
        SavePrefab(star, "Star");
        
        DestroyImmediate(star);
        
        EditorUtility.DisplayDialog("完成", "星星预制体已创建！\n路径: Assets/Prefabs/Star.prefab", "确定");
    }

    /// <summary>
    /// 创建道路段预制体
    /// </summary>
    private void CreateRoadPrefab()
    {
        GameObject roadSegment = new GameObject("RoadSegment");

        // 创建路面
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "Road";
        road.transform.SetParent(roadSegment.transform);
        road.transform.localPosition = Vector3.zero;
        road.transform.localScale = new Vector3(roadWidth * 0.6f, 0.1f, roadLength);
        
        Material roadMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        roadMat.color = roadColor;
        road.GetComponent<Renderer>().material = roadMat;

        // 创建中央绿化带
        GameObject greenBelt = GameObject.CreatePrimitive(PrimitiveType.Cube);
        greenBelt.name = "GreenBelt";
        greenBelt.transform.SetParent(roadSegment.transform);
        greenBelt.transform.localPosition = Vector3.zero;
        greenBelt.transform.localScale = new Vector3(roadWidth * 0.1f, 0.15f, roadLength);
        
        Material beltMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        beltMat.color = greenBeltColor;
        greenBelt.GetComponent<Renderer>().material = beltMat;

        // 创建左侧草地
        GameObject grassLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grassLeft.name = "Grass_Left";
        grassLeft.transform.SetParent(roadSegment.transform);
        grassLeft.transform.localPosition = new Vector3(-roadWidth * 0.4f, 0, 0);
        grassLeft.transform.localScale = new Vector3(roadWidth * 0.2f, 0.05f, roadLength);
        
        Material grassMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        grassMat.color = grassColor;
        grassLeft.GetComponent<Renderer>().material = grassMat;

        // 创建右侧草地
        GameObject grassRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grassRight.name = "Grass_Right";
        grassRight.transform.SetParent(roadSegment.transform);
        grassRight.transform.localPosition = new Vector3(roadWidth * 0.4f, 0, 0);
        grassRight.transform.localScale = new Vector3(roadWidth * 0.2f, 0.05f, roadLength);
        grassRight.GetComponent<Renderer>().material = grassMat;

        // 保存为预制体
        SavePrefab(roadSegment, "RoadSegment");
        
        DestroyImmediate(roadSegment);
        
        EditorUtility.DisplayDialog("完成", "道路段预制体已创建！\n路径: Assets/Prefabs/RoadSegment.prefab", "确定");
    }

    /// <summary>
    /// 创建星星材质
    /// </summary>
    private Material CreateStarMaterial()
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = starColor;
        
        // 设置为透明模式
        mat.SetFloat("_Surface", 1); // Transparent
        mat.SetFloat("_Blend", 0); // Alpha
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;
        
        // 添加自发光
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", starColor * 0.5f);
        }

        // 保存材质
        string materialPath = "Assets/Materials/StarMaterial.mat";
        EnsureDirectoryExists(Path.GetDirectoryName(materialPath));
        AssetDatabase.CreateAsset(mat, materialPath);
        
        return mat;
    }

    /// <summary>
    /// 保存预制体
    /// </summary>
    private void SavePrefab(GameObject obj, string name)
    {
        string prefabPath = $"Assets/Prefabs/{name}.prefab";
        EnsureDirectoryExists("Assets/Prefabs");
        
        PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
        Debug.Log($"[PrefabCreator] 预制体已保存: {prefabPath}");
    }

    /// <summary>
    /// 确保目录存在
    /// </summary>
    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}
