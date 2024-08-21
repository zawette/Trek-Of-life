using Godot;
using System;


namespace Enemies.Tfa7a.States;

public partial class Tfa7aStateMachine : StateMachine
{
	public Label DebugState;
	public override async void _Ready()
	{
		await ToSignal(Owner, SignalName.Ready); //waiting for owner to be ready
		base._Ready();
		DebugState = Owner.GetNode<Label>("DebugState");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		DebugState.Text = CurrentState.GetType().Name;

    }
}
