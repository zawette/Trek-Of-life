using Godot;
using Godot.Collections;
using System;

namespace Player.States;

public partial class WallHangState : BasePlayerState
{
    public override void OnEnter(Dictionary<string, Variant> message = null)
    {
        base.OnEnter(message);
        PlayerV.Velocity = PlayerV.Velocity with { Y = 0 };

    }

    public override void OnPhysicsUpdate(double delta)
    {
        base.OnPhysicsUpdate(delta);
        HandleWallJump();
        PlayerV.MoveAndSlide();
    }


    // move to wall hang state
    private void HandleWallJump()
    {
        if (PlayerV.IsOnWallOnly())
        {
            if (Input.IsActionPressed("hang"))
            {
                PlayerV.Velocity = PlayerV.Velocity with { Y = 0 };
            }
            else
            {
                PlayerV.Velocity = PlayerV.Velocity with { Y = PlayerV.MovementData.WallSlideAcceleration };
            }

            if (Input.IsActionJustPressed("jump"))
            {
                var dic = new Godot.Collections.Dictionary<string, Variant>() { { playerMsgKeys.wallJump.ToString(), true } };
                EmitSwitchState("JumpState", dic);
            }
        }
        else
        {
            EmitSwitchState("IdleState");
        }

    }

}
