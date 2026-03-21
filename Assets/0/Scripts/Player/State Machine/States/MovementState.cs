using Zenject;

namespace Bellepron.Player
{
    public class MovementState : PlayerBaseState
    {
        [Inject] readonly PlayerInputHandler _inputHandler;
        [Inject] readonly PlayerMovementController _movementController;
        [Inject] readonly PlayerStateMachine _stateMachine;
        [Inject] readonly PlayerAnimatorController _animatorController;
        [Inject] readonly PlayerInteractController _interactController;
        [Inject] readonly PlayerDashController _dashController;

        public override void Enter()
        {
            _animatorController.CrossMovement();
        }

        public override void Exit()
        {
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            _movementController.ApplyMovement();
        }

        public override void Tick(float deltaTime)
        {
            if (_inputHandler.Get_DashPressed && _dashController.CanDash())
            {
                _stateMachine.ChangeState(State.Dash);
            }
            else if (_inputHandler.Get_SpecialPressed)
            {
                _stateMachine.ChangeState(State.Special);
            }
            else if (_inputHandler.Get_AttackPressed)
            {
                _stateMachine.ChangeState(State.Attack);
            }
            else if (_inputHandler.Get_CastPressed)
            {
                _stateMachine.ChangeState(State.Cast);
            }
            else if (_inputHandler.Get_InteractPressed)
            {
                _interactController.TryInteract();
            }
        }
    }
}