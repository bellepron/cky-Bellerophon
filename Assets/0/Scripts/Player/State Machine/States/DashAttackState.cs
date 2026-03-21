using UnityEngine;

namespace Bellepron.Player
{
    public class DashAttackState : AttackState
    {
        public override void Enter()
        {
            animStartedTime = Time.time;
            _attackController.DashAttack();
        }

        public override void Tick(float deltaTime)
        {
            if (Time.time <= animStartedTime + _animatorController.DashAttackFadeDuration) return;

            if (!_dashController.isDashing && _animatorController.GetNormalizedTime() >= 1f)
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