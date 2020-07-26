using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _canvasRectTransform;
    [SerializeField] private Text _text;

    [SerializeField] private Transform _targetTransform;
    public Transform TargetTransform => _targetTransform;
    [SerializeField] private Transform _graphicRootTransform;
    public Transform GraphicRootTransform => _graphicRootTransform;
    [SerializeField] private float _chatBubbleHoldDelay = 5;

    private void Awake()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
        if (_canvasRectTransform == null)
            _canvasRectTransform = transform.root.Find("Canvas").GetComponent<RectTransform>();
    }

    public void Initialize(Transform targetTransform)
    {
        _targetTransform = targetTransform;
    }

    public void SetData(string sender, string message)
    {
        _text.text = $"{sender}:{message}";
        SDObjectManager.I.StopSetActive(_graphicRootTransform.gameObject);
        _graphicRootTransform.gameObject.SetActive(true);
        _graphicRootTransform.gameObject.SetActive(false, _chatBubbleHoldDelay);
    }

    private void Update()
    {
        if (_targetTransform != null)
        {
            var vp = Camera.main.WorldToViewportPoint(_targetTransform.position + Vector3.up * 1.2f);
            var sp = new Vector2(
             ((vp.x * _canvasRectTransform.sizeDelta.x) - (_canvasRectTransform.sizeDelta.x * 0.5f)),
             ((vp.y * _canvasRectTransform.sizeDelta.y) - (_canvasRectTransform.sizeDelta.y * 0.5f)));
            _rectTransform.anchoredPosition = sp;
        }
    }
}
