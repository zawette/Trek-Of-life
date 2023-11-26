using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StateMachine : Node
{
	[Export]
	public State InitialState;
	public State CurrentState;
	public string PreviousStateName;
	public Dictionary<string, State> States;


	public override void _Ready()
	{
		if (InitialState is null)
			return;

		foreach (var state in GetChildren().OfType<State>())
		{
			var stateName = state.GetType().Name;
            state.SwitchState += OnSwitchState;
			States.Add(stateName, state);
			GD.Print(stateName);
		}

		CurrentState = InitialState;
		CurrentState.OnEnter();

	}

	public override void _PhysicsProcess(double delta)
	{
		if (CurrentState is null)
			return;

		CurrentState.OnPhysicsUpdate(delta);
	}

	public void OnSwitchState(string stateName, Godot.Collections.Dictionary<string,Variant> message = null)
	{
		var state = States[stateName];
		if (state is null || state == CurrentState)
			return;

		PreviousStateName = CurrentState.GetType().Name;
		CurrentState.OnExit();

		CurrentState = state;
		CurrentState.OnEnter(message);
	}
}
