// using Content.Shared.Armor;
// using Content.Shared.Damage;
// using Content.Shared.ADT.Armor;
// using Robust.Shared.Prototypes;
// using Robust.Shared.GameObjects;
// using Content.Shared.Whitelist;

// namespace Content.Server.Armor;

// public sealed class ArmorPlateSystem : EntitySystem
// {
//     [Dependency] private readonly SharedContainerSystem _containers = default!;
//     [Dependency] private readonly ArmorSystem _armorSystem = default!; // Ссылка на систему брони
//     [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

//     public override void Initialize()
//     {
//         base.Initialize();
//         SubscribeLocalEvent<ArmorPlateComponent, ComponentStartup>(OnStartup);
//         SubscribeLocalEvent<ArmorPlateComponent, EntInsertedIntoContainerMessage>(OnInsertedOrRemoved);
//         SubscribeLocalEvent<ArmorPlateComponent, EntRemovedFromContainerMessage>(OnInsertedOrRemoved);
//         SubscribeLocalEvent<ArmorPlateComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
//         SubscribeLocalEvent<ArmorPlateComponent, DamageModifyEvent>(OnDamageModify);
//     }

//     private void OnStartup(EntityUid uid, ArmorPlateComponent comp, ComponentStartup args)
//     {
//         comp.Container = _containers.EnsureContainer<Container>(uid, comp.ContainerId);
//         UpdateArmor(uid, comp);
//     }

//     private void OnInsertAttempt(EntityUid uid, ArmorPlateComponent comp, ContainerIsInsertingAttemptEvent args)
//     {
//         if (args.Container.ID != comp.ContainerId)
//             return;

//         // Проверка whitelist через систему
//         if (comp.Whitelist is not null && !_whitelist.IsValid(comp.Whitelist, args.EntityUid))
//         {
//             args.Cancel();
//             return;
//         }

//         if (comp.Container?.ContainedEntities.Count >= comp.MaxSlots)
//         {
//             args.Cancel();
//             return;
//         }
//     }

//     private void OnInsertedOrRemoved(EntityUid uid, ArmorPlateComponent comp, ContainerModifiedMessage args)
//     {
//         if (args.Container.ID != comp.ContainerId)
//             return;

//         UpdateArmor(uid, comp);
//     }

//     private void UpdateArmor(EntityUid uid, ArmorPlateComponent comp)
//     {
//         if (!TryComp(uid, out ArmorComponent? armor))
//             return;

//         var combined = new DamageModifierSet
//         {
//             Coefficients = new Dictionary<string, float>(armor.Modifiers.Coefficients),
//             FlatReduction = new Dictionary<string, float>(armor.Modifiers.FlatReduction)
//         };

//         if (comp.Container != null)
//         {
//             foreach (var plate in comp.Container.ContainedEntities)
//             {
//                 if (!TryComp(plate, out ArmorComponent? plateArmor))
//                     continue;

//                 foreach (var (type, value) in plateArmor.Modifiers.Coefficients)
//                     combined.Coefficients[type] = combined.Coefficients.TryGetValue(type, out var existing) ? existing * value : value;

//                 foreach (var (type, value) in plateArmor.Modifiers.FlatReduction)
//                     combined.FlatReduction[type] = combined.FlatReduction.TryGetValue(type, out var existing) ? existing + value : value;
//             }
//         }

//         comp.CombinedModifiers = combined;
//         Dirty(uid, comp);
//     }

//     private void OnDamageModify(EntityUid uid, ArmorPlateComponent comp, DamageModifyEvent args)
//     {
//         // Применяем модификаторы через ArmorSystem
//         args.Damage = _armorSystem.ApplyModifierSet(args.Damage, comp.CombinedModifiers);
//     }
// }