using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SDSceneManager : SDSingleton<SDSceneManager>
{
    private Coroutine _progressCoroutine = null;
    [SerializeField] private UnityEvent<float> _onSceneLoadProgress;
    [SerializeField] private UnityEvent<float> _onSceneLoaded;

    private void Awake()
    {
        SetInstance(this);
    }

    #region Logic Functions

    public void LoadScene(int index)
    {
        var ao = SceneManager.LoadSceneAsync(index);
        if (_progressCoroutine != null)
            StopCoroutine(_progressCoroutine);
        _progressCoroutine = StartCoroutine(CO_SceneLoadProgress(ao));
    }

    public void LoadScene(string name)
    {
        var ao = SceneManager.LoadSceneAsync(name);
        if (_progressCoroutine != null)
            StopCoroutine(_progressCoroutine);
        _progressCoroutine = StartCoroutine(CO_SceneLoadProgress(ao));
    }

    private IEnumerator CO_SceneLoadProgress(AsyncOperation operation)
    {
        while (operation.progress < 9.0f)
        {
            _onSceneLoadProgress?.Invoke(operation.progress);
            yield return null;
        }
        _onSceneLoaded?.Invoke(operation.progress);
    }

    #endregion

    #region Event Functions

    public void AddProgressListener(UnityAction<float> action)
    {
        _onSceneLoadProgress.AddListener(action);
    }

    public void RemoveProgressListener(UnityAction<float> action)
    {
        _onSceneLoadProgress.RemoveListener(action);
    }

    public void AddLoadedListener(UnityAction<float> action)
    {
        _onSceneLoaded.AddListener(action);
    }

    public void RemoveLoadedListener(UnityAction<float> action)
    {
        _onSceneLoaded.RemoveListener(action);
    }

    #endregion
}
