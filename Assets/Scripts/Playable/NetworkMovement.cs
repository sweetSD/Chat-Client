using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMovement : NetworkView
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Animator _animator;

    private Vector3 _velocity = Vector3.zero;

    private void Awake()
    {
        Camera.main.transform.parent = transform;
    }

    private void Update()
    {
        if (isMine)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            // UNITY STANDALONE INPUTs.
            _velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
#elif UNITY_ANDROID
            // UNITY ANDROID INPUTs.
            _velocity = new Vector3(SDUIManager.I.MovementJoyStick.Horizontal, 0, SDUIManager.I.MovementJoyStick.Vertical);
#endif

            if (_velocity.sqrMagnitude > 0)
                _animator.SetBool("isRunning", true);
            else
                _animator.SetBool("isRunning", false);

            if (Input.GetKeyDown(KeyCode.Space))
                _animator.SetTrigger("onJump");
        }
    }

    private void FixedUpdate()
    {
        if(isMine)
            transform.Translate(_velocity * _moveSpeed * Time.fixedDeltaTime);
    }

    public override Packet OnSerializeView(Packet packet)
    {
        if (isMine)
        {
            packet = new Packet(false, new Header(PacketType.TRANSLATE, base.NetworkID));
            packet.Push(new CustomVector3(transform.position), System.Runtime.InteropServices.Marshal.SizeOf<CustomVector3>());
            packet.Push(new CustomVector3(transform.eulerAngles), System.Runtime.InteropServices.Marshal.SizeOf<CustomVector3>());
        }
        return packet;
    }

    public override void OnDeserializeView(Packet packet)
    {
        var pos = packet.Pop<CustomVector3>();
        transform.position = new Vector3(pos.x, pos.y, pos.z);
        var rot = packet.Pop<CustomVector3>();
        transform.eulerAngles = new Vector3(rot.x, rot.y, rot.z);
    }
}
