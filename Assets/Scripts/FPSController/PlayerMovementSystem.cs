using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[UpdateBefore(typeof(LateSimulationSystemGroup))]
partial class PlayerMovementSystem : SystemBase
{
    const float speed = 3f;

    protected override void OnUpdate()
    {
        // Camera position offset relative to player's position.
        float3 camOffset = new(0, 1.5f, 0);
        Vector3 camPosition = default;
        float2 moveVelocity = default;
        float3 moveInput = new float3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));
        Transform camTransform = CameraControl.Instance.transform;
        Quaternion camRotation = Quaternion.Euler(0, camTransform.rotation.eulerAngles.y, 0);

        Entities
            .WithAll<PlayerTag>()
           .ForEach((ref LocalTransform local, ref PhysicsVelocity vel) =>
            {
                // Sync camera and player.
                camPosition = local.Position + camOffset;
                local.Rotation = camRotation;

                // Move forward & backward.
                if (moveInput.x != 0)
                {
                    moveVelocity += new float2(local.Right().x, local.Right().z) * moveInput.x * speed;
                }

                // Strafe right & left.
                if (moveInput.z != 0)
                {
                    moveVelocity += new float2(local.Forward().x, local.Forward().z) * moveInput.z * speed;
                }

                // Jump up.
                if (moveInput.y > 0)
                {
                    vel.Linear.y = local.Up().y * moveInput.y * speed;
                }

                // Move rigid body based on input.
                vel.Linear.xz = moveVelocity;
            })
        .Run();
        //.WithBurst().ScheduleParallel();
        camTransform.position = camPosition;
    }
}
