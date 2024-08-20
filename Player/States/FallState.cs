using Godot;
using Godot.Collections;
using System;

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
			if (PlayerV.InputDir.X != 0) EmitSwitchState("RunState");
			else EmitSwitchState("IdleState");
		}

		HandleXAirMovements(delta);
		ApplyAirResistance(delta);
		HandleWallJump();
		PlayerV.MoveAndSlide();
	}

	private void HandleXAirMovements(double delta)
	{
		if (PlayerV.InputDir.X != 0)
		{
			PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, PlayerV.MovementData.Speed * PlayerV.InputDir.X, PlayerV.MovementData.AirAcceleration * delta) };
		}
	}
	private void ApplyAirResistance(double delta)
	{

		if (PlayerV.InputDir.X == 0)
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
