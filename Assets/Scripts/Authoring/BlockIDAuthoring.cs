using Unity.Entities;
using UnityEngine;

class BlockIDAuthoring : MonoBehaviour
{
    public float blockID;
}

class BlockIDBaker : Baker<BlockIDAuthoring>
{
    public override void Bake(BlockIDAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new BlockID
        {
            blockID = authoring.blockID
        });
    }
}