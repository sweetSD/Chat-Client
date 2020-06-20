using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _sensivity = 10.0f;
    [SerializeField] private float _offset = 5.0f;
    [SerializeField] private Transform _cameraContainer;
    [SerializeField] private Transform _cameraTransform;

    private Vector2 _mouseInput;

    private void Awake()
    {
        if (_cameraTransform == null)
            _cameraTransform = FindObjectOfType<Camera>().transform;
        _cameraTransform.localPosition = new Vector3(0, 0, -_offset);
    }

    private void Update()
    {
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void FixedUpdate()
    {
        // 카메라 회전 로직
        transform.Rotate(Vector3.up, _mouseInput.x * _sensivity);
        _cameraContainer.Rotate(Vector3.right, _mouseInput.y * _sensivity);
        _cameraContainer.eulerAngles = new Vector3(Mathf.Clamp(_cameraContainer.eulerAngles.x <= 180 ? _cameraContainer.eulerAngles.x : _cameraContainer.eulerAngles.x - 360, -30, 50), _cameraContainer.eulerAngles.y, 0);

        // 카메라 충돌체 충돌 로직
        var directionFromContainer = _cameraContainer.rotation * Vector3.back;
        Ray ray = new Ray(_cameraContainer.position, directionFromContainer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _offset, 1 << LayerMask.NameToLayer("Environments")))
            _cameraTransform.localPosition = new Vector3(0, 0, -hit.distance + 0.5f);
        else
            _cameraTransform.localPosition = new Vector3(0, 0, -_offset);
    }
}
