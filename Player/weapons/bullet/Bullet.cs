using Godot;
using System;

namespace Player.weapons;

public partial class Bullet : Node2D
{
    private VisibleOnScreenNotifier2D _visibleOnScreenEnabler2D;
    private Area2D _hitBox;

    [Export] public float Speed = 20f;
    public Vector2 Direction { get; set; }
    public override void _Ready()
    {
        _visibleOnScreenEnabler2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        _hitBox = GetNode<Area2D>("Hitbox");
        _visibleOnScreenEnabler2D.ScreenExited += OnScreenExited;
        _hitBox.AreaEntered += OnAreaEntered;
        _hitBox.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        QueueFree();
    }


    private void OnAreaEntered(Area2D area)
    {
        area.GetParent().QueueFree();
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
