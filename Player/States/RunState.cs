using Godot;
using Godot.Collections;
using System;

public partial class RunState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);
		if (PlayerV.InputDir.X == 0)
		{
			EmitSwitchState("IdleState");
		}

		if ((PlayerV.IsOnFloor() || PlayerV.CoyoteJumpTimer.TimeLeft > 0) && Input.IsActionJustPressed("jump"))
		{
			EmitSwitchState("JumpState");
		}


		HandleXMovements(delta);
		ApplyFriction(delta);
	}


	private void HandleXMovements(double delta)
	{
		PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, PlayerV.MovementData.Speed * PlayerV.InputDir.X, PlayerV.MovementData.Acceleration * delta) };
	}

	private void ApplyFriction(double delta)
	{

		if (PlayerV.InputDir.X == 0 && PlayerV.IsOnFloor())
		{
			PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, 0, PlayerV.MovementData.Friction * delta) };
		}
	}
}
