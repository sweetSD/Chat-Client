using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
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
            var vp = Camera.main.WorldToViewportPoint(_targetTransform.position);
            var sp = new Vector2(
             ((vp.x * _rectTransform.sizeDelta.x) - (_rectTransform.sizeDelta.x * 0.5f)),
             ((vp.y * _rectTransform.sizeDelta.y) - (_rectTransform.sizeDelta.y * 0.5f)));
            _rectTransform.anchoredPosition = sp;
        }
    }
}
