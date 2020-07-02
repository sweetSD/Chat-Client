using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    private Transform _targetTransform;

    private void Awake()
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(Transform targetTransform)
    {
        _targetTransform = targetTransform;
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
