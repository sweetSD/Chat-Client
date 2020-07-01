using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

public enum E_NETWORK
{
    MAX_BUFFER_SIZE = 1024
}

public class NetworkManager : SDSingleton<NetworkManager>
{
    // 서버로부터 부여받은 자신의 클라이언트 Id
    private int _clientId = -1;
    public int ClientId => _clientId;

    // 서버와 통신할 틱레이트 (초당)
    [SerializeField] private int _tickRate = 16;
    public int TickRate
    {
        get => _tickRate;
        set
        {
            _tickRate = value;
            _processDelay = new WaitForSeconds(1 / _tickRate);
        }
    }

    // 서버와 직접적으로 통신하게 될 객체들
    TcpClient _tcpClient = null;
    NetworkStream _networkStream = null;
    Task _readTask = null;

    // 서버와의 통신 프로세스 객체들
    Coroutine _processCoroutine = null;
    WaitForSeconds _processDelay = null;

    // 서버로부터 받아온 모든 Network 객체들 리스트
    List<NetworkView> _networkViews = new List<NetworkView>();
    List<UnityAction> _invokeMainActions = new List<UnityAction>();

    [Header("Server Info")]
    [SerializeField] private string _serverAddress;

    [Header("Debugging")]
    [SerializeField] private SDDebugPerSecond _sendDPS;
    [SerializeField] private SDDebugPerSecond _recvDPS;

    [Header("Playable")]
    [SerializeField] private NetworkView _playerPrefab;

    private void Awake()
    {
        SetInstance(this);

        InitalizeSocket();
    }

    private void OnEnable()
    {
        if (_processCoroutine == null)
            _processCoroutine = StartCoroutine(CO_Process());
    }

    private void OnDisable()
    {
        if (_processCoroutine != null)
            StopCoroutine(_processCoroutine);
    }

    private void OnDestroy()
    {
        if(_networkStream != null)
            _networkStream.Close();
        if(_tcpClient != null)
            _tcpClient.Close();
        if(_readTask != null)
            _readTask.Dispose();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _invokeMainActions.Count; i++)
        {
            _invokeMainActions[i]?.Invoke();
        }
        _invokeMainActions.Clear();
    }

    // 서버와 통신 초기화 함수
    private void InitalizeSocket()
    {
        _tcpClient = new TcpClient();
        try
        {
            _tcpClient.Connect(_serverAddress, 4000);
            _networkStream = _tcpClient.GetStream();
        }
        catch (System.Exception e)
        {
            Debug.LogError("서버에 연결하는 중 예외가 발생하였습니다. " + e.Message);
        }
        _readTask = Task.Run(ReadProcess);
    }

    // 서버의 응답을 기다리는 Task 함수
    private async void ReadProcess()
    {
        var buffer = new byte[(int)E_NETWORK.MAX_BUFFER_SIZE];
        while(true)
        {
            var result = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
            if (result > 0)
            {
                Packet packet = new Packet(true);
                //Debug.Log($"Read {result} bytes data from server.");
                try
                {
                    packet.SetBuffer(buffer, result);
                    PacketProcess(packet);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error occured while packet processing.. {e.Message}");
                }
                if (_recvDPS != null)
                    _recvDPS.Report();
            }
        }
    }

    // 서버로부터 받아온 패킷을 처리할 함수
    private void PacketProcess(Packet packet)
    {
        var header = packet.Pop<Header>();
        string nickname = packet.Pop(header.nameSize);

        switch (header.type)
        {
            case PacketType.NONE:
                break;
            case PacketType.SET_CLIENT_IDENTITY:
                _clientId = header.clientId;
                Debug.Log($"Client Identity set to {_clientId}.");
                _invokeMainActions.Add(() =>
                {
                    var playable = Instantiate(_playerPrefab);
                    playable.Initialize(_clientId);
                    _networkViews.Add(playable);
                });

                for (int i = 0; i < header.dataSize; i++)
                {
                    var clientInfo = packet.Pop<Header>();
                    if(clientInfo.type == PacketType.CLIENT_LIST)
                        _invokeMainActions.Add(() =>
                        {
                            var playable = Instantiate(_playerPrefab);
                            playable.Initialize(clientInfo.clientId);
                            _networkViews.Add(playable);
                        });
                }
                break;
            case PacketType.MESSAGE:
                string message = packet.Pop(header.dataSize);
                _invokeMainActions.Add(() =>
                {
                    Debug.Log($"{header.clientId} / {nickname} / {message}");
                    SDUIManager.I.ChatBoxPool.ActiveObject(Vector3.zero, Vector3.zero, Vector3.one)
                        .GetComponent<ChatBox>()
                        .SetData($"{header.clientId.ToString()}\n{nickname}", message);
                });
                break;
            case PacketType.TRANSLATE:
                for (int i = 0; i < _networkViews.Count; i++)
                {
                    if (_networkViews[i].NetworkID.Equals(header.clientId))
                    {
                        int index = i;
                        _invokeMainActions.Add(() =>
                        {
                            _networkViews[index].OnDeserializeView(packet);
                        });
                    }
                }
                break;
            case PacketType.CLIENT_CONNECTED:
                Debug.Log($"New Client Connected. {header.clientId}.");
                _invokeMainActions.Add(() =>
                {
                    var playable = Instantiate(_playerPrefab);
                    playable.Initialize(header.clientId);
                    _networkViews.Add(playable);
                });
                break;
            case PacketType.CLIENT_DISCONNECTED:
                _invokeMainActions.Add(() =>
                {
                    for (int i = 0; i < _networkViews.Count; i++)
                    {
                        if(_networkViews[i].NetworkID.Equals(header.clientId))
                        {
                            Destroy(_networkViews[i].gameObject);
                            _networkViews.RemoveAt(i);
                        }
                    }
                });
                break;
            case PacketType.CLIENT_LIST:
                break;
        }
    }

    // 서버로 데이터를 보내는 프로세스 코루틴 함수
    IEnumerator CO_Process()
    {
        _processDelay = new WaitForSeconds(1f / _tickRate);
        while(true)
        {
            if (_sendDPS != null)
                _sendDPS.Report();
            if (_networkStream != null && _clientId != ~0)
            {
                for (int i = 0; i < _networkViews.Count; i++)
                {
                    if (_networkViews[i].NetworkID.Equals(_clientId))
                    {
                        Packet packet = null;
                        packet = _networkViews[i].OnSerializeView(packet);
                        if(packet != null)
                            SendToServer(packet.Buffer, packet.PushBufferPosition);
                    }
                }
            }
            yield return _processDelay;
        }
    }

    public void SendMessageToServer(string message)
    {
        var nameBytes = System.Text.Encoding.UTF8.GetBytes("sweetSD");
        var msgBytes = Encoding.UTF8.GetBytes(message);
        Packet packet = new Packet(false, new Header(_type: PacketType.MESSAGE, _nameSize: nameBytes.Length, _size: msgBytes.Length));
        packet.Push(nameBytes);
        packet.Push(msgBytes);
        SendToServer(packet.Buffer, packet.PushBufferPosition);
        SDUIManager.I.ChatBoxPool.ActiveObject(Vector3.zero, Vector3.zero, Vector3.one)
                    .GetComponent<ChatBox>()
                    .SetData(_clientId.ToString(), message);
    }

    public void SendToServer(byte[] buffer, int size = -1)
    {
        if (_networkStream == null)
            return;
        _networkStream.Write(buffer, 0, size != -1 ? size : buffer.Length);
    }
}
