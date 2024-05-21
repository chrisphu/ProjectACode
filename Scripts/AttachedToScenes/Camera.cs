using Godot;
using System;

namespace ProjectA;

public partial class Camera : Camera3D
{
    [Export] private Node3D _trackedObject;
    [Export] private Vector3 _cameraPositionOffset;
    [Export] private Vector3 _lookingAtOffset;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _cameraPositionSmoothness = 0.995f;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _verticalTrackingSmoothness = 0.9f;

    private float _trackedObjectVerticalPosition;
    
    public override void _Ready()
    {
        if (_trackedObject == null)
        {
            return;
        }

        _trackedObjectVerticalPosition = _trackedObject.Position.Y;
    }
    
    /// <summary>
    /// Calls camera lerping method every frame.
    /// </summary>
    public override void _Process(double delta)
    {
        if (_trackedObject == null)
        {
            return;
        }
        
        LerpCameraToDesiredPosition(delta);
        UpdateCameraLookingAt(delta);
    }

    /// <summary>
    /// Lerps camera to desired position with damped factor independent of frame rate.
    /// </summary>
    private void LerpCameraToDesiredPosition(double delta)
    {
        Vector3 desiredPosition = _trackedObject.Position + _trackedObject.Basis * _cameraPositionOffset;

        Position = Position.Lerp(
            desiredPosition,
            FloatExtensionMethods.DampFactorForLerp(_cameraPositionSmoothness, delta));
    }

    private void UpdateCameraLookingAt(double delta)
    {
        _trackedObjectVerticalPosition = _trackedObjectVerticalPosition.Lerp(
            _trackedObject.Position.Y,
            FloatExtensionMethods.DampFactorForLerp(_verticalTrackingSmoothness, delta));
        
        Transform = Transform.LookingAt( new Vector3(
            _trackedObject.Position.X,
            _trackedObjectVerticalPosition,
            _trackedObject.Position.Z) + _trackedObject.Basis * _lookingAtOffset);
    }
}