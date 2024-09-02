using Godot;
using Godot.Collections;


namespace Player.States;

//TODO: Player should be able to dash only once mid air
public partial class DashState : BasePlayerState
{

	private SceneTreeTimer dashTimer;
	private Vector2 initialVelocity;

	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.DisableGravity();
		PlayerV.LegsAnimation.Play("Dash");
		initialVelocity = PlayerV.Velocity;
		PlayerV.Velocity = new(){ X = PlayerV.MovementData.DashPower * PlayerV.Direction.X, Y = 0 };
		dashTimer = GetTree().CreateTimer(PlayerV.MovementData.DashDuration);
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if (dashTimer != null && dashTimer.TimeLeft <= 0)
		{
			PlayerV.Velocity = new(){ X = initialVelocity.X, Y = 0 };
			if (PlayerV.IsOnFloor()) EmitSwitchState("RunState");
			else EmitSwitchState("FallState");
			return;
		}

		PlayerV.MoveAndSlide();
	}

	public override void OnExit(){
		base.OnExit();
		PlayerV.EnableGravity();
	}

}
