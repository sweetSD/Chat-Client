using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDUIManager : SDSingleton<SDUIManager>
{

    [SerializeField] private JoyStick _movementJoyStick;
    public JoyStick MovementJoyStick => _movementJoyStick;

    [SerializeField] private DragDetector _cameraControlDetector;
    public DragDetector CameraControlDetector => _cameraControlDetector;

    [SerializeField] private SDObjectPool _chatBoxPool;
    public SDObjectPool ChatBoxPool => _chatBoxPool;

    [SerializeField] private InputField _chatInputField;

    [SerializeField] private Image _image;

    private void Awake()
    {
        SetInstance(this, false);

        _chatInputField.onEndEdit.AddListener((value) =>
        {
            if(value.IsNotEmpty())
            {
                NetworkManager.I.SendMessageToServer(value);
                _chatInputField.text = string.Empty;
            }
        });
    }

    /// <summary>
    /// 말풍선 캐릭터 위치 생성 테스트
    /// </summary>
    public void test()
    {
        var rt = _image.canvas.GetComponent<RectTransform>();
        var vp = Camera.main.WorldToViewportPoint(FindObjectOfType<NetworkMovement>().transform.position);
        var sp = new Vector2(
         ((vp.x * rt.sizeDelta.x) - (rt.sizeDelta.x * 0.5f)),
         ((vp.y * rt.sizeDelta.y) - (rt.sizeDelta.y * 0.5f)));
        _image.rectTransform.anchoredPosition = sp;
    }

    private void Update()
    {
        test();
    }
}
