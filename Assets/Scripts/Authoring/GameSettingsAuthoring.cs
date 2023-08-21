using Unity.Entities;
using UnityEngine;

class GameSettingsAuthoring : MonoBehaviour
{
    public int chunkSize;
}

class GameSettingsBaker : Baker<GameSettingsAuthoring>
{
    public override void Bake(GameSettingsAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new GameSettings
        {
            chunkSize = authoring.chunkSize
        });
    }
}