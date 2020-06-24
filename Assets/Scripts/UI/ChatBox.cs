using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    [SerializeField] private Text _senderText;
    [SerializeField] private Text _contentText;

    public void SetData(string sender, string content)
    {
        _senderText.text = sender;
        _contentText.text = content;
    }
}
