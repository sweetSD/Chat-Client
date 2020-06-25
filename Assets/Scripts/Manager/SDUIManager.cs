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
