using Godot;
using System;

public partial class PlayerStateMachine : StateMachine
{
	public Label DebugState;
	public override void _Ready()
	{
		base._Ready();
		DebugState = Owner.GetNode<Label>("DebugState");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		DebugState.Text = CurrentState.GetType().Name;

    }
}
