using Godot;
using Godot.Collections;
using System;

namespace Player.States;

public partial class IdleState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.PlayIdleAnimation();
		PlayerV.Velocity = PlayerV.Velocity with { Y = 0, X = 0 };
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if (PlayerV.IsMovementDelayed) return;

		if (PlayerV.InputDir.X != 0 || PlayerV.IsAutoRunning)
		{
			EmitSwitchState("RunState");
		}

		if (PlayerV.CanDash && Input.IsActionJustPressed("dash"))
		{
			EmitSwitchState("DashState");
		}

		if ((PlayerV.IsOnFloor() || PlayerV.CoyoteJumpTimer.TimeLeft > 0) && Input.IsActionJustPressed("jump"))
		{
			EmitSwitchState("JumpState");
		}
		else if (!PlayerV.IsOnFloor())
		{
			EmitSwitchState("FallState");
		}

		HandleShooting();
		
		PlayerV.MoveAndSlide();
	}


	private void HandleShooting()
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			PlayerV.PlayGroundShootAnimation();
		}
	}
}
