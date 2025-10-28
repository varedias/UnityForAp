using UnityEngine;

/// <summary>
/// 输入管理器 - 兼容旧输入系统和新输入系统
/// </summary>
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("InputManager");
                instance = go.AddComponent<InputManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    /// <summary>
    /// 检测空格键是否按下
    /// </summary>
    public static bool GetSpaceKeyDown()
    {
        #if ENABLE_INPUT_SYSTEM
        // 新输入系统
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            return UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame;
        }
        return false;
        #else
        // 旧输入系统
        return Input.GetKeyDown(KeyCode.Space);
        #endif
    }

    /// <summary>
    /// 检测任意键是否按下（新输入系统）
    /// </summary>
    public static bool GetKeyDown(KeyCode keyCode)
    {
        #if ENABLE_INPUT_SYSTEM
        // 新输入系统 - 需要映射 KeyCode 到新 API
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            
            // 映射常用按键
            switch (keyCode)
            {
                case KeyCode.Space:
                    return keyboard.spaceKey.wasPressedThisFrame;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    return keyboard.enterKey.wasPressedThisFrame;
                case KeyCode.Escape:
                    return keyboard.escapeKey.wasPressedThisFrame;
                case KeyCode.R:
                    return keyboard.rKey.wasPressedThisFrame;
                default:
                    Debug.LogWarning($"[InputManager] KeyCode {keyCode} 未映射到新输入系统");
                    return false;
            }
        }
        return false;
        #else
        // 旧输入系统
        return Input.GetKeyDown(keyCode);
        #endif
    }

    /// <summary>
    /// 检测鼠标点击
    /// </summary>
    public static bool GetMouseButtonDown(int button)
    {
        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            switch (button)
            {
                case 0: return mouse.leftButton.wasPressedThisFrame;
                case 1: return mouse.rightButton.wasPressedThisFrame;
                case 2: return mouse.middleButton.wasPressedThisFrame;
                default: return false;
            }
        }
        return false;
        #else
        return Input.GetMouseButtonDown(button);
        #endif
    }

    /// <summary>
    /// 获取鼠标位置
    /// </summary>
    public static Vector3 GetMousePosition()
    {
        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            var pos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            return new Vector3(pos.x, pos.y, 0);
        }
        return Vector3.zero;
        #else
        return Input.mousePosition;
        #endif
    }

    /// <summary>
    /// 检测触摸输入（移动端）
    /// </summary>
    public static bool GetTouchDown()
    {
        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Touchscreen.current != null)
        {
            return UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        }
        return false;
        #else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        #endif
    }
}
