using UnityEngine.AI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyMovementController : IInitializable
    {
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly EnemySettings _settings;
        [Inject] readonly NavMeshAgent _navMeshAgent;

        public void Initialize()
        {
            SetSpeed(_settings.movementSpeed);
        }

        public void SetSpeed(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        public bool IsMoving => _navMeshAgent.velocity.sqrMagnitude > 0.01f;
        public bool HasReachedDestination => !_navMeshAgent.pathPending &&
                                              _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;

        public bool IsDestinationReachable(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            if (!_navMeshAgent.isOnNavMesh) return false;

            _navMeshAgent.CalculatePath(destination, path);
            return path.status == NavMeshPathStatus.PathComplete
            // || path.status == NavMeshPathStatus.PathPartial
             ;
        }

        public void MoveTo(Vector3 destination)
        {
            if (!_navMeshAgent.isOnNavMesh) return;
            if (!IsDestinationReachable(destination)) return;

            _navMeshAgent.SetDestination(destination);
        }

        public void StopMoving()
        {
            if (_navMeshAgent.isOnNavMesh)
                _navMeshAgent.ResetPath();
        }
    }
}