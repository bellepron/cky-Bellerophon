using Zenject;

namespace Bellepron.Player
{
    public class DashState : PlayerBaseState
    {
        [Inject] readonly PlayerStateMachine _stateMachine;
        [Inject] readonly PlayerInputHandler _inputHandler;
        [Inject] readonly PlayerDashController _dashController;

        public override void Enter()
        {
            _dashController.TryDash();
        }

        public override void Exit()
        {

        }

        public override void FixedTick(float fixedDeltaTime)
        {

        }

        public override void Tick(float deltaTime)
        {
            if (_inputHandler.Get_AttackPressed)
            {
                _stateMachine.ChangeState(State.DashAttack);
            }

            if (!_dashController.isDashing)
            {
                _stateMachine.ChangeState(State.Movement);
            }
        }
    }
}