using UnityEngine;

namespace Bellepron.Player
{
    public class DashAttackState : AttackState
    {
        public override void Enter()
        {
            animStartedTime = Time.time;
            _playerAttackController.DashAttack();
        }

        public override void Tick(float deltaTime)
        {
            if (_playerInputHandler.Get_DashPressed && _playerDashController.CanDash())
            {
                _stateMachine.ChangeState(State.Dash);
            }

            if (Time.time <= animStartedTime + _playerAnimatorController.DashAttackFadeDuration) return;

            if (!_playerDashController.isDashing && _playerAnimatorController.GetCurrentAttackNormalizedTime() >= 1f)
            {
                _stateMachine.ChangeState(State.Movement);
            }
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime) { }
    }
}