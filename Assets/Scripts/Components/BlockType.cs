using Unity.Entities;

public struct BlockType : IComponentData
{
    public Entity sixSidedPrefab;
    public Entity defaultPrefab;
    public Entity defaultAlphaPrefab;
    public Entity plantPrefab;
}