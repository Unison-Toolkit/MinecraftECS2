using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Unity.Physics.Extensions
{
    class PickaxeController : MonoBehaviour
    {
        protected RaycastInput RaycastInput;
        protected NativeList<RaycastHit> RaycastHits;

        public static int SelectedIndex = 1;

        [SerializeField] Vector3 _raycastDirection = new(0, 0, 1);
        [SerializeField] float _raycastDistance = 10.0f;
        [SerializeField] AudioClip _createBlockSound;
        [SerializeField] AudioClip _removeBlockSound;
        [SerializeField] ParticleSystem _digEffect;

        AudioSource _audioSource;

        void Start()
        {
            _audioSource = this.GetComponent<AudioSource>();
            Cursor.lockState = CursorLockMode.Locked;
            RaycastHits = new NativeList<RaycastHit>(Allocator.Persistent);
        }

        void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                SelectedIndex++;

                if (SelectedIndex > 7)
                {
                    SelectedIndex = 1;
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                SelectedIndex--;

                if (SelectedIndex < 1)
                {
                    SelectedIndex = 7;
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                CreateOrRemoveBlock(false);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                CreateOrRemoveBlock(true);
            }
        }

        void OnDestroy()
        {
            if (RaycastHits.IsCreated)
            {
                RaycastHits.Dispose();
            }
        }

        void CreateOrRemoveBlock(bool creating)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
            EntityQuery singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
            PhysicsWorld phyworld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
            float3 origin = transform.position;
            float3 direction = (transform.rotation * _raycastDirection) * _raycastDistance;
            RaycastHits.Clear();
            singletonQuery.Dispose();

            RaycastInput = new RaycastInput
            {
                Start = origin,
                End = origin + direction,
                Filter = CollisionFilter.Default
            };

            if (!phyworld.CastRay(RaycastInput, out RaycastHit hit))
                return;

            var world = World.DefaultGameObjectInjectionWorld;
            var entityManager = world.EntityManager;

            if (!entityManager.HasComponent<Block>(hit.Entity))
                return;

            if (creating)
            {
                // TODO: Move logic elsewhere.
                int blockID = 1;
                float mat = 0f;

                switch (SelectedIndex)
                {
                    case 1: // stone
                        blockID = 1;
                        mat = 1;
                        break;
                    case 2: // plank
                        blockID = 1;
                        mat = 4;
                        break;
                    case 3: // glass
                        blockID = 2;
                        mat = 49;
                        break;
                    case 4: // wood
                        blockID = 0;
                        mat = 0.67f;
                        break;
                    case 5: // cobble
                        blockID = 1;
                        mat = 16;
                        break;
                    case 6: // TNT
                        blockID = 0;
                        mat = 0.33f;
                        break;
                    case 7: // brick
                        blockID = 1;
                        mat = 7;
                        break;
                }

                _audioSource.PlayOneShot(_createBlockSound);
                var newBlock = entityManager.CreateEntity();
                var blockPos = entityManager.GetComponentData<LocalToWorld>(hit.Entity);
                var newPosition = hit.SurfaceNormal + blockPos.Position;
                entityManager.AddComponentData(newBlock, new AddBlock { spawnPos = newPosition, spawnType = blockID, spawnMat = mat });
            }
            else // Remove a block.
            {
                _audioSource.PlayOneShot(_removeBlockSound);
                _digEffect.transform.position = hit.Position;
                _digEffect.Play();
                entityManager.AddComponentData(hit.Entity, new RemoveBlock());
            }
        }
    }
}
