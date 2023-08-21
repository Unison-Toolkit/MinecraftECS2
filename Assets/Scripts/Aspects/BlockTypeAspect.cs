using Unity.Entities;

readonly partial struct BlockTypeAspect : IAspect
{
    readonly RefRO<BlockType> _blockType;

    public Entity SixSidedPrefab => _blockType.ValueRO.sixSidedPrefab;
    public Entity DefaultPrefab => _blockType.ValueRO.defaultPrefab;
    public Entity DefaultAlphaPrefab => _blockType.ValueRO.defaultAlphaPrefab;
    public Entity PlantPrefab => _blockType.ValueRO.plantPrefab;
}

readonly partial struct GameSettingsAspect : IAspect
{
    readonly RefRO<GameSettings> m_GameSettings;

    public int ChunkSize => m_GameSettings.ValueRO.chunkSize;
}