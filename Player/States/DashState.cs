using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

namespace Player.States;

public partial class DashState : BasePlayerState
{

	private SceneTreeTimer dashTimer;
	private Vector2 initialVelocity;


	//TODO: add dash while in air and disable Gravity while in air
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.LegsAnimation.Play("Dash");
		initialVelocity = PlayerV.Velocity;
		PlayerV.Velocity = PlayerV.Velocity with { X = PlayerV.MovementData.DashPower * PlayerV.Direction.X };
		dashTimer = GetTree().CreateTimer(PlayerV.MovementData.DashDuration);
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if (dashTimer != null && dashTimer.TimeLeft == 0)
		{
			PlayerV.Velocity = initialVelocity;
			EmitSwitchState("RunState");
			return;
		}

		PlayerV.MoveAndSlide();
	}

}
