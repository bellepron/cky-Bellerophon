using Bellepron.Player;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class ChaseState : EnemyBaseState
    {
        [Inject] readonly EnemyStateMachine _stateMachine;
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly PlayerHolder _playerHolder;

        Transform _playerTransform;

        public override void Enter()
        {
            _playerTransform = _playerHolder.PlayerFacade.transform;
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime)
        {

        }

        public override void Tick(float deltaTime)
        {
            if (_facade.IsDestinationReachable(_playerTransform.position))
            {
                _facade.MoveTo(_playerTransform.position);
            }
        }
    }
}