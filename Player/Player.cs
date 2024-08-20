using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public PlayerMovementData MovementData;
	public Vector2 InputDir = Vector2.Zero;
	public Timer CoyoteJumpTimer;

	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public AnimationPlayer LegsAnimation;
	public AnimatedSprite2D LegsSprite;
	private AnimatedSprite2D _bodySprite;
	private AnimatedSprite2D _frontArmSprite;
	private Marker2D _hand;
	private AnimatedSprite2D _headSprite;
	private CharacterBody2D _characterBody2D;
	private Node2D _playerSprite;

	public override void _Ready()
	{
		_playerSprite = GetNode<Node2D>("PlayerSprite");
		CoyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		LegsAnimation = GetNode<AnimationPlayer>("LegsAnimation");
		_frontArmSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body/Front_Arm");
		_hand = GetNode<Marker2D>("PlayerSprite/Body/Front_Arm/Hand");
		LegsSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Legs");
		_bodySprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body");
		_headSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body/Head");
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
		var mousePosition = (GetGlobalMousePosition() - _hand.GlobalPosition).Normalized();
		var shouldFlip = mousePosition.X < 0;

		_frontArmSprite.Rotation = mousePosition.Angle();

		_frontArmSprite.FlipV = shouldFlip;
		_bodySprite.FlipH = shouldFlip;
		_headSprite.FlipH = shouldFlip;

		//GD.Print(mousePosition);
		//GD.Print(mousePosition.Angle());
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

	public bool IsPlayerJumping() => Velocity.Y < 0;
	public bool IsPlayerFalling() => !IsOnFloor() && Velocity.Y >= 0;
}
