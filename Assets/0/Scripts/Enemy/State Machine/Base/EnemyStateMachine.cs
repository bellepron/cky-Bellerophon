using Bellepron.StateMachine;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyStateMachine : BaseStateMachine
    {
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly IdleState _idleState;
        [Inject] readonly ChaseState _chaseState;
        [Inject] readonly AttackState _attackState;
        [Inject] readonly PatrolState _patrolState;

        public override void Initialize()
        {
            SwitchState(_idleState);
        }

        public void ChangeState(State newState)
        {
            if (_facade.State == newState) return;

            _facade.State = newState;

            SwitchState(EnumToState(newState));
        }

        EnemyBaseState EnumToState(State newState)
        {
            return newState switch
            {
                State.Idle => _idleState,
                State.Chase => _chaseState,
                State.Attack => _attackState,
                State.Patrol => _patrolState,
                _ => _idleState
            };
        }
    }
}