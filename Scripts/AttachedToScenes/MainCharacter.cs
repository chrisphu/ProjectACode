using Godot;
using System;

namespace ProjectA;

public partial class MainCharacter : CharacterBody3D
{
    [ExportGroup("Nodes")]
    [Export] private MeshInstance3D _debugIndicator;
    [Export] private RelativeMouseMovementTracker _relativeMouseMovementTracker;
    
    [ExportGroup("Behavior")]
    [Export] private float _maxMovementSpeed = 1000.0f;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _movementSmoothing = 0.995f;
    [Export] private float _rotationSpeed = 0.5f;
    
    private Vector2 _relativeMouseMovement;
    private float _forwardBackwardMovementSpeed;
    private float _leftRightMovementSpeed;
    
    /// <summary>
    /// Sets mouse mode to being captured when scene starts.
    /// </summary>
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _relativeMouseMovementTracker.OnMouseMoved += UpdateRelativeMouseMovement;
    }

    /// <summary>
    /// Called when any input is detected and stores relative mouse movement from event arguments.
    /// </summary>
    // public override void _Input(InputEvent @event)
    // {
    //     if (@event is InputEventMouseMotion eventArguments)
    //     {
    //         _relativeMouseMovement = eventArguments.Relative;
    //     }
    // }

    private void UpdateRelativeMouseMovement(Vector2 relativeMouseMovement)
    {
        _relativeMouseMovement = relativeMouseMovement;
    }
    
    /// <summary>
    /// Calls movement and rotation methods every frame.
    /// </summary>
    public override void _Process(double delta)
    {
        RotateMainCharacter(delta);
        MoveMainCharacter(delta);
    }
    
    /// <summary>
    /// Rotates main character based on last stored relative mouse moment and rotation speed.
    /// </summary>
    private void RotateMainCharacter(double delta)
    {
        Rotate(Transform.Basis.Y, -_relativeMouseMovement.X * _rotationSpeed * (float)delta);
        
        // Needs to be reset as _Input otherwise will never set it to (0.0f, 0.0f).
        _relativeMouseMovement = new Vector2();
    }
    
    /// <summary>
    /// Moves main character based on input map.
    /// </summary>
    private void MoveMainCharacter(double delta)
    {
        var forwardBackwardMovement = Input.GetAxis("MoveBackward", "MoveForward");
        var leftRightMovement = Input.GetAxis("MoveRight", "MoveLeft");
        
        _forwardBackwardMovementSpeed = Mathf.Lerp(
            _forwardBackwardMovementSpeed,
            (forwardBackwardMovement == 0.0f ? 0.0f : forwardBackwardMovement * _maxMovementSpeed),
            FloatExtensionMethods.DampFactorForLerp(_movementSmoothing, delta));
        
        _leftRightMovementSpeed = Mathf.Lerp(
            _leftRightMovementSpeed,
            (leftRightMovement == 0.0f ? 0.0f : leftRightMovement * _maxMovementSpeed),
            FloatExtensionMethods.DampFactorForLerp(_movementSmoothing, delta));
        
        var combinedMovement = Transform.Basis.Z * _forwardBackwardMovementSpeed
                               + Transform.Basis.X * _leftRightMovementSpeed;
        
        // Needs to be normalized to avoid diagonal movement being faster than single-axis movement.
        if (combinedMovement.Length() > _maxMovementSpeed)
        {
            combinedMovement = combinedMovement.Normalized() * _maxMovementSpeed;
        }
        
        Velocity = combinedMovement * (float)delta;
        MoveAndSlide();
    }
    
    /// <summary>
    /// Sets the albedo color of the debug indicator's material.
    /// </summary>
    private void SetDebugIndicatorColor(Color color)
    {
        var debugMaterial = _debugIndicator.GetActiveMaterial(0);
        
        if (debugMaterial == null)
        {
            return;
        }
        
        var newDebugMaterial = debugMaterial.Duplicate() as StandardMaterial3D; 
        newDebugMaterial.AlbedoColor = color; 
        _debugIndicator.SetSurfaceOverrideMaterial(0, newDebugMaterial);
    }
}
