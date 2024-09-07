using Godot;
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
	public Node2D PlayerSprite;
	private bool _isGravityDisabled;

	private Timer _startDelayTimer;

	public override void _Ready()
	{
		PlayerSprite = GetNode<Node2D>("PlayerSprite");
		CoyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		BodyMinusRightHandAnimation = GetNode<AnimationPlayer>("BodyMinusRightHandAnimation");
		RightHandAnimation = GetNode<AnimationPlayer>("RightHandAnimation");
		//_handRight = GetNode<Marker2D>("PlayerSprite/Body/Front_Arm/HandRight");
		//_handLeft = GetNode<Marker2D>("PlayerSprite/Body/Front_Arm/HandLeft");
		LegsSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Legs");
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
        //HandleAim();
        AlignCharToSlope();
        HandleGhosting(delta);
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

    /*private void HandleAim()
	{
		var globalMousePosition = GetGlobalMousePosition();
		var direction = (globalMousePosition - _handRight.GlobalPosition).Normalized();
		var shouldFlip = direction.X < 0;
		_frontArmSprite.LookAt(GetGlobalMousePosition());
		var currentHand = shouldFlip ? _handLeft : _handRight;
		_frontArmSprite.FlipV = shouldFlip;
		_bodySprite.FlipH = shouldFlip;
		_headSprite.FlipH = shouldFlip;
	
		if(Input.IsActionJustPressed("shoot")){
			var bullet = GD.Load<PackedScene>("res://Player/weapons/bullet/Bullet.tscn").Instantiate<Bullet>();
			GetParent().AddChild(bullet);
			bullet.GlobalPosition = currentHand.GlobalPosition;
			bullet.Direction = direction;
		}

	}
	*/

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
		BodyMinusRightHandAnimation.Play("RunForward");
		RightHandAnimation.Play("RunForward");
	}

	public void PlayDashAnimation()
	{
		BodyMinusRightHandAnimation.Play("Dash");
		RightHandAnimation.Play("Dash");
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

		//SpawnGhost();
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
		var ghost = (Node2D)PlayerSprite.Duplicate();
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