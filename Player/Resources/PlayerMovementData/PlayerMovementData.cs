using Godot;
using System;

[GlobalClass]
public partial class PlayerMovementData : Resource 
{
    [Export]
    public float Speed { get; set; } = 150.0f;
    [Export]
    public float Acceleration { get; set; } = 550.0f;
    [Export]
    public float AirAcceleration { get; set; } = 300.0f;
    [Export]
    public float Friction { get; set; } = 950.0f;
    [Export]
    public float AirResistance { get; set; } = 300.0f;
    [Export]
    public float JumpVelocity { get; set; } = -250.0f;
    [Export]
    public float LowJumpMultiplier { get; set; } = 0.06f;
    [Export]
    public float GravityScale { get; set; } = 1f;
    [Export]
    public byte AdditionalJumps { get; set; } = 1;
    [Export]
    public float AdditionalJumpsVelocityMultiplier { get; set; } = 0.8f;
}
