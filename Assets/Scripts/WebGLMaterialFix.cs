using UnityEngine;

public class WebGLMaterialFix : MonoBehaviour
{
    void Start()
    {
        // 在 WebGL 运行时，如果材质显示不正确，使用默认材质
        #if UNITY_WEBGL && !UNITY_EDITOR
        FixMaterials();
        #endif
    }

    void FixMaterials()
    {
        // 查找所有 MeshRenderer
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>();
        
        foreach (MeshRenderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                // 如果材质显示为粉色/蓝色（着色器错误），尝试修复
                if (mat.shader == null || mat.shader.name.Contains("Hidden"))
                {
                    Debug.LogWarning($"修复材质: {mat.name} 在 {renderer.gameObject.name}");
                    mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                }
            }
        }
        
        Debug.Log("WebGL 材质检查完成");
    }
}
