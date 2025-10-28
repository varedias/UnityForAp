using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// 场景自动设置工具 - 一键创建完整的动画场景
/// </summary>
public class SceneSetupTool : EditorWindow
{
    private string priceText = "¥99";
    private bool createCamera = true;
    private bool createLighting = true;
    private bool createRoad = true;
    private bool createUI = true;

    [MenuItem("Tools/Star Falling Animation/Scene Setup")]
    public static void ShowWindow()
    {
        GetWindow<SceneSetupTool>("场景设置工具");
    }

    void OnGUI()
    {
        GUILayout.Label("StarFalling 场景自动设置", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("此工具将自动创建完整的动画场景，包括摄像机、灯光、道路、UI等所有必要组件。", MessageType.Info);
        GUILayout.Space(10);

        // 配置选项
        GUILayout.Label("配置选项", EditorStyles.boldLabel);
        createCamera = EditorGUILayout.Toggle("创建摄像机", createCamera);
        createLighting = EditorGUILayout.Toggle("创建灯光", createLighting);
        createRoad = EditorGUILayout.Toggle("创建道路系统", createRoad);
        createUI = EditorGUILayout.Toggle("创建UI系统", createUI);

        GUILayout.Space(10);

        // UI配置
        if (createUI)
        {
            GUILayout.Label("UI 配置", EditorStyles.boldLabel);
            priceText = EditorGUILayout.TextField("价格文本", priceText);
        }

        GUILayout.Space(20);

        // 创建按钮
        if (GUILayout.Button("一键创建场景", GUILayout.Height(40)))
        {
            SetupScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("清理场景", GUILayout.Height(30)))
        {
            CleanupScene();
        }
    }

    /// <summary>
    /// 设置完整场景
    /// </summary>
    private void SetupScene()
    {
        Debug.Log("========== 开始设置场景 ==========");

        // 1. 创建主控制器
        GameObject masterController = CreateMasterController();

        // 2. 创建摄像机
        if (createCamera)
        {
            CreateCamera();
        }

        // 3. 创建灯光
        if (createLighting)
        {
            CreateLighting();
        }

        // 4. 创建道路系统
        if (createRoad)
        {
            CreateRoadSystem(masterController);
        }

        // 5. 创建UI系统
        if (createUI)
        {
            CreateUISystem(masterController);
        }

        // 6. 创建星星生成器
        CreateStarSpawner(masterController);

        Debug.Log("========== 场景设置完成 ==========");
        EditorUtility.DisplayDialog("完成", "场景已成功设置！\n按下播放按钮即可预览动画。", "确定");
    }

    /// <summary>
    /// 创建主控制器
    /// </summary>
    private GameObject CreateMasterController()
    {
        GameObject go = GameObject.Find("MasterController");
        if (go == null)
        {
            go = new GameObject("MasterController");
            go.AddComponent<MasterController>();
            Debug.Log("[SceneSetup] 已创建 MasterController");
        }
        else
        {
            Debug.Log("[SceneSetup] MasterController 已存在");
        }
        return go;
    }

    /// <summary>
    /// 创建摄像机
    /// </summary>
    private void CreateCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            mainCam = camGo.AddComponent<Camera>();
            camGo.tag = "MainCamera";
        }

        // 添加摄像机控制器
        CameraController camController = mainCam.GetComponent<CameraController>();
        if (camController == null)
        {
            camController = mainCam.gameObject.AddComponent<CameraController>();
        }

        // CameraController 会在 Awake 时自动设置正确的位置和旋转
        // 位置: (0, 8, -12), 旋转: (3, 0, 0), FOV: 60
        // 无需手动设置,脚本会自动处理

