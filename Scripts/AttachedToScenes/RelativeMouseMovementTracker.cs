using Godot;
using System;

namespace ProjectA;

public partial class RelativeMouseMovementTracker : Node
{
    [Signal] public delegate void OnMouseMovedEventHandler(Vector2 relativeMouseMovement);
    // public Vector2 RelativeMouseMovement { get; private set; }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventArguments)
        {
            // RelativeMouseMovement = eventArguments.Relative;
            EmitSignal(SignalName.OnMouseMoved, eventArguments.Relative);
        }
    }
}
