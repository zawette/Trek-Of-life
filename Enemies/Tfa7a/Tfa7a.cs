using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enemies.Tfa7a;
public partial class Tfa7a : CharacterBody2D
{
	[Export] public float Speed;
	[Export] public int StateWaitTime;
	[Export] public Node PatrolPointsNode;
	public List<Marker2D> patrolPointsList;
	public int currentPointId;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public AnimatedSprite2D Tfa7aAnimatedSprite2d;
    public Timer StateTimer;


    public override void _Ready()
	{
		Tfa7aAnimatedSprite2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		StateTimer = GetNode<Timer>("Timer");
		StateTimer.WaitTime = StateWaitTime;

		if (PatrolPointsNode is null) GD.Print("patrol points not found");
		else{
			patrolPointsList = PatrolPointsNode.GetChildren().OfType<Marker2D>().ToList();
			currentPointId = 0;
		}
	}

	public override void _PhysicsProcess(double delta)
	{

		//handle gravity => move to seperate function
		if (!IsOnFloor())
			Velocity = Velocity with { Y = Velocity.Y +gravity * (float)delta };

	}
}
