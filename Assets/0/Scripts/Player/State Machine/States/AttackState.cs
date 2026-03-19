using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class AttackState : PlayerBaseState
    {
        [Inject] protected readonly PlayerStateMachine _stateMachine;
        [Inject] protected readonly PlayerInputHandler _playerInputHandler;
        [Inject] protected readonly PlayerAnimatorController _playerAnimatorController;
        [Inject] protected readonly PlayerAttackController _playerAttackController;
        [Inject] protected readonly PlayerDashController _playerDashController;

        protected float animStartedTime;
        int _attackStep = 1;
        bool _comboQueued;

        public override void Enter()
        {
            animStartedTime = Time.time;
            _attackStep = 1;
            _comboQueued = false;
            _playerAttackController.Attack(_attackStep);
        }

        public override void Tick(float deltaTime)
        {
            if (_playerInputHandler.Get_DashPressed && _playerDashController.CanDash())
            {
                _stateMachine.ChangeState(State.Dash);
            }

            if (Time.time <= animStartedTime + _playerAnimatorController.AttackFadeDuration) return;

            if (!_playerAnimatorController.IsPlayingAttack())
                return;

            if (_playerInputHandler.Get_AttackPressed)
            {
                if (_attackStep < 3)
                    _comboQueued = true;
            }

            if (_playerAnimatorController.GetCurrentAttackNormalizedTime() >= 1f)
            {
                if (_comboQueued)
                {
                    _attackStep++;
                    _comboQueued = false;

                    _playerAttackController.Attack(_attackStep);

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