        Debug.Log("[SceneSetup] 已设置摄像机 (将在运行时自动修复位置和角度)");
    }

    /// <summary>
    /// 创建灯光
    /// </summary>
    private void CreateLighting()
    {
        // 主方向光（太阳光）
        Light directionalLight = FindObjectOfType<Light>();
        if (directionalLight == null || directionalLight.type != LightType.Directional)
        {
            GameObject lightGo = new GameObject("Directional Light");
            directionalLight = lightGo.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
        }

        directionalLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        directionalLight.color = new Color(1f, 0.96f, 0.84f); // 温暖的日光
        directionalLight.intensity = 1.5f;

        // 设置环境光
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 0.8f;
        
        // 尝试自动加载 Fantasy Skybox 材质
        AutoLoadFantasySkybox();

        Debug.Log("[SceneSetup] 已设置灯光和天空盒");
    }
    
    /// <summary>
    /// 自动加载 Fantasy Skybox 材质
    /// </summary>
    private void AutoLoadFantasySkybox()
    {
        // 直接尝试加载 Fantasy Skybox FREE 中的材质
        string[] specificPaths = {
            "Assets/Fantasy Skybox FREE/Cubemaps/Classic/FS000_Day_01.mat",
            "Assets/Fantasy Skybox FREE/Cubemaps/Classic/FS000_Day_02.mat",
            "Assets/Fantasy Skybox FREE/Cubemaps/Classic/FS000_Day_03.mat"
        };
        
        foreach (string path in specificPaths)
        {
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (skyboxMat != null)
            {
                RenderSettings.skybox = skyboxMat;
                DynamicGI.UpdateEnvironment();
                Debug.Log($"[SceneSetup] 已自动加载天空盒: {skyboxMat.name}");
                return;
            }
        }
        
        // 如果直接路径失败，尝试搜索
        string[] guids = AssetDatabase.FindAssets("t:Material FS000", new[] { "Assets/Fantasy Skybox FREE" });
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(path);
            
            if (skyboxMat != null)
            {
                RenderSettings.skybox = skyboxMat;
                DynamicGI.UpdateEnvironment();
                Debug.Log($"[SceneSetup] 已自动加载天空盒: {skyboxMat.name}");
                return;
            }
        }
        
        Debug.LogWarning("[SceneSetup] 未找到 Fantasy Skybox 材质\n" +
                        "请手动设置: Window > Rendering > Lighting > Skybox Material\n" +
                        "或使用: Tools > Star Falling Animation > Configure Assets");
    }

    /// <summary>
    /// 创建道路系统
    /// </summary>
    private void CreateRoadSystem(GameObject masterController)
    {
        GameObject roadManager = GameObject.Find("RoadManager");
        if (roadManager == null)
        {
            roadManager = new GameObject("RoadManager");
            roadManager.AddComponent<RoadManager>();
        }

        // 尝试自动加载 KajamansRoads 预制体
        RoadManager roadMgr = roadManager.GetComponent<RoadManager>();
        AutoLoadRoadPrefab(roadMgr);

        // 连接到主控制器
        MasterController mc = masterController.GetComponent<MasterController>();
        if (mc != null)
        {
            SerializedObject so = new SerializedObject(mc);
            so.FindProperty("roadManager").objectReferenceValue = roadMgr;
            so.ApplyModifiedProperties();
        }

        Debug.Log("[SceneSetup] 已创建道路系统");
    }
    
    /// <summary>
    /// 自动加载道路预制体
    /// </summary>
    private void AutoLoadRoadPrefab(RoadManager roadManager)
    {
        if (roadManager == null) return;
        
        // 直接尝试加载 KajamansRoads 中的预制体
        string[] specificPaths = {
            "Assets/KajamansRoads/Free/Prefabs/l10km_cc4_sl20_t(12123025)_rw10_wh3_n3_RsBTW_MeshV00.prefab",
            "Assets/KajamansRoads/Free/Prefabs/l10km_cc4_sl69_t(1212029)_rw12_wh15_n1_RsBtW_MeshV00.prefab",
            "Assets/KajamansRoads/Free/Prefabs/l20km_cc2_sl30_t(401509)_rw28_wh15_n1_RsBTW_MeshV00.prefab"
        };
        
        foreach (string path in specificPaths)
        {
            GameObject roadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (roadPrefab != null)
            {
                SerializedObject so = new SerializedObject(roadManager);
                so.FindProperty("roadSegmentPrefab").objectReferenceValue = roadPrefab;
                so.FindProperty("segmentLength").floatValue = 10f; // 根据预制体调整
                so.ApplyModifiedProperties();
                
                Debug.Log($"[SceneSetup] 已自动加载道路预制体: {roadPrefab.name}");
                return;
            }
        }
        
        // 如果直接路径失败，搜索文件夹
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/KajamansRoads/Free/Prefabs" });
        
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject roadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (roadPrefab != null)
            {
                SerializedObject so = new SerializedObject(roadManager);
                so.FindProperty("roadSegmentPrefab").objectReferenceValue = roadPrefab;
                so.FindProperty("segmentLength").floatValue = 10f;
                so.ApplyModifiedProperties();
                
                Debug.Log($"[SceneSetup] 已自动加载道路预制体: {roadPrefab.name}");
                return;
            }
        }
        
        Debug.LogWarning("[SceneSetup] 未找到 KajamansRoads 预制体\n" +
                        "将使用程序化生成道路\n" +
                        "或使用: Tools > Star Falling Animation > Configure Assets 手动配置");
    }

    /// <summary>
    /// 创建UI系统
    /// </summary>
    private void CreateUISystem(GameObject masterController)
    {
        // 查找或创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("Canvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // 创建价签UI
        GameObject priceTag = new GameObject("PriceTag");
        priceTag.transform.SetParent(canvas.transform, false);

        // 添加RectTransform
        RectTransform rt = priceTag.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(300, 100);

        // 添加CanvasGroup
        priceTag.AddComponent<CanvasGroup>();

        // 添加背景图片
        UnityEngine.UI.Image bg = priceTag.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // 创建文本
        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(priceTag.transform, false);

        RectTransform textRt = textGo.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        // 添加TextMeshPro
        TextMeshProUGUI textMesh = textGo.AddComponent<TextMeshProUGUI>();
        textMesh.text = priceText;
        textMesh.fontSize = 48;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.white;

        // 添加价签动画控制器
        PriceTagAnimator animator = priceTag.AddComponent<PriceTagAnimator>();
        
        // 设置引用
        SerializedObject so = new SerializedObject(animator);
        so.FindProperty("priceText").objectReferenceValue = textMesh;
        so.FindProperty("canvasGroup").objectReferenceValue = priceTag.GetComponent<CanvasGroup>();
        so.FindProperty("priceString").stringValue = priceText;
        so.ApplyModifiedProperties();

        // 连接到主控制器
        MasterController mc = masterController.GetComponent<MasterController>();
        if (mc != null)
        {
            SerializedObject mcSo = new SerializedObject(mc);
            mcSo.FindProperty("priceTagAnimator").objectReferenceValue = animator;
            mcSo.ApplyModifiedProperties();
        }

        Debug.Log("[SceneSetup] 已创建UI系统");
    }

    /// <summary>
    /// 创建星星生成器
    /// </summary>
    private void CreateStarSpawner(GameObject masterController)
    {
        GameObject spawner = GameObject.Find("StarSpawner");
        if (spawner == null)
        {
            spawner = new GameObject("StarSpawner");
            spawner.AddComponent<StarSpawner>();
        }

        // 连接到主控制器
        MasterController mc = masterController.GetComponent<MasterController>();
        if (mc != null)
        {
            SerializedObject so = new SerializedObject(mc);
            so.FindProperty("starSpawner").objectReferenceValue = spawner.GetComponent<StarSpawner>();
            so.ApplyModifiedProperties();
        }

        Debug.Log("[SceneSetup] 已创建星星生成器");
    }

    /// <summary>
    /// 清理场景
    /// </summary>
    private void CleanupScene()
    {
        if (EditorUtility.DisplayDialog("确认", "确定要清理场景吗？这将删除所有相关GameObject。", "确定", "取消"))
        {
            // 删除所有相关对象
            string[] objectsToDelete = { "MasterController", "RoadManager", "StarSpawner", "Canvas" };
            
            foreach (string objName in objectsToDelete)
            {
                GameObject obj = GameObject.Find(objName);
                if (obj != null)
                {
                    DestroyImmediate(obj);
                    Debug.Log($"[SceneSetup] 已删除 {objName}");
                }
            }

            Debug.Log("========== 场景清理完成 ==========");
        }
    }
}
