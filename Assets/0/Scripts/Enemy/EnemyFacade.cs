using System.Collections;
using UnityEngine.AI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyFacade : MonoBehaviour, IDamageable, IPoolable<Vector3, IMemoryPool>
    {
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public State State { get; set; }
        [Inject] private EnemiesParent _enemiesParent;
        [Inject] readonly EnemyHealthController _healthController;
        // [Inject] readonly Animator _animator;
        [Inject] readonly CapsuleCollider _capsuleCollider;
        [Inject] readonly Rigidbody _rb;
        // [Inject] readonly SkinnedMeshRenderer _skinnedMeshRenderer;
        [Inject] readonly NavMeshAgent _navMeshAgent;

        public Transform Transform => transform;
        public bool IsAlive => _healthController.IsAlive;
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Position => _rb.position;
        public Quaternion Rotation => _rb.rotation;
        public Vector3 Velocity => _rb.linearVelocity;

        IMemoryPool _pool;

        public void TakeDamage(int damage, GameObject instigator)
        {
            _healthController.ChangeHealth(-damage, instigator);
        }

        public void OnSpawned(Vector3 position, IMemoryPool pool)
        {
            _pool = pool;

            transform.position = position;
            gameObject.SetActive(true);
            _healthController.OnSpawned();
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
            StartCoroutine(DelayedDespawn());
        }
        IEnumerator DelayedDespawn()
        {
            _healthController.SetCurrentHealth(0);

            yield return null; // For returning to the pool; CastProjectileEcho
            // yield return new WaitForSeconds(0.5f);
            _pool?.Despawn(this);
            transform.parent = _enemiesParent.Transform;
        }

        public class Factory : PlaceholderFactory<Vector3, EnemyFacade> { }
    }
}