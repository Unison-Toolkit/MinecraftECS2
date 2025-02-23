using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;

public class WorldGeneratorMeshes : MonoBehaviour
{
    public Mesh surfaceMesh;
    public Mesh defaultMesh;
    public Mesh plantMesh;
    public Material surfaceMaterial;
    public Material defaultMaterial;
    public Material defaultMaterialAlpha;
    public Material plantMaterial;

    public int m_chunkSize;

    Mesh[] meshes = new Mesh[3];

    int m_w;
    int m_h;
    //int m_y = 15;

    // Example Burst job that creates many entities
    [GenerateTestsForBurstCompatibility]
    public struct SpawnJob : IJobParallelFor
    {
        public Entity Prototype;

        public int w;
        public int h;

        public EntityCommandBuffer.ParallelWriter Ecb;
        int meshNumber;
        int matNumber;
        float m_mat;

        int ranDice;
        bool spawnFlag;
        float3 posTemp;

        public void Execute(int index)
        {

            //Each x,y coordinate has 15 blocks downwards
            for (int i = 0; i < 15; i++)
            {
                spawnFlag = true;
                bool b_rotate = false;
                //Dealing with bricks on the surface
                if (i == 0)
                {
                    spawnFlag = false;

                    //If the height is 0, it means that this is on the surface, and random numbers are generated (no blocks, grass, flowers, trees, clouds) 
                    var Random = Unity.Mathematics.Random.CreateFromIndex((uint)index);
                    ranDice = Random.NextInt(1, 201);

                    if (ranDice <= 20)
                    {
                        //grass 88-95
                        meshNumber = 2;
                        matNumber = 3;
                        m_mat = Random.NextInt(88, 95);
                        spawnFlag = true;
                        b_rotate = true;
                    }
                    if (ranDice == 198)
                    {
                        //cloud
                        meshNumber = 1;
                        matNumber = 1;
                        m_mat = 66;
                        TreeNCloudGenerator(index, 0);
                    }
                    if (ranDice == 200)
                    {
                        //flower
                        meshNumber = 2;
                        matNumber = 3;
                        m_mat = Random.NextInt(12, 13);
                        spawnFlag = true;
                        b_rotate = true;
                    }
                    if (ranDice == 199)
                    {
                        //tree
                        TreeNCloudGenerator(index, 1);
                    }
                }
                //Treat the turf
                else if (i == 1)
                {
                    meshNumber = 0;
                    matNumber = 0;
                }
                //Underground blocks (i > 1)
                else
                {
                    meshNumber = 1;
                    matNumber = 1;

                    //switch materials that underground
                    switch (i)
                    {
                        //If the height is 1, it means that this is the surface layer, stuffed into the surface block
                        //case 0:
                        //    m_mat = 0;

                        //break;
                        //If the height is 2, 3, 4 stuffed into the dirt block
                        case 2:
                        case 3:
                        case 4:
                            //Dirt
                            m_mat = 2f;
                            break;
                        //If the height is 5,6 stuffed into stone squares
                        case 5:
                        case 6:
                            //stone block
                            m_mat = 1f;
                            break;
                        //If the height is 7,8 stuffed into the pebbles squares
                        case 7:
                        case 8:
                            m_mat = 16f;
                            break;
                        //number 32 block
                        default:
                            m_mat = 32f;
                            break;
                    }
                }

                float3 blockPos = ComputeTransform(index, i);
                if (blockPos.y > -16 && spawnFlag)
                {
                    BlockSpawner(index, blockPos, b_rotate);
                }
            }
        }

        public float3 ComputeTransform(int index, int i)
        {
            int y = index / w;
            int x = index % h;
            float2 posTemp2 = new float2(x - (float)w * 0.5f, y - (float)h * 0.5f);
            //Prelin noise for hight value
            int yValue = (int)(noise.cnoise(posTemp2 / 10) * 4) - 5 - i;
            float3 pos = new float3(x - (float)w * 0.5f, yValue, y - (float)h * 0.5f);

            return pos;
            //return float4x4.Translate(pos);
        }

