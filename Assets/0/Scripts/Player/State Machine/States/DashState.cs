using Zenject;

namespace Bellepron.Player
{
    public class DashState : PlayerBaseState
    {
        [Inject] readonly PlayerStateMachine _stateMachine;
        [Inject] readonly PlayerInputHandler _playerInputHandler;
        [Inject] readonly PlayerDashController _playerDashController;

        public override void Enter()
        {
            _playerDashController.TryDash();
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime)
        {

        }

        public override void Tick(float deltaTime)
        {
            if (_playerInputHandler.Get_AttackPressed)
            {
                _stateMachine.ChangeState(State.DashAttack);
            }

            if (!_playerDashController.isDashing)
            {
                _stateMachine.ChangeState(State.Movement);
            }
        }
    }
}