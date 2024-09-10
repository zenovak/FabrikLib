using Godot;
using System;

public partial class ArmController : RigidBody2D {
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public override void _PhysicsProcess(double delta) {

		/*var targetDirection = (GetGlobalMousePosition() - GlobalPosition).Angle();
		GD.Print(targetDirection);
		Rotation += Mathf.Lerp(0f, targetDirection - GlobalRotation, (float)delta);*/

		var targetDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		var currentDirection = Vector2.FromAngle(GlobalRotation);

		Rotation += Mathf.Lerp(0f, currentDirection.AngleTo(targetDirection), (float)delta);
	}
}
