using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public PlayerMovementData MovementData;
	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private AnimationPlayer _legsAnimation;
	private AnimatedSprite2D _legsSprite;
	private AnimatedSprite2D _bodySprite;
	private AnimatedSprite2D _frontArmSprite;
	private AnimatedSprite2D _headSprite;
	private CharacterBody2D _characterBody2D;
	private Node2D _playerSprite;
	private Timer _coyoteJumpTimer;
	private byte _additionalJumpsCount;
	private bool _isAirJump;
	private bool _isStuckOnWall;

	public override void _Ready()
	{
		_playerSprite = GetNode<Node2D>("PlayerSprite");
		_coyoteJumpTimer = GetNode<Timer>("CoyoteJumpTimer");
		_additionalJumpsCount = MovementData.AdditionalJumps;
		_legsAnimation = GetNode<AnimationPlayer>("LegsAnimation");
		_frontArmSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body/Front_Arm");
		_legsSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Legs");
		_bodySprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body");
		_headSprite = GetNode<AnimatedSprite2D>("PlayerSprite/Body/Head");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		AddGravity(delta);
		HandleAim();
		HandleXMovements(delta, input);
		ApplyFriction(delta, input);
		ApplyAirResistance(delta, input);
		HandleJump(delta);
		UpdateAnimations(input);
		var wasOnfloor = IsOnFloor();
		HandleWallJump();
		MoveAndSlide();
		AlignCharToSlope();
		HandleCoyoteTimer(wasOnfloor);

	}

	private void HandleAim()
	{
		var mousePosition = (GetGlobalMousePosition() - GlobalPosition).Normalized();
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

	private void HandleWallJump()
	{
		if (IsOnWallOnly())
		{
			if (Input.IsActionPressed("hang"))
			{
				Velocity = Velocity with { Y = 0 };
				_isStuckOnWall = true;
			}
			else if (_isStuckOnWall) // isOnWall but hang not pressed => wallSlide
			{
				Velocity = Velocity with { Y = MovementData.WallSlideAcceleration };
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
			_legsSprite.FlipH = input.X < 0;
			_legsAnimation.Play("RunForward");
		}
		else
			_legsAnimation.Play("Idle");
		if (!IsOnFloor())
			_legsAnimation.Play("Jump");
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
