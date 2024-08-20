using Godot;
using System;

public partial class Bullet : Node2D
{
    private VisibleOnScreenNotifier2D _visibleOnScreenEnabler2D;
    [Export] public float Speed = 20f;
    public Vector2 Direction { get; set; }

    public override void _Ready()
    {
        _visibleOnScreenEnabler2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        _visibleOnScreenEnabler2D.ScreenExited += OnScreenExited;
    }

    private void OnScreenExited()
    {
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
        GD.Print("bullet position ", Position);
    }
}
