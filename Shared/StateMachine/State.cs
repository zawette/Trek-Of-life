using Godot;
using System;

public partial class State : Node
{

	[Signal] public delegate void StateStartEventHandler();
	[Signal] public delegate void StatePhysicsUpdateEventHandler();
	[Signal] public delegate void StateExitEventHandler();
	[Signal] public delegate void SwitchStateEventHandler(string stateName, Godot.Collections.Dictionary<string,Variant> message = null);


	public virtual void OnEnter(Godot.Collections.Dictionary<string,Variant> message = null)
	{
		EmitSignal(nameof(StateStart));
	}


	public virtual void OnPhysicsUpdate(double delta)
	{
		EmitSignal(nameof(StatePhysicsUpdate));
	}

	public virtual void OnExit()
	{
		EmitSignal(nameof(StateExit));
	}

	public void EmitSwitchState(string stateName, Godot.Collections.Dictionary<string,Variant> message = null)
	{
		EmitSignal(nameof(SwitchState), stateName, message);
	}
}
