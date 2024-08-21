
using Godot;

namespace Enemies.Tfa7a.States;

public partial class BaseTfa7aState : State
{

    [Export] public Tfa7a Tfa7aV;


    public override void _Ready()
    {
        Tfa7aV = Owner as Tfa7a;
    }
}