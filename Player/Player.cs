using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public PlayerMovementData MovementData;
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	//private AnimatedSprite2D _animatedSprite2D;
	private AnimationPlayer _legsAnimation;
	private CharacterBody2D _characterBody2D;
	private Node2D _playerSprite;
	private Timer _coyoteJumpTimer;
	private byte _additionalJumpsCount;
	private bool _isAirJump;
	private bool _isStuckOnWall;

	public override void _Ready()
	{
		//_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_playerSprite = GetNode<Node2D>("PlayerSprite");
		_coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		_additionalJumpsCount = MovementData.AdditionalJumps;
		_legsAnimation = GetNode<AnimationPlayer>("LegsAnimation");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		AddGravity(delta);
		HandleXMovements(delta, input);
		ApplyFriction(delta, input);
		ApplyAirResistance(delta, input);
		HandleJump(delta);
		UpdateAnimations(input);
		var wasOnfloor = IsOnFloor();
		if (IsOnWallOnly())
		{
			if (Input.IsActionPressed("hang"))
			{
				Velocity = Velocity with { Y = 0 };
				_isStuckOnWall = true;
			}
			else if (_isStuckOnWall)
			{
				var wallSlideAcceleration = 5;
				Velocity = Velocity with { Y = wallSlideAcceleration };
			}
			if (Input.IsActionJustPressed("jump") && _isStuckOnWall)
			{
				_isStuckOnWall = false;
				var wallJumpYPower = -80;
				var wallJumpXPower = 80;
				Velocity = Velocity with { Y = wallJumpYPower };
				Velocity = Velocity with { X = GetWallNormal().X * wallJumpXPower };
			}
		}
		if (IsOnFloor())
		{
			_isStuckOnWall = false;
		}
		MoveAndSlide();
		AlignCharToSlope();
		HandleCoyoteTimer(wasOnfloor);

	}

	private void AddGravity(double delta)
	{
		if (!IsOnFloor())
			Velocity = Velocity with { Y = Velocity.Y + gravity * MovementData.GravityScale * (float)delta };
	}
	private void HandleJump(double delta)
	{
		if (IsOnFloor() || _coyoteJumpTimer.TimeLeft > 0)
		{
			_additionalJumpsCount = MovementData.AdditionalJumps;
			_isAirJump = true;
			if (Input.IsActionJustPressed("jump"))
			{
				Velocity = Velocity with { Y = MovementData.JumpVelocity };
			}
		}
		else if (!IsOnFloor())
		{
			if (Input.IsActionJustReleased("jump") && IsPlayerJumping())
			{
				Velocity = Velocity with { Y = Velocity.Y + MovementData.LowJumpMultiplier * gravity };
			}

			if (Input.IsActionJustPressed("jump") && _isAirJump && _additionalJumpsCount > 0)
			{
				Velocity = Velocity with { Y = MovementData.JumpVelocity * MovementData.AdditionalJumpsVelocityMultiplier };
				_additionalJumpsCount--;
			}

			if (_additionalJumpsCount == 0)
			{
				_isAirJump = false;
			}
		}
	}

	private void HandleXMovements(double delta, Vector2 input)
	{

		if (input.X != 0 && IsOnFloor())
		{
			Velocity = Velocity with { X = (float)Mathf.MoveToward(Velocity.X, MovementData.Speed * input.X, MovementData.Acceleration * delta) };
		}

		if (input.X != 0 && !IsOnFloor())
		{
			Velocity = Velocity with { X = (float)Mathf.MoveToward(Velocity.X, MovementData.Speed * input.X, MovementData.AirAcceleration * delta) };
		}
	}

	private void ApplyFriction(double delta, Vector2 input)
	{

		if (input.X == 0 && IsOnFloor())
		{
			Velocity = Velocity with { X = (float)Mathf.MoveToward(Velocity.X, 0, MovementData.Friction * delta) };
		}
	}

	private void ApplyAirResistance(double delta, Vector2 input)
	{

		if (input.X == 0 && !IsOnFloor())
		{
			Velocity = Velocity with { X = (float)Mathf.MoveToward(Velocity.X, 0, MovementData.AirResistance * delta) };
		}
	}

	private void UpdateAnimations(Vector2 input)
	{
		if (input.X != 0)
		{
			//_animatedSprite2D.Play("Run");
			//_animatedSprite2D.FlipH = input.X < 0;
			_legsAnimation.Play("RunForward");
		}
		else
			_legsAnimation.Play("RunForward");
		//_animatedSprite2D.Play("Idle");
		if (!IsOnFloor())
			_legsAnimation.Play("RunForward");
		//_animatedSprite2D.Play("Jump");
	}

	private void HandleCoyoteTimer(bool wasPlayerOnTheFloor)
	{
		if (wasPlayerOnTheFloor && IsPlayerFalling())
		{
			_coyoteJumpTimer.Start();
		}
	}

	private void AlignCharToSlope()
	{
		if (IsOnFloor())
		{
			var normal = GetFloorNormal();
			_playerSprite.Rotation = normal.Angle() + Mathf.DegToRad(90);
		}
	}

	private bool IsPlayerJumping() => Velocity.Y < 0;
	private bool IsPlayerFalling() => !IsOnFloor() && Velocity.Y >= 0;
}
