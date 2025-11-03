using UnityEditor;
using UnityEngine;
using System.Reflection;

public class FlutterExportHelper : EditorWindow
{
    [MenuItem("Tools/Flutter Export Helper")]
    static void ShowWindow()
    {
        GetWindow<FlutterExportHelper>("Flutter Export");
    }

    void OnGUI()
    {
        GUILayout.Label("Flutter Unity Export Helper", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Export Android (Debug)", GUILayout.Height(40)))
        {
            ExportAndroid(false);
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Export Android (Release)", GUILayout.Height(40)))
        {
            ExportAndroid(true);
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Export iOS (Debug)", GUILayout.Height(40)))
        {
            ExportIOS(false);
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("Export iOS (Release)", GUILayout.Height(40)))
        {
            ExportIOS(true);
        }
    }
    
    void ExportAndroid(bool isRelease)
    {
        // 使用反射调用 Build 类的方法
        var buildType = System.Type.GetType("FlutterUnityIntegration.Editor.Build, Assembly-CSharp-Editor");
        if (buildType == null)
        {
            Debug.LogError("找不到 FlutterUnityIntegration.Editor.Build 类！");
            Debug.LogError("请确保 FlutterUnityIntegration 包已正确导入。");
            return;
        }
        
        string methodName = isRelease ? "DoBuildAndroidLibraryRelease" : "DoBuildAndroidLibraryDebug";
        var method = buildType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        
        if (method != null)
        {
            Debug.Log($"开始导出 Android ({(isRelease ? "Release" : "Debug")})...");
            method.Invoke(null, null);
        }
        else
        {
            Debug.LogError($"找不到方法: {methodName}");
        }
    }
    
    void ExportIOS(bool isRelease)
    {
        var buildType = System.Type.GetType("FlutterUnityIntegration.Editor.Build, Assembly-CSharp-Editor");
        if (buildType == null)
        {
            Debug.LogError("找不到 FlutterUnityIntegration.Editor.Build 类！");
            return;
        }
        
        string methodName = isRelease ? "DoBuildIOSLibraryRelease" : "DoBuildIOSLibraryDebug";
        var method = buildType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        
        if (method != null)
        {
            Debug.Log($"开始导出 iOS ({(isRelease ? "Release" : "Debug")})...");
            method.Invoke(null, null);
        }
        else
        {
            Debug.LogError($"找不到方法: {methodName}");
        }
    }
}
