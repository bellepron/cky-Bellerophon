using UnityEngine;
using Zenject;

namespace Bellepron.StateMachine
{
    public abstract class BaseStateMachine : ITickable, IFixedTickable, IInitializable
    {
        private BaseState currentState;

        public void SwitchState(BaseState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        protected virtual void Tick() => currentState?.Tick(Time.deltaTime);
        protected virtual void FixedTick() => currentState?.FixedTick(Time.fixedDeltaTime);

        public virtual void Initialize() { }
        void ITickable.Tick() { if (Time.timeScale != 0) Tick(); }
        void IFixedTickable.FixedTick() { if (Time.timeScale != 0) FixedTick(); }
    }
}