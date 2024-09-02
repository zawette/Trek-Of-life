using Godot;
using Godot.Collections;
using System;

namespace Player.States;

public partial class JumpState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.LegsAnimation.Play("Jump");

		if (message.ContainsKey(playerMsgKeys.wallJump.ToString())) //MAybe move to a separate walljump state
		{
			PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.WallJumpYPower, X = PlayerV.MovementData.WallJumpXPower * PlayerV.GetWallNormal().X };
			return;
		}

		PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.JumpVelocity };
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if(Input.IsActionJustPressed("dash")){
			EmitSwitchState("DashState");
			return;
		}

		if (PlayerV.IsOnFloor())
		{
			if (PlayerV.InputDir.X != 0 || PlayerV.IsAutoRunning) EmitSwitchState("RunState");
			else EmitSwitchState("IdleState");
		}

		if(PlayerV.Velocity.Y >= 0) EmitSwitchState("FallState");

		HandleXAirMovements(delta);
		ApplyAirResistance(delta);
		ApplyLowJump(delta);
		HandleWallHang();
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

		if (PlayerV.InputDir.X == 0)
		{
			PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, 0, PlayerV.MovementData.AirResistance * delta) };
		}
	}

	private void ApplyLowJump(double delta)
	{
		if (Input.IsActionJustReleased("jump") && PlayerV.IsPlayerJumping())
		{
			PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.Velocity.Y + PlayerV.MovementData.LowJumpMultiplier * PlayerV.Gravity };
		}
	}

	private void HandleWallHang()
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
