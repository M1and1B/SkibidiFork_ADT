using Robust.Shared.GameStates;

namespace Content.Shared.ADT.SpacePod.Components;

/// <summary>
/// Компонент для пассажира космического транспорта
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpacePodPilotComponent : Component
{
    /// <summary>
    /// Транспорт, в котором находится пассажир
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public EntityUid SpacePod;
}








