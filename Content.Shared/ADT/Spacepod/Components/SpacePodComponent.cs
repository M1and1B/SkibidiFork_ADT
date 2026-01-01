using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using System.Numerics;

namespace Content.Shared.ADT.SpacePod;

[RegisterComponent, NetworkedComponent]
public sealed partial class SpacePodComponent : Component
{
    [DataField] public float Acceleration = 2f;
    [DataField] public float Deceleration = 1;
    [DataField] public float MaxSpeed = 5f;

    [ViewVariables(VVAccess.ReadWrite)]
    public Vector2 Velocity = Vector2.Zero;
}


