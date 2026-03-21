using UnityEngine.AI;
using Bellepron.Cast;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyFacade : MonoBehaviour, IDamageable, IPoolable<Vector3, IMemoryPool>
    {
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public State State { get; set; }
        [Inject(Id = "EnemiesParent")] private Transform _enemiesParent;
        [Inject] readonly EnemyHealthController _enemyHealthController;
        // [Inject] readonly Animator _animator;
        [Inject] readonly CapsuleCollider _capsuleCollider;
        [Inject] readonly Rigidbody _rb;
        // [Inject] readonly SkinnedMeshRenderer _skinnedMeshRenderer;
        [Inject] readonly NavMeshAgent _navMeshAgent;

        public Transform Transform => transform;
        public bool IsAlive => _enemyHealthController.IsAlive;
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Position => _rb.position;
        public Quaternion Rotation => _rb.rotation;
        public Vector3 Velocity => _rb.linearVelocity;

        IMemoryPool _pool;

        public void TakeDamage(int damage, GameObject instigator)
        {
            _enemyHealthController.ChangeHealth(-damage, instigator);
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
            transform.parent = _enemiesParent;
        }

        // Inner Factory Zenject SubContainer pattern
        // Her enemy kendi bagimlilik scope'unda uretilir.
        public class Factory : PlaceholderFactory<Vector3, EnemyFacade> { }
    }
}