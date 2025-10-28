using UnityEngine;
using UnityEditor;

/// <summary>
/// 修复紫色材质工具 - 一键修复场景中所有紫色材质
/// </summary>
public class FixPurpleMaterials : EditorWindow
{
    [MenuItem("Tools/Star Falling Animation/Fix Purple Materials")]
    public static void ShowWindow()
    {
        GetWindow<FixPurpleMaterials>("修复紫色材质");
    }

    void OnGUI()
    {
        GUILayout.Label("紫色材质修复工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "紫色材质表示着色器缺失或不兼容。\n" +
            "点击下面的按钮自动修复场景中所有紫色材质。",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("🔧 修复所有紫色材质", GUILayout.Height(40)))
        {
            FixAllPurpleMaterials();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("🎨 仅修复道路材质", GUILayout.Height(30)))
        {
            FixRoadMaterials();
        }

        GUILayout.Space(5);
        
        if (GUILayout.Button("�️ 修复 KajamansRoads 贴图", GUILayout.Height(30)))
        {
            FixKajamansRoadsMaterials();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("�📊 检测紫色材质", GUILayout.Height(30)))
        {
            DetectPurpleMaterials();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "提示:\n" +
            "• 🔧 修复所有紫色材质 - 修复整个场景\n" +
            "• 🎨 仅修复道路材质 - 只修复RoadManager下的物体\n" +
            "• 🛣️ 修复KajamansRoads贴图 - 保留原始贴图，只换着色器\n" +
            "• 📊 检测紫色材质 - 诊断问题",
            MessageType.None
        );
    }

    /// <summary>
    /// 修复所有紫色材质
    /// </summary>
    private void FixAllPurpleMaterials()
    {
        int fixedCount = 0;
        
        // 查找所有Renderer
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            bool materialChanged = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                
                if (mat == null)
                {
                    Debug.LogWarning($"[FixPurpleMaterials] {renderer.gameObject.name} 的材质槽 {i} 为空，跳过");
                    continue;
                }
                
                if (NeedsFix(mat))
                {
                    // 保留原材质的贴图和属性
                    Material fixedMat = FixMaterialShader(mat);
                    if (fixedMat != null)
                    {
                        materials[i] = fixedMat;
                        materialChanged = true;
                        fixedCount++;
                        Debug.Log($"[FixPurpleMaterials] 修复材质: {renderer.gameObject.name} - {mat.name}");
                    }
                }
            }
            
