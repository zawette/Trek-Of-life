using Godot;
using Godot.Collections;
using System;

namespace Enemies.Tfa7a.States;

public partial class MovingState : BaseTfa7aState
{
	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);

		Tfa7aV.Tfa7aAnimatedSprite2d.Play("Moving");
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		var currentTargetPosition = Tfa7aV.patrolPointsList[Tfa7aV.currentPointId].GlobalPosition;
		var direction = Tfa7aV.GlobalPosition.DirectionTo(currentTargetPosition);

		if(direction.X < 0) Tfa7aV.Tfa7aAnimatedSprite2d.FlipH = true;
		else Tfa7aV.Tfa7aAnimatedSprite2d.FlipH = false;

		Tfa7aV.Velocity = Tfa7aV.Velocity with { X = (float)(direction.X * Tfa7aV.Speed * delta) };

		if(Math.Abs(Tfa7aV.GlobalPosition.X - currentTargetPosition.X) <= 1){
			Tfa7aV.currentPointId = (Tfa7aV.currentPointId + 1) % Tfa7aV.patrolPointsList.Count;
			EmitSwitchState("IdleState");
		}
		Tfa7aV.MoveAndSlide();
	}
}
