using Unity.Entities;
using UnityEngine;

struct PlayerTag : IComponentData
{

}

class PlayerAuthoring : MonoBehaviour
{

}

class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent<PlayerTag>(entity);
    }
}