        //Tree cloud generator
        public void TreeNCloudGenerator(int index, int plantType)
        {
            //location
            float3 blockPos = ComputeTransform(index, 0);

            //cloud
            if (plantType == 0)
            {

                //Add 15 to Y from this coordinate to randomly generate cloud blocks of 4-7 size, and the resulting Entity specifies the model material
                var Random = Unity.Mathematics.Random.CreateFromIndex((uint)index);
                int ranBlock = Random.NextInt(4, 7);

                for (int i = 0; i < ranBlock; i++)
                {
                    for (int j = 0; j < ranBlock; j++)
                    {
                        posTemp = new float3((int)blockPos.x + i, (int)blockPos.y + 15, (int)blockPos.z + j);
                        BlockSpawner(index, posTemp, false);
                    }
                }
            }
            else if (plantType == 1)
            {
                int3 blockTemp = new int3((int3)blockPos);
                //Trees, with the current coordinates xpos, ypos, zpos as roots, grow other trunks and leaves up
                for (int i = 0; i < 7; i++)
                {
                    //Leaves should be placed at the top of the height
                    if (i == 6)
                    {
                        //leaves
                        meshNumber = 1;
                        matNumber = 2;
                        m_mat = 52f;
                    }
                    else
                    {
                        //wood
                        meshNumber = 0;
                        matNumber = 0;
                        m_mat = 0.67f;
                    }

                    posTemp = new float3((int)blockPos.x, (int)blockPos.y + i, (int)blockPos.z);
                    BlockSpawner(index, posTemp, false);

                    //If the height is between 3-6, plant additional leaves
                    if (i >= 3 && i <= 6)
                    {
                        for (int j = blockTemp.x - 1; j <= blockTemp.x + 1; j++)
                        {
                            for (int k = blockTemp.z - 1; k <= blockTemp.z + 1; k++)
                            {
                                if (k != blockTemp.z || j != blockTemp.x)
                                {
                                    //leaves
                                    meshNumber = 1;
                                    matNumber = 2;
                                    m_mat = 52f;

                                    posTemp = new float3((int)j, blockPos.y + (int)i, (int)k);
                                    BlockSpawner(index, posTemp, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void BlockSpawner(int index, float3 blockPos, bool m_rotate)
        {
            float3 r;
            //rotate 45c for plants
            if (m_rotate)
            {
                r = new float3(0, 45, 0);
            }
            else
            {
                r = new float3(0, 0, 0);
            }

            quaternion R;
            float3 s = new float3(1, 1, 1);
            R = quaternion.EulerXYZ(r);


            //var e = Ecb.Instantiate(index, BlockType);
            var e = Ecb.Instantiate(index, Prototype);

            // Prototype has all correct components up front, can use SetComponent to
            // set values unique to the newly created entity, such as the transform.
            Ecb.SetComponent(index, e, MaterialMeshInfo.FromRenderMeshArrayIndices(matNumber, meshNumber));
            //Ecb.SetComponent(index, e, new LocalToWorld {Value = float4x4.Translate(blockPos)});
            Ecb.SetComponent(index, e, new LocalToWorld { Value = float4x4.TRS(blockPos, R, s) });

            //Set the Shader Graph corresponding material number
            Ecb.SetComponent(index, e, new Block { blockID = m_mat });

        }
    }

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var entityManager = world.EntityManager;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        //a chunk size = 10 x 10 x 15
        m_w = m_chunkSize * 10;
        m_h = m_w;

        //mesh
        meshes[0] = surfaceMesh;
        meshes[1] = defaultMesh;
        meshes[2] = plantMesh;

        //int objCount = m_w * m_h;

        var matList = new List<Material>();
        matList.Add(surfaceMaterial);
        matList.Add(defaultMaterial);
        matList.Add(defaultMaterialAlpha);
        matList.Add(plantMaterial);


        // Create a RenderMeshDescription using the convenience constructor
        // with named parameters.
        var desc = new RenderMeshDescription(
            shadowCastingMode: ShadowCastingMode.Off,
            receiveShadows: false);

        var renderMeshArray = new RenderMeshArray(matList.ToArray(), meshes);

        // Create empty base entity
        var prototype = entityManager.CreateEntity();

        // Call AddComponents to populate base entity with the components required
        // by Hybrid Renderer
        RenderMeshUtility.AddComponents(
            prototype,
            entityManager,
            desc,
            renderMeshArray,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

        entityManager.AddComponentData(prototype, new LocalToWorld());
        entityManager.AddComponentData(prototype, new Block());

        //Add a BoxCollider
        quaternion R;
        float3 c = new float3(0, 0, 0);
        float3 s = new float3(1, 1, 1);
        R = quaternion.EulerXYZ(new float3(0, 0, 0));
        var boxGeometry = new BoxGeometry();
        boxGeometry.Center = c;
        boxGeometry.Size = s;
        boxGeometry.Orientation = R;
        BlobAssetReference<Unity.Physics.Collider> boxCollider = Unity.Physics.BoxCollider.Create(boxGeometry, CollisionFilter.Default);
        entityManager.AddComponentData(prototype, new PhysicsCollider { Value = boxCollider });

        // Spawn most of the entities in a Burst job by cloning a pre-created prototype entity,
        // which can be either a Prefab or an entity created at run time like in this sample.
        // This is the fastest and most efficient way to create entities at run time.
        var spawnJob = new SpawnJob
        {
            Prototype = prototype,
            Ecb = ecb.AsParallelWriter(),
            w = m_w,
            h = m_h
        };

        var spawnHandle = spawnJob.Schedule(m_h * m_w, 128);
        spawnHandle.Complete();
        ecb.Playback(entityManager);
        ecb.Dispose();
        entityManager.DestroyEntity(prototype);
    }
}