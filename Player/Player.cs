using Godot;
using Player.weapons;
using System;

namespace Player;
public partial class Player : CharacterBody2D
{
	[Export] public PlayerMovementData MovementData;
	[Export] public bool IsAutoRunning;
	[Export] public Vector2 AutoRunDirection;
	[Export] public float StartDelayTime = 7.0f;
	private Timer _startDelayTimer;
	public bool IsMovementDelayed; // Flag to delay movement

	public Vector2 InputDir = Vector2.Zero;
	public Timer CoyoteJumpTimer;

	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public AnimationPlayer LegsAnimation;
	public AnimatedSprite2D LegsSprite;
	private AnimatedSprite2D _bodySprite;
	private AnimatedSprite2D _frontArmSprite;
	private Marker2D _handRight;
    private Marker2D _handLeft;
    private AnimatedSprite2D _headSprite;
	private CharacterBody2D _characterBody2D;
	private Node2D _playerSprite;


    public override void _Ready()
	{
		_playerSprite = GetNode<Node2D>("PlayerSprite");
		CoyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		LegsAnimation = GetNode<AnimationPlayer>("LegsAnimation");
		_frontArmSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body/Front_Arm");
		_handRight = GetNode<Marker2D>("PlayerSprite/Body/Front_Arm/HandRight");
		_handLeft = GetNode<Marker2D>("PlayerSprite/Body/Front_Arm/HandLeft");
		LegsSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Legs");
		_bodySprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body");
		_headSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body/Head");
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
		HandleAim();
		AlignCharToSlope();
	}

	private void HandleAim()
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

	private void AddGravity(double delta)
	{
		if (!IsOnFloor())
			Velocity = Velocity with { Y = Velocity.Y + Gravity * MovementData.GravityScale * (float)delta };
	}

	private void AlignCharToSlope()
	{
		if (IsOnFloor())
		{
			var normal = GetFloorNormal();
			_playerSprite.Rotation = normal.Angle() + Mathf.DegToRad(90);
		}
	}

	private void OnStartDelayTimeout()
	{
		IsMovementDelayed = false;
	}

	public bool IsPlayerJumping() => Velocity.Y < 0;
	public bool IsPlayerFalling() => !IsOnFloor() && Velocity.Y >= 0;
}
