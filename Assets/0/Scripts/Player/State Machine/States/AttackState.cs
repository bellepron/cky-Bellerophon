using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class AttackState : PlayerBaseState
    {
        [Inject] protected readonly PlayerStateMachine _stateMachine;
        [Inject] protected readonly PlayerInputHandler _inputHandler;
        [Inject] protected readonly PlayerAnimatorController _animatorController;
        [Inject] protected readonly PlayerAttackController _attackController;
        [Inject] protected readonly PlayerDashController _dashController;

        protected float animStartedTime;
        int _attackStep = 1;
        bool _comboQueued;

        public override void Enter()
        {
            animStartedTime = Time.time;
            _attackStep = 1;
            _comboQueued = false;
            _attackController.Attack(_attackStep);
        }

        public override void Tick(float deltaTime)
        {
            if (_inputHandler.Get_DashPressed && _dashController.CanDash())
            {
                _stateMachine.ChangeState(State.Dash);
            }

            if (Time.time <= animStartedTime + _animatorController.AttackFadeDuration) return;

            if (!_animatorController.IsPlayingAttack()) return;

            if (_inputHandler.Get_AttackPressed)
            {
                if (_attackStep < 3)
                    _comboQueued = true;
            }

            if (_animatorController.GetNormalizedTime() >= 1f)
            {
                if (_comboQueued)
                {
                    _attackStep++;
                    _comboQueued = false;

                    _attackController.Attack(_attackStep);

                    animStartedTime = Time.time;

                    return;
                }
                else
                {
                    _stateMachine.ChangeState(State.Movement);
                }
            }
        }

        public override void Exit()
        {
            _attackStep = 1;
            _comboQueued = false;
        }

        public override void FixedTick(float fixedDeltaTime) { }
    }
}