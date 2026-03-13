using UnityEngine.AI;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Enemy
{
    public class EnemyFacade : MonoBehaviour, IInitializable, IDamageable, IPoolable<Vector3, IMemoryPool>, IDisposable
    {
        [field: SerializeField] public EnemyTypes EnemyType { get; private set; }
        [Inject] EnemyHealthController _enemyHealthController;

        [Space(10)]
        [SerializeField] Animator animator;
        [SerializeField] CapsuleCollider capsuleCollider;
        [SerializeField] Rigidbody rb;
        [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] NavMeshAgent navMeshAgent;

        public Animator Animator => animator;
        public SkinnedMeshRenderer SkinnedMeshRenderer => skinnedMeshRenderer;
        public NavMeshAgent NavMeshAgent => navMeshAgent;

        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Position => rb.position;
        public Quaternion Rotation => rb.rotation;
        public Vector3 Velocity => rb.linearVelocity;

        IMemoryPool _pool;

        public void Initialize()
        {

        }

        public void TakeDamage(int damage)
        {
            _enemyHealthController.ChangeHealth(-damage);
        }

        public void OnSpawned(Vector3 position, IMemoryPool pool)
        {

            _pool = pool;

            transform.position = position;
            gameObject.SetActive(true);

            //Debug.Log($"[EnemyFacade] {EnemyType} spawned at {position}");
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            _pool = null;

            //Debug.Log($"[EnemyFacade] {EnemyType} despawned");
        }

        public void Despawn()
        {
            _pool?.Despawn(this);
        }

        public void Dispose()
        {
            _pool?.Despawn(this);
        }

        void OnValidate()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!capsuleCollider) capsuleCollider = GetComponent<CapsuleCollider>();
        }

        // Inner Factory — Zenject SubContainer pattern
        // Her enemy kendi baðýmlýlýk scope'unda üretilir.
        public class Factory : PlaceholderFactory<Vector3, EnemyFacade> { }
    }
}