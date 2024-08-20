using Godot;
using System;

public partial class PlayerStateMachine : StateMachine
{
	public Label DebugState;
	public override async void _Ready()
	{
		await ToSignal(Owner, SignalName.Ready);
		base._Ready();
		DebugState = Owner.GetNode<Label>("DebugState");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		DebugState.Text = CurrentState.GetType().Name;

    }
}
