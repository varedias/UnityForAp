using UnityEngine;
using UnityEditor;

/// <summary>
/// 资源配置助手 - 帮助设置外部资源包
/// </summary>
public class AssetConfigHelper : EditorWindow
{
    private Material fantasySkybox;
    private GameObject roadPrefab;
    private bool autoSearch = true;

    [MenuItem("Tools/Star Falling Animation/Configure Assets")]
    public static void ShowWindow()
    {
        AssetConfigHelper window = GetWindow<AssetConfigHelper>("资源配置");
        window.minSize = new Vector2(450, 400);
        window.Show();
    }

    void OnEnable()
    {
        if (autoSearch)
        {
            AutoSearchAssets();
        }
    }

    void OnGUI()
    {
        GUILayout.Label("外部资源配置", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "将从资源包中拖入的资源配置到场景中。\n" +
            "支持的资源包：\n" +
            "• Fantasy Skybox（天空盒）\n" +
            "• KajamansRoads（道路）",
            MessageType.Info);

        GUILayout.Space(10);

        // Fantasy Skybox 配置
        DrawSkyboxSection();

        GUILayout.Space(15);

        // KajamansRoads 配置
        DrawRoadSection();

        GUILayout.Space(15);

        // 自动搜索
        DrawAutoSearchSection();

        GUILayout.Space(15);

        // 应用按钮
        DrawApplyButtons();
    }

    /// <summary>
    /// 绘制天空盒部分
    /// </summary>
    private void DrawSkyboxSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        GUILayout.Label("☁️ Fantasy Skybox 天空盒", EditorStyles.boldLabel);
        
        fantasySkybox = (Material)EditorGUILayout.ObjectField(
            "天空盒材质",
            fantasySkybox,
            typeof(Material),
            false);

        if (fantasySkybox != null)
        {
            EditorGUILayout.HelpBox($"已选择: {fantasySkybox.name}", MessageType.None);
        }
        else
        {
            EditorGUILayout.HelpBox("请从 Fantasy Skybox 资源包中拖入天空盒材质", MessageType.Warning);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("🔍 自动搜索天空盒材质"))
        {
            SearchForSkybox();
        }

