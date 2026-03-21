using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class SpecialState : PlayerBaseState
    {
        [Inject] readonly PlayerInputHandler _inputHandler;
        [Inject] readonly PlayerStateMachine _stateMachine;
        [Inject] readonly PlayerAttackController _attackController;
        [Inject] protected readonly PlayerAnimatorController _animatorController;

        public override void Enter()
        {
            _attackController.Special();
        }

        public override void Exit()
        {
        }

        public override void FixedTick(float fixedDeltaTime)
        {
        }

        public override void Tick(float deltaTime)
        {
            if (_inputHandler.Get_DashPressed)
            {
                _stateMachine.ChangeState(State.Dash); // TODO: Basýlacak ama bozmayacak.
            }
            else if (_inputHandler.Get_AttackPressed)
            {
                _stateMachine.ChangeState(State.Attack);
            }

            if (!_animatorController.IsPlayingSpecial()) return;

            if (_animatorController.GetNormalizedTime() >= 1f)
            {
                _stateMachine.ChangeState(State.Movement);
            }
        }
    }
}