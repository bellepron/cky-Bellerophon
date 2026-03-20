using Zenject;

namespace Bellepron.Enemy
{
    public class IdleState : EnemyBaseState
    {
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly EnemyStateMachine _stateMachine;
        [Inject] readonly EnemyMovementController _movementController;
        [Inject] readonly EnemyDetectionController _detectionController;

        public override void Enter()
        {
            _movementController.StopMoving();
            _detectionController.ResetDetection();
        }

        public override void Exit() { }

        public override void Tick(float deltaTime) { }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (_detectionController.HasLineOfSight)
                _stateMachine.ChangeState(State.Chase);
        }
    }
}