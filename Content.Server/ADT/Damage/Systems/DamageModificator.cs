using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
Content.Shared.Weapons.Marker;

namespace Content.Shared.ADT.Damage.Systems;

public abstract class DamageModificatorSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamageMarkerComponent, AttackedEvent>(OnAttack);
    }

    private void OnAttack(EntityUid uid, DamageModificatorComponent comp, AttackedEvent args)
    {
        args.BonusDamage += component.Damage;
        if (TryComp<LeechOnMarkerComponent>(args.Used, out var leech))
        {
            _damageable.TryChangeDamage(args.User, leech.Leech, true, false, origin: args.Used);
        }
    }
}
