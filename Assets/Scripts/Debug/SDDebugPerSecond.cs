using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDDebugPerSecond : MonoBehaviour
{
    private int _reportCount = 0;
    private int _reportCountPerSecond = 0;
    public int CountPerSecond => _reportCountPerSecond;

    private Coroutine _debuggingCor = null;

    [Header("GUI")]
    [SerializeField] private bool _isGUI = true;
    [SerializeField] private string _guiLabelPrefix = "DPS";
    [SerializeField] private string _guiLabelSuffix = "Called.";
    [SerializeField] private Vector2 _guiPos = Vector2.zero;

    public void Report()
    {
        _reportCount++;
    }

    private void OnEnable()
    {
        StartProcess();
    }

    private void OnDisable()
    {
        StopProcess();
    }

    public void StartProcess()
    {
        StopProcess();
        _debuggingCor = StartCoroutine(CO_DebugProcess());
    }

    public void StopProcess()
    {
        if (_debuggingCor != null)
            StopCoroutine(_debuggingCor);
    }

    private IEnumerator CO_DebugProcess()
    {
        var cachedWait = new WaitForSeconds(1);
        while(true)
        {
            yield return cachedWait;
            _reportCountPerSecond = _reportCount;
            _reportCount = 0;
        }
    }

    private void OnGUI()
    {
        if(_isGUI)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 50;
            GUI.Label(new Rect(_guiPos.x, _guiPos.y, 500, 100), $"{_guiLabelPrefix} - {_reportCountPerSecond} {_guiLabelSuffix}.", style);
        }
    }
}
