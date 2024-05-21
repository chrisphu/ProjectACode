using Godot;
using System;

namespace ProjectA;

public partial class MainCharacter : CharacterBody3D
{
	[Export] private MeshInstance3D _debugIndicator;
	[Export] private float _maxMovementSpeed = 1.0f;
	[Export(PropertyHint.Range, "0.0f, 1.0f")] private float _movementSmoothing = 0.99f;
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
		float forwardBackwardMovement = Input.GetAxis("MoveBackward", "MoveForward");
		float leftRightMovement = Input.GetAxis("MoveRight", "MoveLeft");

		_forwardBackwardMovementSpeed = _forwardBackwardMovementSpeed.Lerp(
			(forwardBackwardMovement == 0.0f ? 0.0f : forwardBackwardMovement * _maxMovementSpeed),
			FloatExtensionMethods.DampFactorForLerp(_movementSmoothing, delta));
		
		_leftRightMovementSpeed = _leftRightMovementSpeed.Lerp(
			(leftRightMovement == 0.0f ? 0.0f : leftRightMovement * _maxMovementSpeed),
			FloatExtensionMethods.DampFactorForLerp(_movementSmoothing, delta));

		Vector3 combinedMovement = Transform.Basis.Z * _forwardBackwardMovementSpeed
		                           + Transform.Basis.X * _leftRightMovementSpeed;

		// Needs to be normalized to avoid diagonal movement being faster than single-axis movement.
		if (combinedMovement.Length() > _maxMovementSpeed)
		{
			combinedMovement = combinedMovement.Normalized() * _maxMovementSpeed;
		}

		Velocity = combinedMovement * (float)delta;
		MoveAndSlide();
	}

	private void SetDebugIndicatorColor(Color color)
	{
		Material debugMaterial = _debugIndicator.GetActiveMaterial(0);
		StandardMaterial3D newDebugMaterial = debugMaterial.Duplicate() as StandardMaterial3D; 
		newDebugMaterial.AlbedoColor = color; 
		_debugIndicator.SetSurfaceOverrideMaterial(0, newDebugMaterial);
	}
}
