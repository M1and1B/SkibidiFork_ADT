using System.Numerics;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Content.Shared.ADT.SpacePod;

namespace Content.Shared.ADT.SpacePod;

public sealed partial class SharedSpacePodSystem : EntitySystem
{
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedMoverController _mover = default!;

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<
            SpacePodComponent,
            InputMoverComponent,
            PhysicsComponent>();

        while (query.MoveNext(out var uid, out var accel, out var mover, out var physics))
        {
            var velocityInput = _mover.GetVelocityInput(mover);
            var input = velocityInput.Sprinting;

            if (input == Vector2.Zero)
            {
                var speed = accel.Velocity.Length();
                if (speed > 0f)
                {
                    var drop = accel.Deceleration * frameTime;
                    var newSpeed = MathF.Max(speed - drop, 0f);

                    accel.Velocity = newSpeed == 0f
                        ? Vector2.Zero
                        : Vector2.Normalize(accel.Velocity) * newSpeed;
                }
            }
            else
            {
                accel.Velocity += input * accel.Acceleration * frameTime;

                if (accel.Velocity.LengthSquared() >
                    accel.MaxSpeed * accel.MaxSpeed)
                {
                    accel.Velocity =
                        Vector2.Normalize(accel.Velocity) * accel.MaxSpeed;
                }
            }

            _physics.SetLinearVelocity(uid, accel.Velocity);
        }
    }
}