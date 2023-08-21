using System;
using Unity.Entities;
using Unity.Rendering;

[Serializable]
[MaterialProperty("_BlockID")]
public struct BlockID : IComponentData
{
    public float blockID;
}