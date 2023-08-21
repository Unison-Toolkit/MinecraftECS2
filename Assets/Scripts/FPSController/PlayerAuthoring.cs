using Unity.Entities;
using UnityEngine;

// An empty "tag" component used to identify the player.
struct PlayerEntity : IComponentData
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
        AddComponent<PlayerEntity>(entity);
    }
}