using Bellepron.UI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyInstaller : MonoInstaller
    {
        // Hangi tip icin kuruldugunu GameInstaller atar (BindInstance ile)
        [SerializeField] EnemyFacade enemyFacade;
        [SerializeField] EnemyType enemyType;
        [SerializeField] HealthBarController _healthBarController;

        public override void InstallBindings()
        {
            Container.Bind<EnemyFacade>().FromInstance(enemyFacade).AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyStateMachine>().AsSingle().NonLazy();

            Container.Bind<IdleState>().AsSingle();
            Container.Bind<ChaseState>().AsSingle();
            Container.Bind<AttackState>().AsSingle();
            Container.Bind<PatrolState>().AsSingle();

            Container.BindInterfacesAndSelfTo<EnemyHealthController>().AsSingle();
            Container.Bind<HealthBarController>().FromInstance(_healthBarController).AsSingle();

            Container.BindInstance(enemyType).AsSingle();

            switch (enemyType)
            {
                case EnemyType.Satyr:
                    BindSatyr();
                    break;

                case EnemyType.Minotaur:
                    BindMinotaur();
                    break;

                case EnemyType.Hydra:
                    BindHydra();
                    break;
            }
        }

        private void BindSatyr()
        {

        }

        private void BindMinotaur()
        {

        }

        private void BindHydra()
        {

        }
    }
}