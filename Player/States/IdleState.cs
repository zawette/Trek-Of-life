using Godot;
using Godot.Collections;
using System;

public partial class IdleState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.LegsAnimation.Play("Idle");
		PlayerV.Velocity = PlayerV.Velocity with { Y = 0, X = 0 };
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if (PlayerV.InputDir.X != 0)
		{
			EmitSwitchState("RunState");
		}

		if ((PlayerV.IsOnFloor() || PlayerV.CoyoteJumpTimer.TimeLeft > 0) && Input.IsActionJustPressed("jump"))
		{
			EmitSwitchState("JumpState");
		}

		else if (!PlayerV.IsOnFloor())
		{
			EmitSwitchState("FallState");
		}
		PlayerV.MoveAndSlide();
	}
}
