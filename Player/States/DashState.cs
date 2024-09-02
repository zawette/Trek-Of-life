using Godot;
using Godot.Collections;


namespace Player.States;

//TODO: add delay btw dashes where you can't dash
public partial class DashState : BasePlayerState
{

	private SceneTreeTimer dashTimer;
	private Vector2 initialVelocity;
	private float ghostTimer = 0f;
	[Export] public float ghostSpawnInterval = 0.07f;

	public override void OnEnter(Dictionary<string, Variant> message = null)
	{
		base.OnEnter(message);
		PlayerV.DisableGravity();
		PlayerV.LegsAnimation.Play("Dash");
		initialVelocity = PlayerV.Velocity;
		PlayerV.Velocity = new() { X = PlayerV.MovementData.DashPower * PlayerV.Direction.X, Y = 0 };
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
			return;
		}

		ghostTimer += (float)delta;
		if (ghostTimer >= ghostSpawnInterval)
		{
			SpawnGhost();
			ghostTimer = 0f;
		}

		PlayerV.MoveAndSlide();
	}

	public override void OnExit()
	{
		base.OnExit();
		PlayerV.EnableGravity();
	}


	private void SpawnGhost()
	{
		var ghost = new AnimatedSprite2D();
		ghost.SpriteFrames = PlayerV.LegsSprite.SpriteFrames;
		ghost.Animation = PlayerV.LegsSprite.Animation;
		ghost.Frame = PlayerV.LegsSprite.Frame;
		ghost.Position = PlayerV.LegsSprite.GlobalPosition;
		ghost.Scale = PlayerV.PlayerSprite.Scale;
		ghost.FlipH = PlayerV.LegsSprite.FlipH;
		ghost.FlipV = PlayerV.LegsSprite.FlipV;
		ghost.Modulate = new Color(1, 1, 1, 0.5f);

		PlayerV.GetParent().AddChild(ghost);

		var tween = ghost.CreateTween();
		tween.TweenProperty(ghost, "modulate:a", 0f, 0.5f);
		tween.TweenCallback(Callable.From(() => ghost.QueueFree()));
		tween.Play();
	}

	

}