        if (fantasySkybox != null && GUILayout.Button("✅ 应用到场景"))
        {
            ApplySkyboxToScene();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制道路部分
    /// </summary>
    private void DrawRoadSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        GUILayout.Label("🛣️ KajamansRoads 道路", EditorStyles.boldLabel);
        
        roadPrefab = (GameObject)EditorGUILayout.ObjectField(
            "道路预制体",
            roadPrefab,
            typeof(GameObject),
            false);

        if (roadPrefab != null)
        {
            EditorGUILayout.HelpBox($"已选择: {roadPrefab.name}", MessageType.None);
        }
        else
        {
            EditorGUILayout.HelpBox("请从 KajamansRoads 资源包中拖入道路预制体", MessageType.Warning);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("🔍 自动搜索道路预制体"))
        {
            SearchForRoadPrefab();
        }

        if (roadPrefab != null && GUILayout.Button("✅ 应用到 RoadManager"))
        {
            ApplyRoadPrefabToManager();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制自动搜索部分
    /// </summary>
    private void DrawAutoSearchSection()
    {
        EditorGUILayout.BeginVertical("box");
        
        if (GUILayout.Button("🔍 自动搜索所有资源", GUILayout.Height(35)))
        {
            AutoSearchAssets();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 绘制应用按钮
    /// </summary>
    private void DrawApplyButtons()
    {
        EditorGUILayout.BeginVertical("box");
        
        if (GUILayout.Button("✅ 应用所有配置", GUILayout.Height(40)))
        {
            ApplyAllConfigurations();
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 自动搜索资源
    /// </summary>
    private void AutoSearchAssets()
    {
        SearchForSkybox();
        SearchForRoadPrefab();
    }

    /// <summary>
    /// 搜索天空盒材质
    /// </summary>
    private void SearchForSkybox()
    {
        // 优先搜索 Fantasy Skybox FREE 文件夹
        string[] specificPaths = {
            "Assets/Fantasy Skybox FREE/Cubemaps/Classic/FS000_Day_01.mat",
            "Assets/Fantasy Skybox FREE/Cubemaps/Classic/FS000_Day_02.mat",
            "Assets/Fantasy Skybox FREE/Cubemaps/Classic/FS000_Day_03.mat"
        };
        
        // 先尝试直接加载常见的天空盒
        foreach (string path in specificPaths)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                fantasySkybox = mat;
                Debug.Log($"[AssetConfig] 找到天空盒: {mat.name} at {path}");
                return;
            }
        }
        
        // 如果直接路径失败，搜索整个项目
        string[] guids = AssetDatabase.FindAssets("t:Material FS000", new[] { "Assets/Fantasy Skybox FREE" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (mat != null && (mat.shader.name.Contains("Skybox") || path.Contains("Fantasy")))
            {
                fantasySkybox = mat;
                Debug.Log($"[AssetConfig] 找到天空盒: {mat.name} at {path}");
                return;
            }
        }
        
        Debug.LogWarning("[AssetConfig] 未找到天空盒材质，请从 Assets/Fantasy Skybox FREE/Cubemaps/Classic 文件夹手动拖入");
    }

    /// <summary>
    /// 搜索道路预制体
    /// </summary>
    private void SearchForRoadPrefab()
    {
        // 优先搜索 KajamansRoads 文件夹中的预制体
        string[] specificPaths = {
            "Assets/KajamansRoads/Free/Prefabs/l10km_cc4_sl20_t(12123025)_rw10_wh3_n3_RsBTW_MeshV00.prefab",
            "Assets/KajamansRoads/Free/Prefabs/l10km_cc4_sl69_t(1212029)_rw12_wh15_n1_RsBtW_MeshV00.prefab",
            "Assets/KajamansRoads/Free/Prefabs/l20km_cc2_sl30_t(401509)_rw28_wh15_n1_RsBTW_MeshV00.prefab"
        };
        
        // 先尝试直接加载
        foreach (string path in specificPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                roadPrefab = prefab;
                Debug.Log($"[AssetConfig] 找到道路预制体: {prefab.name} at {path}");
                return;
            }
        }
        
        // 如果直接路径失败，搜索 KajamansRoads 文件夹
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/KajamansRoads/Free/Prefabs" });
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                roadPrefab = prefab;
                Debug.Log($"[AssetConfig] 找到道路预制体: {prefab.name} at {path}");
                return;
            }
        }
        
        Debug.LogWarning("[AssetConfig] 未找到道路预制体，请从 Assets/KajamansRoads/Free/Prefabs 文件夹手动拖入");
    }

    /// <summary>
    /// 应用天空盒到场景
    /// </summary>
    private void ApplySkyboxToScene()
    {
        if (fantasySkybox == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择天空盒材质", "确定");
            return;
        }

        // 应用到场景设置
        RenderSettings.skybox = fantasySkybox;
        DynamicGI.UpdateEnvironment();

        // 查找或创建 EnvironmentSettings 组件
        EnvironmentSettings envSettings = FindObjectOfType<EnvironmentSettings>();
        if (envSettings == null)
        {
            GameObject envGo = new GameObject("EnvironmentSettings");
            envSettings = envGo.AddComponent<EnvironmentSettings>();
        }

        // 设置材质引用
        SerializedObject so = new SerializedObject(envSettings);
        so.FindProperty("skyboxMaterial").objectReferenceValue = fantasySkybox;
        so.FindProperty("useProceduralSky").boolValue = false;
        so.ApplyModifiedProperties();

        Debug.Log($"[AssetConfig] 已应用天空盒: {fantasySkybox.name}");
        EditorUtility.DisplayDialog("成功", $"天空盒已应用: {fantasySkybox.name}", "确定");
    }

    /// <summary>
    /// 应用道路预制体到 RoadManager
    /// </summary>
    private void ApplyRoadPrefabToManager()
    {
        if (roadPrefab == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择道路预制体", "确定");
            return;
        }

        // 查找 RoadManager
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager == null)
        {
            bool create = EditorUtility.DisplayDialog(
                "未找到 RoadManager",
                "场景中未找到 RoadManager，是否创建？",
                "创建", "取消");

            if (create)
            {
                GameObject roadGo = new GameObject("RoadManager");
                roadManager = roadGo.AddComponent<RoadManager>();
            }
            else
            {
                return;
            }
        }

        // 设置预制体引用
        SerializedObject so = new SerializedObject(roadManager);
        so.FindProperty("roadSegmentPrefab").objectReferenceValue = roadPrefab;
        so.ApplyModifiedProperties();

        Debug.Log($"[AssetConfig] 已应用道路预制体: {roadPrefab.name}");
        EditorUtility.DisplayDialog("成功", $"道路预制体已应用: {roadPrefab.name}", "确定");
    }

    /// <summary>
    /// 应用所有配置
    /// </summary>
    private void ApplyAllConfigurations()
    {
        bool hasChanges = false;

        if (fantasySkybox != null)
        {
            ApplySkyboxToScene();
            hasChanges = true;
        }

        if (roadPrefab != null)
        {
            ApplyRoadPrefabToManager();
            hasChanges = true;
        }

        if (!hasChanges)
        {
            EditorUtility.DisplayDialog("提示", "请先选择要应用的资源", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("完成", "所有配置已应用！", "确定");
        }
    }
}
