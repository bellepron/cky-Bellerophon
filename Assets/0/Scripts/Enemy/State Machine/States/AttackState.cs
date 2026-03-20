using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class AttackState : EnemyBaseState
    {
        [Inject] readonly EnemyStateMachine _stateMachine;
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly EnemyMovementController _movementController;
        [Inject] readonly EnemyAttackController _attackController;

        public override void Enter()
        {
            _movementController.StopMoving();
            _attackController.AttakTEMP(1.5f);
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime)
        {

        }

        public override void Tick(float deltaTime)
        {
            if (!_attackController.IsAttacking)
            {
                _stateMachine.ChangeState(State.Idle);
            }
        }
    }
}