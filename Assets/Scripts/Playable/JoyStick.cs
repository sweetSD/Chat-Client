using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image _joyBG;
    [SerializeField] private Image _joyStick;
    private Vector3 _inputVector;

    public float Horizontal => _inputVector.x;
    public float Vertical => _inputVector.y;
    public bool HasValue => _inputVector.sqrMagnitude > 0;

    void Start()
    {
        if(_joyBG == null)
            _joyBG = GetComponent<Image>();
        if(_joyStick == null)
            _joyStick = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joyBG.rectTransform,
            ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / _joyBG.rectTransform.sizeDelta.x);
            pos.y = (pos.y / _joyBG.rectTransform.sizeDelta.y);

            _inputVector = new Vector3(pos.x * 2, pos.y * 2, 0);
            _inputVector.Normalize();

            _joyStick.rectTransform.anchoredPosition = new Vector3(_inputVector.x * (_joyBG.rectTransform.sizeDelta.x / 3),
                _inputVector.y * (_joyBG.rectTransform.sizeDelta.y / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        _inputVector = Vector3.zero;
        _joyStick.rectTransform.anchoredPosition = Vector3.zero;
    }
}
