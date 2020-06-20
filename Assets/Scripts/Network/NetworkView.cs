using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct CustomVector3
{
    public float x;
    public float y;
    public float z;

    public CustomVector3(float value)
    {
        x = y = z = value;
    }

    public CustomVector3(Vector3 _vector)
    {
        x = _vector.x;
        y = _vector.y;
        z = _vector.z;
    }

    public CustomVector3(float _x, float _y)
    {
        x = _x;
        y = _y;
        z = 0;
    }

    public CustomVector3(float _x, float _y, float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public override string ToString()
    {
        return $"{{{this.x}, {this.y}, {this.z}}}";
    }
}

public class NetworkView : MonoBehaviour
{
    [SerializeField] private Transform _tartgetTransform;

    private bool _isInitialized = false;

    private int _networkId = -1;
    public int NetworkID => _networkId;

    public bool isMine => _networkId.Equals(NetworkManager.I.ClientId);

    public void Initialize(int id)
    {
        if (_isInitialized)
            return;
        _networkId = id;
        _isInitialized = true;
        if (_tartgetTransform == null)
            _tartgetTransform = GetComponent<Transform>();
    }

    public virtual Packet OnSerializeView(Packet packet)
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDeserializeView(Packet packet)
    {
        throw new System.NotImplementedException();
    }
}
