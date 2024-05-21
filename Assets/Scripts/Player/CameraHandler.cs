using PLAYER;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public PlayerInputs input;

    [Header("Camera")]
    public GameObject _mainCamera;
    public float _targetRotation;
    public float _rotationVelocity;
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = .12f;
    // cinemachine
    public float _cinemachineTargetYaw;
    public float _cinemachineTargetPitch;
    public const float _threshold = 0.01f;
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    public float _sensitivity = 60.0f;

    private void Awake()
    {
        if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    #region Handle Camera
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += input.look.x * _sensitivity * Time.deltaTime;
            _cinemachineTargetPitch += input.look.y * _sensitivity * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    #endregion
}
