using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SDUIManager : SDSingleton<SDUIManager>
{
    [Header("Controller")]
    [SerializeField] private JoyStick _movementJoyStick;
    public JoyStick MovementJoyStick => _movementJoyStick;

    [SerializeField] private DragDetector _cameraControlDetector;
    public DragDetector CameraControlDetector => _cameraControlDetector;

    [Header("Chatting")]
    [SerializeField] private InputField _nicknameInputField;
    [SerializeField] private InputField _chatInputField;

    [SerializeField] private SDObjectPool _chatBoxPool;
    public SDObjectPool ChatBoxPool => _chatBoxPool;

    [SerializeField] private SDObjectPool _chatBubblePool;
    public SDObjectPool ChatBubblePool => _chatBubblePool;
    [SerializeField] private RectTransform _chatListTransform;


    private void Awake()
    {
        SetInstance(this, false);

        _nicknameInputField.onEndEdit.AddListener((value) =>
        {
            NetworkManager.I.Nickname = value.IsEmpty() ? "User" : value;
            _nicknameInputField.text = NetworkManager.I.Nickname;
            _nicknameInputField.interactable = false;
        });

        _chatInputField.onEndEdit.AddListener((value) =>
        {
            if(value.IsNotEmpty())
            {
                NetworkManager.I.SendMessageToServer(value);
                _chatInputField.text = string.Empty;
            }
            _chatInputField.interactable = false;
        });
    }

    public void AddChatUI(string sender, string message, Transform targetTransform)
    {
        _chatBoxPool.ActiveObject<ChatBox>(Vector3.zero, Vector3.zero, Vector3.one)
                    .SetData(sender.ToString(), message);

        if (_chatBubblePool.ObjectPool.Any((e) => e.GetComponent<ChatBubble>().TargetTransform == targetTransform))
        {
            _chatBubblePool.ObjectPool.Find((e) => e.GetComponent<ChatBubble>().TargetTransform == targetTransform)
                .GetComponent<ChatBubble>().SetData(sender, message);
        }
        else
        {
            var chatBubble = _chatBubblePool.ActiveObject<ChatBubble>(targetTransform.position, Vector3.zero, Vector3.one);
            chatBubble.transform.SetAsLastSibling();
            chatBubble.Initialize(targetTransform);
            chatBubble.SetData(sender, message);
        }
    }

    #region On Click Event Functions

    public void OnClick_ToggleChatList()
    {
        _chatListTransform.gameObject.SetActive(!_chatListTransform.gameObject.activeSelf);
    }

    #endregion
}
