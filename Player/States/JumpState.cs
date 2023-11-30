using Godot;
using Godot.Collections;
using System;

public partial class JumpState : BasePlayerState
{
    private bool _isStuckOnWall = false;

    public override void OnEnter(Dictionary<string, Variant> message = null)
    {
        base.OnEnter(message);
        PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.JumpVelocity };
    }

    public override void OnPhysicsUpdate(double delta)
    {
        base.OnPhysicsUpdate(delta);
        if (PlayerV.IsOnFloor())
        {
            EmitSwitchState("IdleState");
        }

        HandleXAirMovements(delta);
        ApplyAirResistance(delta);
        HandleWallJump();

    }

    private void HandleXAirMovements(double delta)
    {
        if (PlayerV.InputDir.X != 0 && !PlayerV.IsOnFloor())
        {
            PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, PlayerV.MovementData.Speed * PlayerV.InputDir.X, PlayerV.MovementData.AirAcceleration * delta) };
        }
    }
    private void ApplyAirResistance(double delta)
    {

        if (PlayerV.InputDir.X == 0 && !PlayerV.IsOnFloor())
        {
            PlayerV.Velocity = PlayerV.Velocity with { X = (float)Mathf.MoveToward(PlayerV.Velocity.X, 0, PlayerV.MovementData.AirResistance * delta) };
        }
    }

    private void HandleWallJump()
    {
        if (PlayerV.IsOnWallOnly())
        {
            if (Input.IsActionPressed("hang"))
            {
                PlayerV.Velocity = PlayerV.Velocity with { Y = 0 };
                _isStuckOnWall = true;
            }
            else if (_isStuckOnWall) // isOnWall but hang not pressed => wallSlide
            {
                PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.WallSlideAcceleration };
            }

            if (Input.IsActionJustPressed("jump") && _isStuckOnWall)
            {
                _isStuckOnWall = false;
                var wallJumpYPower = -80;
                var wallJumpXPower = 80;
                PlayerV.Velocity = PlayerV.Velocity with { Y = wallJumpYPower };
                PlayerV.Velocity = PlayerV.Velocity with { X = PlayerV.GetWallNormal().X * wallJumpXPower };
            }
        }
    }

}
