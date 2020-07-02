using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SDUIManager : SDSingleton<SDUIManager>
{
    [Header("Controller")]
    [SerializeField] private JoyStick _movementJoyStick;
    public JoyStick MovementJoyStick => _movementJoyStick;

    [SerializeField] private DragDetector _cameraControlDetector;
    public DragDetector CameraControlDetector => _cameraControlDetector;

    [Header("Input Field")]
    [SerializeField] private InputField _chatInputField;

    [Header("Object Pools")]
    [SerializeField] private SDObjectPool _chatBoxPool;
    public SDObjectPool ChatBoxPool => _chatBoxPool;

    [SerializeField] private SDObjectPool _chatBubblePool;
    public SDObjectPool ChatBubblePool => _chatBubblePool;

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
}
