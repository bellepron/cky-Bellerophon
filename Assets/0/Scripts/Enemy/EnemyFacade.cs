using UnityEngine.AI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyFacade : MonoBehaviour, IDamageable, IPoolable<Vector3, IMemoryPool>
    {
        [field: SerializeField] public EnemyType EnemyType { get; private set; }
        [field: SerializeField] public State State { get; set; }
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

        public void TakeDamage(int damage)
        {
            _enemyHealthController.ChangeHealth(-damage);
        }

        public bool IsMoving => navMeshAgent.velocity.sqrMagnitude > 0.01f;
        public bool HasReachedDestination => !navMeshAgent.pathPending &&
                                              navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;

        public bool IsDestinationReachable(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            if (!navMeshAgent.isOnNavMesh) return false;

            navMeshAgent.CalculatePath(destination, path);
            return path.status == NavMeshPathStatus.PathComplete
            // || path.status == NavMeshPathStatus.PathPartial
             ;
        }

        public void MoveTo(Vector3 destination)
        {
            if (!navMeshAgent.isOnNavMesh) return;
            if (!IsDestinationReachable(destination)) return;

            navMeshAgent.SetDestination(destination);
        }

        public void StopMoving()
        {
            if (navMeshAgent.isOnNavMesh)
                navMeshAgent.ResetPath();
        }

        public void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
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

        void OnValidate()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!capsuleCollider) capsuleCollider = GetComponent<CapsuleCollider>();
        }

        // Inner Factory Zenject SubContainer pattern
        // Her enemy kendi bagimlilik scope'unda uretilir.
        public class Factory : PlaceholderFactory<Vector3, EnemyFacade> { }
    }
}