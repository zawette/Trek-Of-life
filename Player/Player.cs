using Godot;
using Player.States;
using Player.weapons;
using System;

namespace Player;
public partial class Player : CharacterBody2D
{
	[Export] public PlayerMovementData MovementData;
	[Export] public bool IsAutoRunning;
	[Export] public Vector2 Direction;
	[Export] public float StartDelayTime = 7.0f;
	[Export] private bool _canDash = true;
	public bool CanDash { get => _canDash; }
	private float ghostTimer = 0f;
	private bool isGhosting = false;
	public bool IsMovementDelayed = true; // Flag to delay movement

	public Vector2 InputDir = Vector2.Zero;
	public Timer CoyoteJumpTimer;

	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public AnimationPlayer BodyMinusRightHandAnimation;

	public AnimationPlayer RightHandAnimation;

	public AnimatedSprite2D LegsSprite;

	public Marker2D ShootingHandMarker;

	public PlayerStateMachine PStateMachine;
	public Node2D PlayerSprite;
	public AnimatedSprite2D RightHandSprite;
	private bool _isGravityDisabled;

	private Timer _startDelayTimer;

	private float shootHoldTime = 0f;
	private bool isShooting = false;
	private Vector2 aimingDirection = Vector2.Right;
	[Export] private float AimingAngleLimit = 30f;
	[Export] private float BurstHoldTime = 0.5f;
	private GpuParticles2D flare;

