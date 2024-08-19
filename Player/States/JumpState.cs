using Godot;
using Godot.Collections;
using System;

public partial class JumpState : BasePlayerState
{
	private bool _isStuckOnWall = false;

	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		if (message.ContainsKey(playerMsgKeys.wallJump.ToString())) { 
			PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.WallJumpYPower, X = PlayerV.MovementData.WallJumpXPower * PlayerV.GetWallNormal().X };
			return;
		}
		if (message.ContainsKey(playerMsgKeys.freeFall.ToString()))
			return;

		PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.JumpVelocity };
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);
		if (PlayerV.IsOnFloor())
		{
			EmitSwitchState("IdleState");
		}

		HandleXAirMovements(delta);
		ApplyAirResistance(delta);
		ApplyLowJump(delta);
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

	private void ApplyLowJump(double delta)
	{
		if (Input.IsActionJustReleased("jump") && PlayerV.IsPlayerJumping())
		{
			PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.Velocity.Y + PlayerV.MovementData.LowJumpMultiplier * PlayerV.Gravity };
		}
	}

	// move to wall hang state
	private void HandleWallJump()
	{
		if (PlayerV.IsOnWallOnly())
		{
			if (Input.IsActionPressed("hang"))
			{
				EmitSwitchState("WallHangState");
			}
			//else if (_isStuckOnWall) // isOnWall but hang not pressed => wallSlide
			//{
			//    PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.WallSlideAcceleration };
			//}

			//if (Input.IsActionJustPressed("jump") && _isStuckOnWall)
			//{
			//    _isStuckOnWall = false;
			//    var wallJumpYPower = -80;
			//    var wallJumpXPower = 80;
			//    PlayerV.Velocity = PlayerV.Velocity with { Y = wallJumpYPower };
			//    PlayerV.Velocity = PlayerV.Velocity with { X = PlayerV.GetWallNormal().X * wallJumpXPower };
			//}
		}
	}

}
