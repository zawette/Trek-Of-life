using Godot;
using Godot.Collections;
using System;

namespace Player.States;


public partial class FallState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.LegsAnimation.Play("Fall");
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if (PlayerV.IsOnFloor())
		{
			if (PlayerV.InputDir.X != 0 || PlayerV.IsAutoRunning) EmitSwitchState("RunState");
			else EmitSwitchState("IdleState");
		}

		HandleXAirMovements(delta);
		ApplyAirResistance(delta);
		HandleWallJump();
		PlayerV.MoveAndSlide();
	}

	private void HandleXAirMovements(double delta)
	{
		if (PlayerV.InputDir.X != 0 || PlayerV.IsAutoRunning)
		{
			var direction = PlayerV.IsAutoRunning ? PlayerV.Direction.X : PlayerV.InputDir.X;
			PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, PlayerV.MovementData.Speed * direction, PlayerV.MovementData.AirAcceleration * delta) };
		}
	}
	private void ApplyAirResistance(double delta)
	{

		if (PlayerV.InputDir.X == 0 && !PlayerV.IsAutoRunning)
		{
			PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, 0, PlayerV.MovementData.AirResistance * delta) };
		}
	}


	private void HandleWallJump()
	{
		if (PlayerV.IsOnWallOnly())
		{
			if (Input.IsActionPressed("hang"))
			{
				EmitSwitchState("WallHangState");
			}
		}
	}

}
