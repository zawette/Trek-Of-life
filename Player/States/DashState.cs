using Godot;
using Godot.Collections;


namespace Player.States;

//Todo: Add an effect to let the player know that they can dash again once dashDelay is over
public partial class DashState : BasePlayerState
{

	private SceneTreeTimer dashTimer;
	private Vector2 initialVelocity;

	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.DisableGravity();
		PlayerV.PlayDashAnimation();
		initialVelocity = PlayerV.Velocity;
		PlayerV.Velocity = new() { X = PlayerV.MovementData.DashPower * PlayerV.Direction.X, Y = 0 };
		PlayerV.StartGhosting(PlayerV.MovementData.DashDuration * PlayerV.MovementData.GhostingDurationMultiplier);
		dashTimer = GetTree().CreateTimer(PlayerV.MovementData.DashDuration);
	}

	public override void OnPhysicsUpdate(double delta)
	{
		base.OnPhysicsUpdate(delta);

		if ((PlayerV.IsOnFloor() || PlayerV.CoyoteJumpTimer.TimeLeft > 0) && Input.IsActionJustPressed("jump"))
		{
			EmitSwitchState("JumpState");
		}

		if (dashTimer != null && dashTimer.TimeLeft <= 0)
		{
			PlayerV.Velocity = new() { X = initialVelocity.X, Y = 0 };
			if (PlayerV.IsOnFloor()) EmitSwitchState("RunState");
			else EmitSwitchState("FallState");
		}

		PlayerV.MoveAndSlide();
	}

	public override void OnExit()
	{
		base.OnExit();
		PlayerV.EnableGravity();
		PlayerV.DisableDash();
		GetTree()
		.CreateTimer(PlayerV.MovementData.DashDelay)
		.Connect("timeout", Callable.From(() => PlayerV.EnableDash()));

	}

}
