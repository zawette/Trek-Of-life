using Godot;
using Godot.Collections;
using System;

public partial class RunState : BasePlayerState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.LegsAnimation.Play("RunForward");
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);
		PlayerV.LegsSprite.FlipH = PlayerV.InputDir.X < 0;

		if (PlayerV.Velocity.X == 0)
		{
			EmitSwitchState("IdleState");
		}

		if ((PlayerV.IsOnFloor() || PlayerV.CoyoteJumpTimer.TimeLeft > 0) && Input.IsActionJustPressed("jump"))
		{
			EmitSwitchState("JumpState");
		}

		if (!PlayerV.IsOnFloor())
		{
			var dic = new Godot.Collections.Dictionary<string, Variant>() { { playerMsgKeys.freeFall.ToString(), true } };
			EmitSwitchState("JumpState", dic); //create new fall state
		}

		HandleXMovements(delta);
		ApplyFriction(delta);
		var wasOnfloor = PlayerV.IsOnFloor();
		PlayerV.MoveAndSlide();
		HandleCoyoteTimer(wasOnfloor);

	}


	private void HandleXMovements(double delta)
	{
		PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, PlayerV.MovementData.Speed * PlayerV.InputDir.X, PlayerV.MovementData.Acceleration * delta) };
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
}
