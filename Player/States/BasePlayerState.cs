﻿using Godot;


namespace Player.States;

public partial class BasePlayerState : State
{
    [Export] public Player PlayerV;
    public enum playerMsgKeys {
        wallJump,
    }
    public override void _Ready()
    {
        PlayerV = Owner as Player;
    }
}
