using Godot;
using System;
using System.Diagnostics;

namespace ProjectA
{
    public partial class QuitGame : Node
    {
        /// <summary>
        /// Quits game if escape key is pressed.
        /// </summary>
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey { Keycode: Key.Escape })
            {
                GetTree().Quit();
            }
        }
    }
}
