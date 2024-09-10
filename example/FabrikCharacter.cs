using FabrikLib;
using Godot;
using System;
using System.Collections.Generic;

public partial class FabrikCharacter : CharacterBody2D
{
	public const float Speed = 100f;
	public const float JumpVelocity = -400.0f;

	public Sprite2D[] sprites = new Sprite2D[3];
	public float[] spriteOriginalRotation;
	private Fabrik2D fabrik;

	private RayCast2D footingSurfaceCast;
	private Vector2 previousGlobalFootPosition;

	public override void _Ready() {
		base._Ready();
		sprites[0] = GetNode<Sprite2D>("p0");
		sprites[1] = GetNode<Sprite2D>("p1");
		sprites[2] = GetNode<Sprite2D>("p2");

		var chain = new Vector2[sprites.Length];
		var maxLimits = new Vector2[sprites.Length];
		spriteOriginalRotation = new float[sprites.Length];
		for (int i = 0; i < chain.Length; i++)
		{
			chain[i] = sprites[i].Position;
			maxLimits[i] = new Vector2(-Mathf.Pi, Mathf.Pi);
			spriteOriginalRotation[i] = sprites[i].Rotation;
			GD.Print(spriteOriginalRotation[i]);
		}

		fabrik = new Fabrik2D(chain);
		fabrik.chainJointAngleMinMax = maxLimits;

		footingSurfaceCast = GetNode<RayCast2D>("footingSurfaceCast");
	}

	public override void _PhysicsProcess(double delta) {
		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
            for (var i = 0;  i < fabrik.chainDirections.Length; ++i)
            {
				Rotation = Position.AngleTo(GetGlobalMousePosition());
                GD.Print(fabrik.chainDirections[i]);
            }
        }

		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = direction * Speed;

		MoveAndSlide();
        var target = footingSurfaceCast.Position + footingSurfaceCast.TargetPosition;
        if (footingSurfaceCast.IsColliding()) {
            var collisionTarget = footingSurfaceCast.GetCollisionPoint() - Position;

			if (collisionTarget.DistanceSquaredTo(sprites[sprites.Length - 1].Position) > 100) {
				target = collisionTarget;
				previousGlobalFootPosition = footingSurfaceCast.GetCollisionPoint();
			}
			else
				target = previousGlobalFootPosition - Position;
        }
        animateLimbs(target);
    }

    private void animateLimbs(Vector2 target) {
		var computedDisplacements = fabrik.solve(target);

		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].Position = computedDisplacements[i];
			sprites[i].Rotation = fabrik.chainDirections[i].Angle() - spriteOriginalRotation[i] + Mathf.Pi;
		}
	}
}
