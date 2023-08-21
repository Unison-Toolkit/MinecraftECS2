using Unity.Entities;
using UnityEngine;

class BlockTypeAuthoring : MonoBehaviour
{
    public GameObject sixSidedPrefab;
    public GameObject defaultPrefab;
    public GameObject defaultAlphaPrefab;
    public GameObject plantPrefab;
}

class BlockTypeBaker : Baker<BlockTypeAuthoring>
{
    public override void Bake(BlockTypeAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new BlockType
        {
            sixSidedPrefab = GetEntity(authoring.sixSidedPrefab, TransformUsageFlags.Dynamic),
            defaultPrefab = GetEntity(authoring.defaultPrefab, TransformUsageFlags.Dynamic),
            defaultAlphaPrefab = GetEntity(authoring.defaultAlphaPrefab, TransformUsageFlags.Dynamic),
            plantPrefab = GetEntity(authoring.plantPrefab, TransformUsageFlags.Dynamic)
        });
    }
}