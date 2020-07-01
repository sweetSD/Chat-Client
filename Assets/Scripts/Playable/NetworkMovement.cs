using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetworkMovement : NetworkView
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [Header("Navigation")]
    [SerializeField] private NavMeshAgent _navAgent;

    private Vector3 _velocity = Vector3.zero;
    private bool _isJumped = false;

    private readonly string _runningAnimBoolName = "isRunning";
    private readonly string _jumpAnimTriggerName = "onJump";


    [Header("Network")]
    [SerializeField] private float _networkLerpValue = 5f;
    private Vector3 _realPosition = Vector3.zero;
    private Quaternion _realRotation = Quaternion.identity;
    private Vector3 _realScale = Vector3.zero;

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
                _animator.SetBool(_runningAnimBoolName, true);
            else
                _animator.SetBool(_runningAnimBoolName, false);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _animator.SetTrigger(_jumpAnimTriggerName);
                _isJumped = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMine)
        {
            transform.Translate(_velocity * _moveSpeed * Time.fixedDeltaTime);
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10, NavMesh.AllAreas))
                _navAgent.Warp(hit.position);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _realPosition, _networkLerpValue * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _realRotation, _networkLerpValue * Time.fixedDeltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, _realScale, _networkLerpValue * Time.fixedDeltaTime);
        }
    }

    public override Packet OnSerializeView(Packet packet)
    {
        if (isMine)
        {
            packet = new Packet(false, new Header(_type: PacketType.TRANSLATE));
            packet.Push(new CustomVector3(transform.position), System.Runtime.InteropServices.Marshal.SizeOf<CustomVector3>());
            packet.Push(new CustomVector3(transform.eulerAngles), System.Runtime.InteropServices.Marshal.SizeOf<CustomVector3>());
            packet.Push(new CustomVector3(transform.localScale), System.Runtime.InteropServices.Marshal.SizeOf<CustomVector3>());
            packet.Push(_animator.GetBool(_runningAnimBoolName), System.Runtime.InteropServices.Marshal.SizeOf<bool>());
            packet.Push(_isJumped, System.Runtime.InteropServices.Marshal.SizeOf<bool>());
            _isJumped = false;
        }
        return packet;
    }

    public override void OnDeserializeView(Packet packet)
    {
        var pos = packet.Pop<CustomVector3>();
        _realPosition = new Vector3(pos.x, pos.y, pos.z);
        var rot = packet.Pop<CustomVector3>();
        _realRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        var scl = packet.Pop<CustomVector3>();
        _realScale = new Vector3(scl.x, scl.y, scl.z);
        var isRunning = packet.Pop<bool>();
        _animator.SetBool(_runningAnimBoolName, isRunning);
        var isJumped = packet.Pop<bool>();
        if (isJumped) { _animator.SetTrigger(_jumpAnimTriggerName); }
    }
}