	public override void _Ready()
	{
		PlayerSprite = GetNode<Node2D>("PlayerSprite");
		RightHandSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Right_hand");
		CoyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		BodyMinusRightHandAnimation = GetNode<AnimationPlayer>("BodyMinusRightHandAnimation");
		RightHandAnimation = GetNode<AnimationPlayer>("RightHandAnimation");
		LegsSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Legs");
		ShootingHandMarker = GetNode<Marker2D>("PlayerSprite/Right_hand/Shooting_hand_marker");
		PStateMachine = GetNode<PlayerStateMachine>("PlayerStateMachine");
		_startDelayTimer = new Timer
		{
			OneShot = true,
			WaitTime = StartDelayTime,
			Autostart = true
		};
		_startDelayTimer.Timeout += OnStartDelayTimeout;
		AddChild(_startDelayTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		InputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		AddGravity(delta);
		AlignCharToSlope();
		HandleGhosting(delta);
		HandleShooting(delta);
	}

	private void HandleGhosting(double delta)
	{
		if (isGhosting)
		{
			ghostTimer += (float)delta;
			if (ghostTimer >= MovementData.GhostSpawnInterval)
			{
				SpawnGhost();
				ghostTimer = 0f;
			}
		}
	}

	private void HandleShooting(double delta)
	{
		if (Input.IsActionPressed("shoot"))
		{
			PlayGroundShootAnimation();
			shootHoldTime += (float)delta;
			isShooting = true;
			HandleAiming();

			if (shootHoldTime >= BurstHoldTime && flare is null)
			{
				flare = GD.Load<PackedScene>("res://Shared/Particles/Flare.tscn").Instantiate<GpuParticles2D>();
				ShootingHandMarker.AddChild(flare);
			}

		}
		else if (isShooting)
		{
			if (shootHoldTime >= BurstHoldTime)
			{
				BurstShot();
			}

			// Rinit
			shootHoldTime = 0f;
			isShooting = false;
			RightHandSprite.Rotation = 0;
			aimingDirection = Vector2.Right;
			RightHandAnimation.Play(PStateMachine.CurrentStateName.Replace("State", String.Empty));

			if (flare is not null)
			{
				flare.QueueFree();
				flare = null;
			}
		}
	}

	private void HandleAiming()
	{

		float verticalInput = Input.GetActionStrength("aim_down") - Input.GetActionStrength("aim_up");
		if (Mathf.Abs(verticalInput) > 0.1f) // Stick input detected
		{
			float targetAngle = verticalInput * AimingAngleLimit; // Convert to angle between -40 to 40 degrees
			aimingDirection = new Vector2(1, Mathf.Tan(Mathf.DegToRad(targetAngle))).Normalized();
		}
		else if (Input.IsMouseButtonPressed(MouseButton.Left)) // Mouse input
		{
			// Get mouse Y-position relative to the screen
			float mouseY = GetViewport().GetMousePosition().Y;
			float screenHeight = GetViewportRect().Size.Y;

			// Map mouse Y-position to an angle between -40 and 40 degrees
			float normalizedMouseY = (mouseY / screenHeight) * 2 - 1; // Normalize between -1 and 1
			float targetAngle = Mathf.Clamp(normalizedMouseY * AimingAngleLimit, -AimingAngleLimit, AimingAngleLimit);

			aimingDirection = new Vector2(1, Mathf.Tan(Mathf.DegToRad(targetAngle))).Normalized();
		}
		RightHandSprite.Rotation = aimingDirection.Angle();
	}

	private void BurstShot() // TODO: use Marker2d to make the bullet spawn in front of the player's hand
	{
		var bullet = GD.Load<PackedScene>("res://Player/weapons/bullet/Bullet.tscn").Instantiate<Bullet>();
		bullet.GlobalPosition = ShootingHandMarker.GlobalPosition;
		bullet.Direction = aimingDirection with { X = Direction.X * aimingDirection.X };
		bullet.Rotation = RightHandSprite.Rotation * Direction.X;
		GetParent().AddChild(bullet);
	}

	private void AddGravity(double delta)
	{
		if (!IsOnFloor() && !_isGravityDisabled)
			Velocity = Velocity with { Y = Velocity.Y + Gravity * MovementData.GravityScale * (float)delta };
	}

	private void AlignCharToSlope()
	{
		if (IsOnFloor())
		{
			var normal = GetFloorNormal();
			PlayerSprite.Rotation = normal.Angle() + Mathf.DegToRad(90);
		}
	}

	private void OnStartDelayTimeout()
	{
		IsMovementDelayed = false;
	}

	public bool IsPlayerJumping() => Velocity.Y < 0;
	public bool IsPlayerFalling() => !IsOnFloor() && Velocity.Y >= 0;
	public void DisableGravity() => _isGravityDisabled = true;
	public void EnableGravity() => _isGravityDisabled = false;
	public void DisableDash() => _canDash = false;
	public void EnableDash() => _canDash = true;



	public void PlayIdleAnimation()
	{
		BodyMinusRightHandAnimation.Play("Idle");
		RightHandAnimation.Play("Idle");
	}

	public void PlayJumpAnimation()
	{
		BodyMinusRightHandAnimation.Play("Jump");
		RightHandAnimation.Play("Jump");
	}

	public void PlayFallAnimation()
	{
		BodyMinusRightHandAnimation.Play("Fall");
		RightHandAnimation.Play("Fall");
	}

	public void PlayWallHangAnimation()
	{
		BodyMinusRightHandAnimation.Play("WallHang");
		RightHandAnimation.Play("WallHang");
	}

	public void PlayRunAnimation()
	{
		BodyMinusRightHandAnimation.Play("Run");
		RightHandAnimation.Play("Run");
	}

	public void PlayDashAnimation()
	{
		BodyMinusRightHandAnimation.Play("Dash");
		RightHandAnimation.Play("Dash");
	}

	public void PlayGroundShootAnimation()
	{
		RightHandAnimation.Play("Ground_shoot");
	}

	public void PlayAirShootAnimation()
	{
		RightHandAnimation.Play("Air_shoot");
	}

	public void FlipSprite(Vector2 direction)
	{
		Direction = direction;
		PlayerSprite.Scale = PlayerSprite.Scale with { X = Math.Abs(PlayerSprite.Scale.X) * direction.X };
	}


	public void StartGhosting(double duration)
	{
		isGhosting = true;
		ghostTimer = 0f;

		GetTree()
			.CreateTimer(duration)
			.Connect("timeout", Callable.From(() => StopGhosting()));
	}

	private void StopGhosting()
	{
		isGhosting = false;
	}

	private void SpawnGhost()
	{
		var ghost = (Node2D)PlayerSprite.Duplicate((int)DuplicateFlags.UseInstantiation);
		ghost.GlobalPosition = PlayerSprite.GlobalPosition;
		ghost.Scale = PlayerSprite.Scale;
		ghost.Modulate = new Color(1, 1, 1, 0.5f);

		GetParent().AddChild(ghost);

		var tween = ghost.CreateTween();
		tween.TweenProperty(ghost, "modulate:a", 0f, 0.5f);
		tween.TweenCallback(Callable.From(() => ghost.QueueFree()));
		tween.Play();
	}
}