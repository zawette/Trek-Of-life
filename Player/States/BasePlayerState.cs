using Godot;


public partial class BasePlayerState : State
{
    [Export] public Player PlayerV;
    public enum playerMsgKeys {
        freeFall
    }
    public override void _Ready()
    {
        PlayerV = Owner as Player;
    }
}
