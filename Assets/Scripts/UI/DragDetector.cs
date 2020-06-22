using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDetector : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private bool _isToucing = false;

    public bool IsToucing => _isToucing;

    private Vector2 _prevPointerPos;

    private Vector2 _curPointerPos;
    public Vector2 DragDelta => _curPointerPos - _prevPointerPos;

    public virtual void OnBeginDrag(PointerEventData ped)
    {
        _isToucing = true;
        _curPointerPos = _prevPointerPos = ped.position;
    }

    public virtual void OnEndDrag(PointerEventData ped)
    {
        _isToucing = false;
        _prevPointerPos = _curPointerPos;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        _curPointerPos = ped.position;
    }

    private void LateUpdate()
    {
        _prevPointerPos = _curPointerPos;
    }
}
