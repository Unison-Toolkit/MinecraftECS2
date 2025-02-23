using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(AddBlockSystem))]
partial class RemoveBlockSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimECBSystem;

    protected override void OnCreate()
    {
        m_BeginSimECBSystem = World.GetExistingSystemManaged<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_BeginSimECBSystem.CreateCommandBuffer();
        var PosTemp = new float3(0, 0, 0);

        Entities
        .WithAll<RemoveBlock>()
        .ForEach((Entity entity, in LocalTransform blockPos) =>
        {
            //Offset position y+1
            PosTemp = new float3(blockPos.Position.x, blockPos.Position.y + 1, blockPos.Position.z);
            ecb.DestroyEntity(entity);
        })
        .WithoutBurst()
        .Run();
        //.Schedule();

        if (!PosTemp.Equals(new float3(0, 0, 0)))
        {

            //Check if needs to remove plant
            Entities
            .WithAll<PlantBlock>()
            .ForEach((Entity entity, in LocalTransform blockPos) =>
            {
                if (PosTemp.Equals(blockPos.Position))
                {
                    //Destroy Plant block
                    ecb.DestroyEntity(entity);
                }

            })
            .WithoutBurst()
            .Run();

        }
        //ecb.Dispose();
        m_BeginSimECBSystem.AddJobHandleForProducer(Dependency);
    }
}