using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T I
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();
            if (_instance == null)
            {
                throw new System.Exception($"{typeof(T)}의 인스턴스를 찾을 수 없습니다.");
            }
            return _instance;
        }
    }

    protected void SetInstance(T instance, bool dontDestroyOnLoad = true)
    {
        _instance = instance;
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(instance);
    }

}
