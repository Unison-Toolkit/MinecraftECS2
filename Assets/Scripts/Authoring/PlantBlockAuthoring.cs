using Unity.Entities;
using UnityEngine;

class PlantBlockAuthoring : MonoBehaviour
{

}

class PlantBlockBaker : Baker<PlantBlockAuthoring>
{
    public override void Bake(PlantBlockAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlantBlock { });
    }
}