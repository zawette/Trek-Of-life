using Godot;
using System;

[GlobalClass]
public partial class PlayerMovementData : Resource
{
	[Export] public float Speed { get; set; } = 40.0f;
	[Export] public float Acceleration { get; set; } = 150.0f;
	[Export] public float AirAcceleration { get; set; } = 100.0f;
	[Export] public float Friction { get; set; } = 10.0f;
	[Export] public float AirResistance { get; set; } = 100.0f;
	[Export] public float JumpVelocity { get; set; } = -80.0f;
	[Export] public float LowJumpMultiplier { get; set; } = 0.03f;
	[Export] public float GravityScale { get; set; } = 0.3f;
	[Export] public float WallSlideAcceleration { get; set; } = 8;
	[Export] public float WallJumpYPower { get; set; } = -50;
	[Export] public float WallJumpXPower { get; set; } = 50;
	[Export] public float DashPower { get; set; } = 75;
	[Export] public double DashDuration { get; set; } = 0.2;
	[Export] public float GhostSpawnInterval { get; set; } = 0.07f;
	[Export] public double GhostingDurationMultiplier { get; set; } = 2.0f;
	[Export] public double DashDelay { get; set; } = 0.4;

}
