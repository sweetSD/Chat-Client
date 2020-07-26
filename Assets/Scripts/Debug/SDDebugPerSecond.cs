using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by sweetSD.
/// 
/// 특정 시간동안 호출된 횟수를 GUI로 출력하는 디버깅 클래스입니다.
/// </summary>
public class SDDebugPerSecond : MonoBehaviour
{
    /// <summary>
    /// 총 호출된 횟수
    /// </summary>
    private int _reportCount = 0;

    /// <summary>
    /// 초당 호출된 횟수
    /// </summary>
    private int _reportCountPerSecond = 0;
    public int CountPerSecond => _reportCountPerSecond;

    /// <summary>
    /// 매 시간마다 횟수를 처리 및 기록하는 코루틴
    /// </summary>
    private Coroutine _debuggingCor = null;

    /// <summary>
    /// 업데이트 시간
    /// </summary>
    [SerializeField] private float _refreshSeconds = 1f;
    
    [Header("GUI")]

    /// <summary>
    /// GUI 표시 여부
    /// </summary>
    [SerializeField] private bool _isGUI = true;

    /// <summary>
    /// GUI 텍스트 접두사
    /// </summary>
    [SerializeField] private string _guiLabelPrefix = "DPS";

    /// <summary>
    /// GUI 텍스트 접미사
    /// </summary>
    [SerializeField] private string _guiLabelSuffix = "Called.";

    /// <summary>
    /// GUI가 표시 될 화면 좌표 (0, 0) => left top
    /// </summary>
    [SerializeField] private Vector2 _guiPos = Vector2.zero;

    /// <summary>
    /// 호출 횟수를 증가합니다. 매 Refresh Seconds마다 호출 횟수가 기록되고 초기화됩니다.
    /// </summary>
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
        var cachedWait = new WaitForSeconds(_refreshSeconds);
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
