using Bellepron.StateMachine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerStateMachine : BaseStateMachine
    {
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly MovementState _movementState;
        [Inject] readonly DashState _dashState;
        [Inject] readonly AttackState _attackState;
        [Inject] readonly SpecialState _specialState;
        [Inject] readonly CastState _castState;
        [Inject] readonly DashAttackState _dashAttackState;

        public override void Initialize()
        {
            SwitchState(_movementState);
        }

        public void ChangeState(State newState)
        {
            if (_playerFacade.State == newState) return;

            _playerFacade.State = newState;

            SwitchState(EnumToState(newState));
        }

        PlayerBaseState EnumToState(State newState)
        {
            return newState switch
            {
                State.Movement => _movementState,
                State.Dash => _dashState,
                State.Attack => _attackState,
                State.Special => _specialState,
                State.Cast => _castState,
                State.DashAttack => _dashAttackState,
                _ => _movementState
            };
        }
    }
}