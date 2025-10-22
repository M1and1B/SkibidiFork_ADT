using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;

namespace Content.Shared.Weapons.Marker;

/// <summary>
/// Marks an entity to take additional damage
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DamageModificatorComponent : Component
{
    [DataField("damage")]
    public DamageSpecifier Damage = new();
}
