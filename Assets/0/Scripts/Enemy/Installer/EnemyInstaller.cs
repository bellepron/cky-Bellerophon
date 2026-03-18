using Bellepron.UI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyInstaller : MonoInstaller
    {
        // Hangi tip icin kuruldugunu GameInstaller atar (BindInstance ile)
        [SerializeField] EnemyTypes _enemyType;
        [SerializeField] HealthBarController _healthBarController;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EnemyHealthController>().AsSingle();
            Container.Bind<HealthBarController>().FromInstance(_healthBarController).AsSingle();

            Container.BindInstance(_enemyType).AsSingle();

            switch (_enemyType)
            {
                case EnemyTypes.Satyr:
                    BindSatyr();
                    break;

                case EnemyTypes.Minotaur:
                    BindMinotaur();
                    break;

                case EnemyTypes.Hydra:
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