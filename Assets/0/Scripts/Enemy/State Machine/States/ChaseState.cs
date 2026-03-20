using Zenject;

namespace Bellepron.Enemy
{
    public class ChaseState : EnemyBaseState
    {
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly EnemyStateMachine _stateMachine;
        [Inject] readonly EnemyMovementController _movementController;
        [Inject] readonly EnemyDetectionController _detectionController;
        [Inject] readonly EnemyAttackController _attackController;

        public override void Enter() { }
        public override void Exit() { }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (!_detectionController.HasTarget)
                _stateMachine.ChangeState(State.Idle);
        }

        public override void Tick(float deltaTime)
        {
            var target = _detectionController.CurrentTarget;
            if (target == null) return;

            if (!_movementController.IsDestinationReachable(target.position)) return;

            if (_attackController.IsInRange(target.position))
                _stateMachine.ChangeState(State.Attack);
            else
                _movementController.MoveTo(target.position);
        }
    }
}