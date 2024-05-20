using Godot;
using System;

namespace ProjectA
{
	public partial class Camera : Camera3D
	{
		[Export] private Node3D _objectToFollow;
		[Export] private Vector3 _positionOffset;
		[Export(PropertyHint.Range, "0.0f,1.0f")] private float _positionSmoothness = 0.995f;
	
		/// <summary>
		/// Calls camera lerping method every frame.
		/// </summary>
		public override void _Process(double delta)
		{
			LerpCameraToDesiredPosition(delta);
		}

		/// <summary>
		/// Lerps camera to desired position with damped factor independent of frame rate.
		/// </summary>
		private void LerpCameraToDesiredPosition(double delta)
		{
			if (_objectToFollow == null)
			{
				return;
			}

			Vector3 desiredPosition = _objectToFollow.Position + _positionOffset;

			Position = Position.Lerp(
				desiredPosition,
				DampExtensionFunctions.DampFactorForLerp(_positionSmoothness, delta));
		}
	}
}
