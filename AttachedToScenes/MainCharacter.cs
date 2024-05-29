using System.Linq;
using Godot;
namespace ProjectA;

public partial class MainCharacter : CharacterBody3D
{
    [ExportGroup("Nodes")]
    [Export] private MeshInstance3D _debugIndicator;
    
    [ExportGroup("Behavior")]
    [Export] private float _maxMovementSpeed = 1000.0f;
    [Export(PropertyHint.Range, "0.0f, 1.0f")] private float _movementSmoothing = 0.995f;
    [Export] private float _rotationSpeed = 0.5f;
    
    private Vector2 _relativeMouseMotion;
    private float _forwardBackwardMovementSpeed;
    private float _leftRightMovementSpeed;
    
    /// <summary>
    /// Sets mouse mode to being captured and initializes relative mouse movement tracker when scene starts.
    /// </summary>
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        FindMouseMotionTracker();
    }
    
    /// <summary>
    /// Searches the scene for the MouseMotionTracker node and then has OnMouseMoved listen to its
    /// OnMouseMoved signal.
    /// </summary>
    private void FindMouseMotionTracker()
    {
        var mouseMotionTrackerGroup = GetTree().GetNodesInGroup("MouseMotionTracker");
        var mouseMotionTracker = mouseMotionTrackerGroup.OfType<MouseMotionTracker>().FirstOrDefault();
        
        if (mouseMotionTracker is null)
        {
            return;
        }
        
        mouseMotionTracker.OnMouseMotion += UpdateRelativeMouseMotion;
    }
    
    /// <summary>
    /// Listens to relative mouse movement tracker in scene for relative mouse movement during mouse inputs.
    /// </summary>
    private void UpdateRelativeMouseMotion(Vector2 relativeMouseMovement)
    {
        _relativeMouseMotion = relativeMouseMovement;
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
        Rotate(Transform.Basis.Y, -_relativeMouseMotion.X * _rotationSpeed * (float)delta);
        
        // Needs to be reset as _Input otherwise will never set it to (0.0f, 0.0f).
        _relativeMouseMotion = new Vector2();
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
        
        if (debugMaterial?.Duplicate() is not StandardMaterial3D newDebugMaterial)
        {
            return;
        }
        
        newDebugMaterial.AlbedoColor = color; 
        _debugIndicator.SetSurfaceOverrideMaterial(0, newDebugMaterial);
    }
}