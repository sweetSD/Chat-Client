using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public enum PacketType : int
{
    NONE = 0x00,
    MESSAGE,
    TRANSLATE,
    SET_CLIENT_IDENTITY,
    CLIENT_CONNECTED,
    CLIENT_DISCONNECTED,
    CLIENT_LIST
};

[Serializable]
public struct Header
{
    public int clientId;
	public PacketType type;
    public int nameSize;
    public int dataSize;

    public Header(PacketType _type = PacketType.NONE, int _nameSize = 0, int _size = (int)E_NETWORK.MAX_BUFFER_SIZE, int _id = -1)
    {
        this.clientId = _id == -1 ? NetworkManager.I.ClientId : _id;
        this.type = _type;
        this.nameSize = _nameSize;
        this.dataSize = _size;
    }
};

public class Packet
{
    // 버퍼 데이터 입력 위치입니다. (실제 버퍼 안에 들어있는 데이터 크기)
    private int _pushBufferPosition = 0;
    public int PushBufferPosition => _pushBufferPosition;

    // 버퍼 데이터 출력 위치입니다.
    private int _popBufferPosition = 0;
    public int PopBufferPosition => _popBufferPosition;

    // 버퍼 데이터입니다.
    private byte[] _buffer = null;
    public byte[] Buffer => _buffer;

    // 버퍼 데이터 최대 사이즈입니다.
    private int _bufferSize;
    public int BufferSize => _bufferSize;

    public Packet(bool readable = false, Header header = new Header(), int bufferSize = (int)E_NETWORK.MAX_BUFFER_SIZE)
    {
        if (!readable)
        {
            _bufferSize = bufferSize;
            _buffer = new byte[_bufferSize];
            Push(header, Marshal.SizeOf<Header>());
        }
    }

    public void SetBuffer(byte[] buffer, int size)
    {
        _buffer = buffer;
        _pushBufferPosition = size;
        _bufferSize = size;
    }

    /// <summary>
    /// 구조체를 데이터에 추가합니다.
    /// </summary>
    /// <param name="obj">추가하려는 데이터</param>
    /// <param name="size">추가하려는 데이터 크기</param>
    public void Push(object obj, int size)
    {
        if (PushBufferPosition + size >= BufferSize)
            return;

        try
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, _buffer, PushBufferPosition, size);
            Marshal.FreeHGlobal(ptr);
            _pushBufferPosition += size;
        }
        catch { }
    }

    /// <summary>
    /// 문자열을 데이터에 추가합니다.
    /// </summary>
    /// <param name="msg">추가하려는 문자열</param>
    /// <param name="size">추가하려는 문자열 크기</param>
    public int Push(byte[] bytes)
    {
        int size = bytes.Length;

        if (PushBufferPosition + size >= BufferSize)
            return -1;

        try
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            //ptr = Marshal.StringToHGlobalAnsi(msg);
            Marshal.Copy(bytes, 0, ptr, size);
            Marshal.Copy(ptr, _buffer, PushBufferPosition, size);
            Marshal.FreeHGlobal(ptr);
            _pushBufferPosition += size;
        }
        catch { }

        return size;
    }

    /// <summary>
    /// 데이터에서 T의 데이터를 가져옵니다.
    /// </summary>
    /// <typeparam name="T">가져올 데이터 타입</typeparam>
    /// <returns></returns>
    public T Pop<T>()
    {
        int size = Marshal.SizeOf<T>();
        if (size <= 0)
            return default(T);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        T obj = default(T);
        try
        {
            Marshal.Copy(_buffer, _popBufferPosition, ptr, size);
            obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            _popBufferPosition += size;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return obj;
    }

    /// <summary>
    /// 데이터에서 문자열을 가져옵니다.
    /// </summary>
    /// <param name="size">가져올 문자열의 길이</param>
    /// <returns></returns>
    public string Pop(int size)
    {
        if (size <= 0)
            return string.Empty;
        IntPtr ptr = Marshal.AllocHGlobal(size);
        byte[] bytes = new byte[size];
        string msg = string.Empty;
        try
        {
            Marshal.Copy(_buffer, _popBufferPosition, ptr, size);
            Marshal.Copy(ptr, bytes, 0, size);
            msg = System.Text.Encoding.UTF8.GetString(bytes);
            _popBufferPosition += size;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return msg;
    }

    public void Clear()
    {
        _buffer = new byte[BufferSize];
        _pushBufferPosition = 0;
        _popBufferPosition = 0;
    }
}
