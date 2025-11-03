using UnityEditor;
using UnityEngine;

public class TestMenu : EditorWindow
{
    [MenuItem("TestMenu/Hello World")]
    static void HelloWorld()
    {
        Debug.Log("Menu is working!");
    }
}
