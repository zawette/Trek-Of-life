using Godot;
using Player.weapons;
using System;

public partial class DoorActivator : StaticBody2D
{
    private Area2D _activatorHurtBox;
    private AnimationPlayer _doorAnimation;


    public override void _Ready()
	{
        _activatorHurtBox = GetNode<Area2D>("Activator/Hurtbox");
        _doorAnimation = GetNode<AnimationPlayer>("DoorAnimation");
		_activatorHurtBox.AreaEntered += OnAreaEntered;
	}

    private void OnAreaEntered(Area2D area)
    {
		if (area.GetParent() is Bullet )
		{
			_doorAnimation.Play("Open");
		}
    }

    public override void _Process(double delta)
	{
		
	}
}
