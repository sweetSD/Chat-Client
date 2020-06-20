using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD. (with Singletone)
/// 
/// 디바이스 정보 매니저 스크립트입니다.
/// 
/// </summary>

public enum E_TOAST
{
    LENGTH_SHORT = 0,
    LENGTH_LONG = 1,
}

public class SDDeviceManager : SDSingleton<SDDeviceManager>
{
    [Header("Device")]

    /// <summary>
    /// 목표 FPS입니다.
    /// </summary>
    [Tooltip("목표 FPS")]
    [SerializeField] private int _targetFPS = 60;
    public int TargetFPS
    {
        get => _targetFPS;
        set
        {
            _targetFPS = value;
            Application.targetFrameRate = _targetFPS;
        }
    }

    /// <summary>
    /// 백그라운드 실행 여부
    /// </summary>
    [Tooltip("백그라운드 실행 여부")]
    [SerializeField] private bool _isRunInBackground = true;
    public bool runInBackground
    {
        get => _isRunInBackground;
        set
        {
            _isRunInBackground = value;
            Application.runInBackground = _isRunInBackground;
        }
    }

    /// <summary>
    /// 절전모드 실행 여부
    /// </summary>
    [Tooltip("절전모드 실행 여부")]
    [SerializeField] private bool _isNeverSleep = true;
    public bool isNeverSleep
    {
        get => _isNeverSleep;
        set
        {
            _isNeverSleep = value;
            Screen.sleepTimeout = _isNeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        }
    }

    /// <summary>
    /// 화면 가로 길이
    /// </summary>
    public float ScreenWidth => Screen.width;

    /// <summary>
    /// 화면 세로 길이
    /// </summary>
    public float ScreenHeight => Screen.height;

    private AndroidJavaObject _currentActivity = null;

    private void Awake()
    {
        Application.targetFrameRate = _targetFPS;
        Application.runInBackground = _isRunInBackground;
        Screen.sleepTimeout = _isNeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

        if(Application.platform == RuntimePlatform.Android)
        {
            var unityPlayer = new AndroidJavaObject("com.unity3d.player.UnityPlayer");
            _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // TEST
            _currentActivity.Call("showToast", "hello", E_TOAST.LENGTH_LONG);
        }
    }

}
