using Godot;
namespace ProjectA;

/// <summary>
/// Camera travels on a halo track. The halo track follows the tracked object and the camera "slides" along this track.
/// </summary>
public partial class Camera : Camera3D
{
    [ExportGroup("Nodes")]
    [Export] private Node3D _trackedObject;
    
    [ExportGroup("Halo track")]
    [Export] private float _haloTrackRadius = 6.0f;
    [Export] private float _haloTrackYOffsetFromTrackedObject = 4.0f;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _haloTrackXZPositionSmoothness = 0.995f;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _haloTrackYPositionSmoothness = 0.9f;
    
    [ExportGroup("Behavior")]
    [Export] private Vector3 _cameraOffsetFromTrackedObject;
    [Export] private Vector3 _lookingAtOffset;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _cameraRotationSmoothness = 0.995f;
    // [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _verticalTrackingSmoothness = 0.9f;
    
    private Vector3 _haloPosition;
    private float _cameraRotation;
    private float _trackedObjectVerticalPosition;
    
    public override void _Ready()
    {
        if (_trackedObject == null)
        {
            return;
        }
        
        _haloPosition = _trackedObject.Position + _trackedObject.Basis.Y * _haloTrackYOffsetFromTrackedObject;
        _trackedObjectVerticalPosition = _trackedObject.Position.Y;
    }
    
    /// <summary>
    /// Calls various camera lerping methods every frame.
    /// </summary>
    public override void _Process(double delta)
    {
        if (_trackedObject == null)
        {
            return;
        }
        
        LerpHaloTrack(delta);
        LerpCameraAlongHaloTrack(delta);
        UpdateCameraLookingAt(delta);
    }
    
    /// <summary>
    /// Lerps halo track over top of tracked object.
    /// </summary>
    private void LerpHaloTrack(double delta)
    {
        var desiredPosition = _trackedObject.Position 
                              + Vector3.Up * _haloTrackYOffsetFromTrackedObject;
        
        _haloPosition = _haloPosition.Lerp(
            desiredPosition,
            FloatExtensionMethods.DampFactorForLerp(_haloTrackXZPositionSmoothness, delta));
        
        _haloPosition = new Vector3()
        {
            X = Mathf.Lerp(
                _haloPosition.X,
                desiredPosition.X,
                FloatExtensionMethods.DampFactorForLerp(_haloTrackXZPositionSmoothness, delta)),
            Y = Mathf.Lerp(
                _haloPosition.Y,
                desiredPosition.Y,
                FloatExtensionMethods.DampFactorForLerp(_haloTrackYPositionSmoothness, delta)),
            Z = Mathf.Lerp(
                _haloPosition.Z,
                desiredPosition.Z,
                FloatExtensionMethods.DampFactorForLerp(_haloTrackXZPositionSmoothness, delta))
        };
    }
    
    /// <summary>
    /// Lerps camera to "slide" along halo track to match rotation of tracked object.
    /// </summary>
    private void LerpCameraAlongHaloTrack(double delta)
    {
        _cameraRotation = Mathf.LerpAngle(
            _cameraRotation,
            _trackedObject.Rotation.Y,
            FloatExtensionMethods.DampFactorForLerp(_cameraRotationSmoothness, delta));
        
        Position = _haloPosition + (Vector3.Forward * Mathf.Cos(_cameraRotation)
                                 + Vector3.Left * Mathf.Sin(_cameraRotation)) * _haloTrackRadius;
    }
    
    /// <summary>
    /// Set camera's transform (essentially rotating) to look at tracked object.
    /// </summary>
    // TODO: Add some play where mouse movement doesn't rotate camera, instead shifts looking at offset to other side
    private void UpdateCameraLookingAt(double delta)
    {
        // _trackedObjectVerticalPosition = _trackedObjectVerticalPosition.Lerp(
        //     _trackedObject.Position.Y,
        //     FloatExtensionMethods.DampFactorForLerp(_verticalTrackingSmoothness, delta));
        
        Transform = Transform.LookingAt( new Vector3(
            _trackedObject.Position.X,
            _trackedObjectVerticalPosition,
            _trackedObject.Position.Z) + _trackedObject.Basis * _lookingAtOffset);
    }
}