using Bellepron.Player;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class IdleState : EnemyBaseState
    {
        [Inject] readonly EnemyStateMachine _stateMachine;
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly PlayerHolder _playerHolder;

        const float CHECK_INTERVAL = 2f;
        const float DETECTION_RANGE = 5f;

        float _timer;
        bool _shouldCheck;

        public override void Enter()
        {
            _timer = 0.0f;
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (!_shouldCheck) return;
            _shouldCheck = false;

            // if (CanSeePlayer())
            // {
            //     _stateMachine.ChangeState(State.Chase);
            // }

            _stateMachine.ChangeState(State.Chase);
        }

        public override void Tick(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= CHECK_INTERVAL)
            {
                _timer = 0f;
                _shouldCheck = true;
            }
        }

        // bool CanSeePlayer()
        // {
        //     var player = _playerHolder.PlayerFacade;
        //     if (player == null) return false;

        //     Vector3 toPlayer = player.Position - _facade.Position;
        //     if (toPlayer.sqrMagnitude > DETECTION_RANGE * DETECTION_RANGE) return false;

        //     if (Physics.Raycast(_facade.Position + Vector3.up, toPlayer.normalized, out RaycastHit hit, DETECTION_RANGE))
        //         return hit.collider.CompareTag("Player");

        //     return false;
        // }
    }
}