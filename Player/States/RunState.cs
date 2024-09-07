using Godot;
using Godot.Collections;
using System;

namespace Player.States;

public partial class RunState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.PlayRunAnimation();
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if (PlayerV.InputDir.X != 0)
		{
			PlayerV.FlipSprite(PlayerV.InputDir);
		}



		if (PlayerV.Velocity.X == 0)
		{
			EmitSwitchState("IdleState");
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

		HandleXMovements(delta);
		ApplyFriction(delta);
		var wasOnfloor = PlayerV.IsOnFloor();
		PlayerV.MoveAndSlide();
		HandleCoyoteTimer(wasOnfloor);

	}


	private void HandleXMovements(double delta)
	{
		var direction = PlayerV.IsAutoRunning ? PlayerV.Direction.X : PlayerV.InputDir.X;

		PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, PlayerV.MovementData.Speed * direction, PlayerV.MovementData.Acceleration * delta) };
	}


	private void HandleCoyoteTimer(bool wasPlayerOnTheFloor)
	{
		if (wasPlayerOnTheFloor && PlayerV.IsPlayerFalling())
		{
			PlayerV.CoyoteJumpTimer.Start();
		}
	}

	private void ApplyFriction(double delta)
	{

		if (PlayerV.InputDir.X == 0 && PlayerV.IsOnFloor())
		{
			PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, 0, PlayerV.MovementData.Friction * delta) };
		}
	}

	private void HandleShooting()
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			PlayerV.PlayGroundShootAnimation();
		}
	}
}
