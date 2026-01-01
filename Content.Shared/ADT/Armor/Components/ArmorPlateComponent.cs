using Robust.Shared.GameStates;
using Robust.Shared.Containers;
using Content.Shared.Damage;
using Content.Shared.Whitelist;

namespace Content.Shared.ADT.Armor;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ArmorPlateComponent : Component
{
    [DataField, AutoNetworkedField]
    public string ContainerId = "plates";

    [DataField("maxSlots"), AutoNetworkedField]
    public int MaxSlots = 2;

    [DataField, AutoNetworkedField]
    public EntityWhitelist? Whitelist;

    [ViewVariables]
    public ContainerSlot? Container;

    [DataField, AutoNetworkedField]
    public DamageModifierSet CombinedModifiers = new();
}