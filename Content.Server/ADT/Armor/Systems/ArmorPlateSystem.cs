using Content.Shared.Whitelist;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Content.Shared.Armor;
using Content.Shared.Damage;
using Content.Shared.ADT.Armor;
using Robust.Shared.GameObjects;

namespace Content.Server.ADT.Armor;

public sealed class ArmorPlateSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containers = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ArmorPlateComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ArmorPlateComponent, DamageModifyEvent>(OnDamageModify);
        SubscribeLocalEvent<ArmorPlateComponent, EntInsertedIntoContainerMessage>(OnContainerModified);
        SubscribeLocalEvent<ArmorPlateComponent, EntRemovedFromContainerMessage>(OnContainerModified);
    }

    private void OnStartup(EntityUid uid, ArmorPlateComponent comp, ComponentStartup args)
    {
        // Создаём ContainerSlot
        comp.Container = _containers.EnsureContainer<ContainerSlot>(uid, "plates");

        // Создаём ItemSlotsComponent, если нет
        var slots = EnsureComp<ItemSlotsComponent>(uid);

        if (!slots.Slots.ContainsKey("plates"))
        {
            var plateSlot = new ItemSlot
            {
                Name = "plates",
                Whitelist = new EntityWhitelist
                {
                    Components = new[] { "Armor" } // массив
                },
                InsertOnInteract = true,
                EjectOnInteract = true,
                Swap = true
            };

            _itemSlots.AddItemSlot(uid, "plates", plateSlot);
        }

        UpdateArmor(uid, comp);
    }

    private void OnContainerModified(EntityUid uid, ArmorPlateComponent comp, ContainerModifiedMessage args)
    {
        if (args.Container.ID != "plates")
            return;

        UpdateArmor(uid, comp);
    }

    private void UpdateArmor(EntityUid uid, ArmorPlateComponent comp)
    {
        if (!TryComp(uid, out ArmorComponent? armor))
            return;

        var combined = new DamageModifierSet
        {
            Coefficients = new Dictionary<string, float>(armor.Modifiers.Coefficients),
            FlatReduction = new Dictionary<string, float>(armor.Modifiers.FlatReduction)
        };

        // Проверяем наличие предмета в ContainerSlot
        if (comp.Container?.ContainedEntity is { } plate &&
            TryComp(plate, out ArmorComponent? plateArmor))
        {
            foreach (var (type, value) in plateArmor.Modifiers.Coefficients)
                combined.Coefficients[type] = combined.Coefficients.TryGetValue(type, out var existing) ? existing * value : value;

            foreach (var (type, value) in plateArmor.Modifiers.FlatReduction)
                combined.FlatReduction[type] = combined.FlatReduction.TryGetValue(type, out var existing) ? existing + value : value;
        }

        comp.CombinedModifiers = combined;
        Dirty(uid, comp);
    }

    private void OnDamageModify(EntityUid uid, ArmorPlateComponent comp, DamageModifyEvent args)
    {
        args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, comp.CombinedModifiers);
    }
}