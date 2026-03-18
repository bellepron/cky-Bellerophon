using Bellepron.UI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyInstaller : MonoInstaller
    {
        // Hangi tip icin kuruldugunu GameInstaller atar (BindInstance ile)
        [SerializeField] EnemyType _enemyType;
        [SerializeField] HealthBarController _healthBarController;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EnemyHealthController>().AsSingle();
            Container.Bind<HealthBarController>().FromInstance(_healthBarController).AsSingle();

            Container.BindInstance(_enemyType).AsSingle();

            switch (_enemyType)
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