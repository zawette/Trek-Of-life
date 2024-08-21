using Godot;
using Godot.Collections;
using System;

namespace Enemies.Tfa7a.States;

public partial class IdleState : BaseTfa7aState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		Tfa7aV.Tfa7aAnimatedSprite2d.Play("Idle");
		Tfa7aV.Velocity = Tfa7aV.Velocity with { Y = 0, X = 0 };
		Tfa7aV.StateTimer.Start();
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if(Tfa7aV.StateTimer.TimeLeft <= 0){
			EmitSwitchState("MovingState");
		}

		Tfa7aV.MoveAndSlide();
	}
}
