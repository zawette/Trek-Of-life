using Godot;
using System;

namespace Player.States;

public partial class PlayerStateMachine : StateMachine
{
	public Label DebugState;
	public override async void _Ready()
	{
		await ToSignal(Owner, SignalName.Ready); //waiting for player to be ready
		base._Ready();
		DebugState = Owner.GetNode<Label>("DebugState");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		DebugState.Text = CurrentState.GetType().Name;

    }
}
