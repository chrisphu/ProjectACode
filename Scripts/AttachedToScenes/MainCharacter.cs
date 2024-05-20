using Godot;
using System;

namespace ProjectA
{
	public partial class MainCharacter : CharacterBody3D
	{
		[Export] private float _movementSpeed = 600.0f;
		[Export] private float _rotationSpeed = 0.5f;

		private Vector2 _relativeMouseMovement;

		/// <summary>
		/// Sets mouse mode to being captured when scene starts.
		/// </summary>
		public override void _Ready()
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		
		/// <summary>
		/// Called when any input is detected and stores relative mouse movement from event arguments.
		/// </summary>
		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseMotion eventArguments)
			{
				_relativeMouseMovement = eventArguments.Relative;
			}
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
			// Needs to be normalized to avoid diagonal movements being faster than single-axis movements.
			Vector3 combinedMovementAxes = (Transform.Basis.Z * Input.GetAxis("MoveBackward", "MoveForward")
			                                + Transform.Basis.X * Input.GetAxis("MoveRight", "MoveLeft")).Normalized();
		
			Velocity = combinedMovementAxes * _movementSpeed * (float)delta;
			MoveAndSlide();
		}
	}
}
