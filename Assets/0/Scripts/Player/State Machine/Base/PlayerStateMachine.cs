using Bellepron.StateMachine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerStateMachine : BaseStateMachine
    {
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly MovementState _movementState;
        [Inject] readonly AttackState _attackState;
        [Inject] readonly DashState _dashState;
        [Inject] readonly DashAttackState _dashAttackState;

        public override void Initialize()
        {
            SwitchState(_movementState);
        }

        public void ChangeState(States newState)
        {
            if (_playerFacade.State == newState) return;

            _playerFacade.State = newState;

            SwitchState(EnumToState(newState));
        }

        PlayerBaseState EnumToState(States newState)
        {
            return newState switch
            {
                States.Movement => _movementState,
                States.Attack => _attackState,
                States.Dash => _dashState,
                States.DashAttack => _dashAttackState,
                _ => _movementState
            };
        }

        protected override void FixedTick()
        {
            base.FixedTick();
        }
    }
}