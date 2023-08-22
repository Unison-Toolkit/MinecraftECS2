using System;
using Unity.Entities;
using Unity.Rendering;

[Serializable]
[MaterialProperty("_BlockID")]
public struct Block : IComponentData
{
    public float blockID;
}