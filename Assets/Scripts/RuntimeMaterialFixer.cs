using UnityEngine;
using System.Collections;

/// <summary>
/// 运行时材质自动修复器 - 自动修复紫色材质
/// </summary>
public class RuntimeMaterialFixer : MonoBehaviour
{
    [Header("自动修复设置")]
    [Tooltip("自动修复材质")]
    [SerializeField] private bool autoFix = true;
    
    [Tooltip("修复延迟（秒）")]
    [SerializeField] private float fixDelay = 0.5f;

    void Start()
    {
        if (autoFix)
        {
            StartCoroutine(AutoFixMaterials());
        }
    }

    private IEnumerator AutoFixMaterials()
    {
        // 等待物体实例化
        yield return new WaitForSeconds(fixDelay);
        
        Debug.Log("[RuntimeMaterialFixer] 开始自动修复材质...");
        
        int fixedCount = 0;
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in allRenderers)
        {
            Material[] materials = renderer.sharedMaterials;
            bool changed = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                
                if (mat != null && NeedsFixing(mat))
                {
                    Material newMat = FixMaterial(mat);
                    if (newMat != null)
                    {
                        materials[i] = newMat;
                        changed = true;
                        fixedCount++;
                    }
                }
            }
            
            if (changed)
            {
                renderer.sharedMaterials = materials;
            }
        }
        
        if (fixedCount > 0)
        {
            Debug.Log($"[RuntimeMaterialFixer] ✅ 自动修复了 {fixedCount} 个材质（保留贴图）");
        }
        else
        {
            Debug.Log("[RuntimeMaterialFixer] ✅ 所有材质正常");
        }
    }

    private bool NeedsFixing(Material mat)
    {
        if (mat.shader == null) return true;
        
        string shaderName = mat.shader.name;
        
        return shaderName.Contains("Error") || 
               shaderName.Contains("Hidden/InternalErrorShader") ||
               shaderName.Contains("Standard") || 
               shaderName.Contains("Mobile/") ||
               shaderName.Contains("Legacy Shaders/");
    }

    private Material FixMaterial(Material original)
    {
        // 提取贴图
        Texture mainTex = original.HasProperty("_MainTex") ? original.GetTexture("_MainTex") : null;
        Color color = original.HasProperty("_Color") ? original.GetColor("_Color") : Color.white;
        
        // 创建URP材质
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
        if (urpShader == null)
        {
            urpShader = Shader.Find("Universal Render Pipeline/Unlit");
        }
        
        if (urpShader != null)
        {
            Material newMat = new Material(urpShader);
            
            if (mainTex != null)
            {
                newMat.SetTexture("_BaseMap", mainTex);
                Debug.Log($"[RuntimeMaterialFixer] 保留贴图: {mainTex.name}");
            }
            
            newMat.SetColor("_BaseColor", color);
            newMat.name = original.name + "_Fixed";
            
            return newMat;
        }
        
        Debug.LogError("[RuntimeMaterialFixer] 无法找到URP着色器");
        return null;
    }
}
