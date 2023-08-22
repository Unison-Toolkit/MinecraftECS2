using Unity.Entities;
using UnityEngine;

class BlockAuthoring : MonoBehaviour
{
    public float blockID;
}

class BlockBaker : Baker<BlockAuthoring>
{
    public override void Bake(BlockAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Block
        {
            blockID = authoring.blockID
        });
    }
}