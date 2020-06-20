using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public enum PacketType : short
{
    NONE = 0x00,
    MESSAGE,
    TRANSLATE,
    SET_CLIENT_IDENTITY,
    CLIENT_CONNECTED,
    CLIENT_DISCONNECTED,
    CLIENT_LIST
};

public struct Header
{
    public int clientId;
	public PacketType type;
    public int dataSize;

    public Header(PacketType _type = PacketType.NONE, int id = -1, int size = (int)E_NETWORK.MAX_BUFFER_SIZE)
    {
        if (id == -1)
            this.clientId = NetworkManager.I.ClientId;
        else
            this.clientId = id;
        this.type = _type;
        this.dataSize = size;
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

    public T Pop<T>()
    {
        int size = Marshal.SizeOf<T>();
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

    public void Clear()
    {
        _buffer = new byte[BufferSize];
        _pushBufferPosition = 0;
        _popBufferPosition = 0;
    }
}
