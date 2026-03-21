using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] PlayerFacade playerFacade;
        [SerializeField] PlayerAnimationEventController playerAnimationEventController;
        [SerializeField] WeaponHolder weaponHolder;
        [SerializeField] CinemachineImpulseSource cinemachineImpulseSource;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerInputHandler>().AsSingle();
            Container.Bind<PlayerFacade>().FromInstance(playerFacade).AsSingle();
            Container.Bind<PlayerAnimationEventController>().FromInstance(playerAnimationEventController).AsSingle();
            Container.Bind<WeaponHolder>().FromInstance(weaponHolder).AsSingle();
            Container.Bind<CinemachineImpulseSource>().FromInstance(cinemachineImpulseSource).AsSingle();
            Container.Bind<PlayerStatus>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerStateMachine>().AsSingle().NonLazy();
            Container.Bind<PlayerMovementController>().AsSingle();
            Container.Bind<PlayerRotationController>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerAnimatorController>().AsSingle();
            Container.Bind<PlayerAttackController>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerDashController>().AsSingle();
            Container.Bind<PlayerGhostTrailController>().AsSingle();
            Container.Bind<PlayerGhostSpawner>().AsSingle();
            Container.Bind<MovementState>().AsSingle();
            Container.Bind<DashState>().AsSingle();
            Container.Bind<AttackState>().AsSingle();
            Container.Bind<SpecialState>().AsSingle();
            Container.Bind<CastState>().AsSingle();
            Container.Bind<DashAttackState>().AsSingle();
            Container.Bind<PlayerDamageHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerInteractController>().AsSingle();
        }
    }
}