using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDUIManager : SDSingleton<SDUIManager>
{

    [SerializeField] private JoyStick _movementJoyStick;
    public JoyStick MovementJoyStick => _movementJoyStick;

    [SerializeField] private DragDetector _cameraControlDetector;
    public DragDetector CameraControlDetector => _cameraControlDetector;

    private void Awake()
    {
        SetInstance(this);
    }
}
