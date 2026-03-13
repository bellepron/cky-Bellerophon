using Zenject;

namespace Bellepron.Player
{
    public class MovementState : PlayerBaseState
    {
        [Inject] readonly PlayerInputHandler _playerInputHandler;
        [Inject] readonly PlayerMovementController _playerMovementController;
        [Inject] readonly PlayerStateMachine _stateMachine;
        [Inject] readonly PlayerAnimatorController _playerAnimatorController;

        public override void Enter()
        {
            _playerAnimatorController.CrossMovement();
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime)
        {
            _playerMovementController
                .ApplyMovement();
        }

        public override void Tick(float deltaTime)
        {
            if (_playerInputHandler.Get_DashPressed)
            {
                _stateMachine.ChangeState(States.Dash);
            }
            else if (_playerInputHandler.Get_AttackPressed)
            {
                _stateMachine.ChangeState(States.Attack);
            }
        }
    }
}