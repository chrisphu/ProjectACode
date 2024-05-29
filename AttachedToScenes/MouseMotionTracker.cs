using Godot;
namespace ProjectA;

public partial class MouseMotionTracker : Node
{
    [Signal] public delegate void OnMouseMotionEventHandler(Vector2 relativeMouseMovement);
    
    /// <summary>
    /// Emits relative mouse movement during mouse inputs.
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventArguments)
        {
            EmitSignal(SignalName.OnMouseMotion, eventArguments.Relative);
        }
    }
}