            if (materialChanged)
            {
                renderer.sharedMaterials = materials;
            }
        }

        EditorUtility.DisplayDialog(
            "修复完成",
            $"已修复 {fixedCount} 个紫色材质！\n\n贴图已保留，请检查 Scene 窗口查看效果。",
            "确定"
        );
    }
    
    /// <summary>
    /// 修复单个材质的着色器（保留贴图）
    /// </summary>
    private Material FixMaterialShader(Material originalMat)
    {
        // 获取原材质的所有贴图
        Texture mainTex = originalMat.HasProperty("_MainTex") ? originalMat.GetTexture("_MainTex") : null;
        Texture baseMap = originalMat.HasProperty("_BaseMap") ? originalMat.GetTexture("_BaseMap") : null;
        Color mainColor = originalMat.HasProperty("_Color") ? originalMat.GetColor("_Color") : Color.white;
        Color baseColor = originalMat.HasProperty("_BaseColor") ? originalMat.GetColor("_BaseColor") : Color.white;
        
        // 使用的贴图（优先级：BaseMap > MainTex）
        Texture texture = baseMap != null ? baseMap : mainTex;
        Color tintColor = baseColor != Color.white ? baseColor : mainColor;
        
        // 创建新材质
        Material newMat = null;
        
        // 尝试使用URP Lit着色器
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit != null)
        {
            newMat = new Material(urpLit);
            if (texture != null)
            {
                newMat.SetTexture("_BaseMap", texture);
            }
            newMat.SetColor("_BaseColor", tintColor);
            newMat.name = originalMat.name + "_Fixed";
            Debug.Log($"[FixPurpleMaterials] 使用 URP/Lit 修复，贴图: {texture?.name ?? "无"}");
            return newMat;
        }
        
        // 回退到URP Unlit
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
        if (urpUnlit != null)
        {
            newMat = new Material(urpUnlit);
            if (texture != null)
            {
                newMat.SetTexture("_BaseMap", texture);
            }
            newMat.SetColor("_BaseColor", tintColor);
            newMat.name = originalMat.name + "_Fixed";
            Debug.Log($"[FixPurpleMaterials] 使用 URP/Unlit 修复，贴图: {texture?.name ?? "无"}");
            return newMat;
        }
        
        // 最后尝试Standard着色器
        Shader standard = Shader.Find("Standard");
        if (standard != null)
        {
            newMat = new Material(standard);
            if (texture != null)
            {
                newMat.SetTexture("_MainTex", texture);
            }
            newMat.color = tintColor;
            newMat.name = originalMat.name + "_Fixed";
            Debug.Log($"[FixPurpleMaterials] 使用 Standard 修复，贴图: {texture?.name ?? "无"}");
            return newMat;
        }
        
        Debug.LogError($"[FixPurpleMaterials] 无法修复材质 {originalMat.name}：找不到合适的着色器");
        return null;
    }

    /// <summary>
    /// 仅修复道路材质
    /// </summary>
    private void FixRoadMaterials()
    {
        int fixedCount = 0;
        
        // 查找RoadManager下的所有Renderer
        RoadManager roadManager = FindObjectOfType<RoadManager>();
        if (roadManager == null)
        {
            EditorUtility.DisplayDialog("错误", "场景中未找到 RoadManager！", "确定");
            return;
        }

        Renderer[] renderers = roadManager.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            bool materialChanged = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                
                if (mat != null && NeedsFix(mat))
                {
                    Material fixedMat = FixMaterialShader(mat);
                    if (fixedMat != null)
                    {
                        materials[i] = fixedMat;
                        materialChanged = true;
                        fixedCount++;
                    }
                }
            }
            
            if (materialChanged)
            {
                renderer.sharedMaterials = materials;
            }
        }

        EditorUtility.DisplayDialog(
            "修复完成",
            $"已修复 {fixedCount} 个道路材质！\n贴图已保留。",
            "确定"
        );
    }
    
    /// <summary>
    /// 专门修复KajamansRoads资源的材质
    /// </summary>
    private void FixKajamansRoadsMaterials()
    {
        int fixedCount = 0;
        
        // 查找所有使用KajamansRoads预制体的物体
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // 检查是否是KajamansRoads的物体（通过名称判断）
            if (!obj.name.ToLower().Contains("km") && 
                !obj.name.ToLower().Contains("road") &&
                !obj.name.ToLower().Contains("l10") &&
                !obj.name.ToLower().Contains("l20"))
                continue;
            
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
                continue;
            
            Material[] materials = renderer.sharedMaterials;
            bool materialChanged = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                
                if (mat != null && NeedsFix(mat))
                {
                    Material fixedMat = FixMaterialShader(mat);
                    if (fixedMat != null)
                    {
                        materials[i] = fixedMat;
                        materialChanged = true;
                        fixedCount++;
                        Debug.Log($"[FixPurpleMaterials] 修复 KajamansRoads 材质: {obj.name} - {mat.name}");
                    }
                }
            }
            
            if (materialChanged)
            {
                renderer.sharedMaterials = materials;
            }
        }

        if (fixedCount == 0)
        {
            EditorUtility.DisplayDialog(
                "提示",
                "未发现 KajamansRoads 的紫色材质。\n\n可能原因:\n1. 材质已经正确\n2. 道路预制体未实例化\n3. 尚未播放场景",
                "确定"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "修复完成",
                $"已修复 {fixedCount} 个 KajamansRoads 材质！\n\n真实道路贴图已保留，只更换了着色器。",
                "确定"
            );
        }
    }

    /// <summary>
    /// 检测紫色材质
    /// </summary>
    private void DetectPurpleMaterials()
    {
        int purpleCount = 0;
        string report = "紫色材质检测报告:\n\n";
        
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in renderers)
        {
            if (NeedsFix(renderer))
            {
                purpleCount++;
                report += $"• {renderer.gameObject.name}\n";
                report += $"  路径: {GetGameObjectPath(renderer.gameObject)}\n";
                
                if (renderer.sharedMaterial != null)
                {
                    report += $"  着色器: {renderer.sharedMaterial.shader.name}\n";
                }
                else
                {
                    report += $"  材质: 缺失\n";
                }
                
                report += "\n";
            }
        }

        if (purpleCount == 0)
        {
            EditorUtility.DisplayDialog(
                "检测完成",
                "✅ 未发现紫色材质！场景材质正常。",
                "确定"
            );
        }
        else
        {
            Debug.Log(report);
            EditorUtility.DisplayDialog(
                "检测完成",
                $"⚠️ 发现 {purpleCount} 个紫色材质！\n\n详细信息已输出到 Console。",
                "确定"
            );
        }
    }

    /// <summary>
    /// 判断材质是否需要修复
    /// </summary>
    private bool NeedsFix(Material mat)
    {
        if (mat == null || mat.shader == null)
            return true;

        string shaderName = mat.shader.name;
        
        // 检查是否是错误着色器
        if (shaderName.Contains("Error") || 
            shaderName.Contains("Hidden/InternalErrorShader") ||
            shaderName.Contains("Hidden/InternalError"))
            return true;
        
        // 检查是否是Built-in着色器但在URP项目中
        if (IsURPProject() && IsBuiltInShader(shaderName))
        {
            Debug.Log($"[FixPurpleMaterials] 检测到Built-in着色器在URP项目中: {shaderName}");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 判断材质是否需要修复（Renderer版本）
    /// </summary>
    private bool NeedsFix(Renderer renderer)
    {
        if (renderer.sharedMaterial == null)
            return true;
        
        return NeedsFix(renderer.sharedMaterial);
    }
    
    /// <summary>
    /// 检查是否是URP项目
    /// </summary>
    private bool IsURPProject()
    {
        var pipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        return pipeline != null && pipeline.GetType().Name.Contains("Universal");
    }
    
    /// <summary>
    /// 检查是否是Built-in着色器
    /// </summary>
    private bool IsBuiltInShader(string shaderName)
    {
        return shaderName.Contains("Standard") || 
               shaderName.Contains("Diffuse") || 
               shaderName.Contains("Specular") ||
               shaderName.Contains("Mobile/") ||
               shaderName.Contains("Legacy Shaders/");
    }

    /// <summary>
    /// 创建默认材质
    /// </summary>
    private Material CreateDefaultMaterial(Color color)
    {
        Material mat = null;
        
        // 优先使用URP Unlit
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
        if (urpUnlit != null)
        {
            mat = new Material(urpUnlit);
            mat.SetColor("_BaseColor", color);
            return mat;
        }

        // 尝试URP Lit
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit != null)
        {
            mat = new Material(urpLit);
            mat.SetColor("_BaseColor", color);
            return mat;
        }

        // 回退到Unlit/Color
        Shader unlit = Shader.Find("Unlit/Color");
        if (unlit != null)
        {
            mat = new Material(unlit);
            mat.SetColor("_Color", color);
            return mat;
        }

        // 最后尝试Standard
        Shader standard = Shader.Find("Standard");
        if (standard != null)
        {
            mat = new Material(standard);
            mat.color = color;
            return mat;
        }

        Debug.LogError("[FixPurpleMaterials] 无法创建材质！");
        return null;
    }

    /// <summary>
    /// 根据物体名称推断颜色
    /// </summary>
    private Color GetColorFromName(string name)
    {
        name = name.ToLower();

        if (name.Contains("road") || name.Contains("路"))
            return new Color(0.2f, 0.2f, 0.2f); // 深灰色

        if (name.Contains("green") || name.Contains("belt") || name.Contains("绿"))
            return new Color(0.2f, 0.6f, 0.2f); // 绿色

        if (name.Contains("grass") || name.Contains("草"))
            return new Color(0.3f, 0.7f, 0.3f); // 浅绿色

        return new Color(0.5f, 0.5f, 0.5f); // 默认灰色
    }

    /// <summary>
    /// 根据物体推断默认颜色
    /// </summary>
    private Color GetDefaultColorForObject(GameObject obj)
    {
        return GetColorFromName(obj.name);
    }

    /// <summary>
    /// 获取GameObject的完整路径
    /// </summary>
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}
