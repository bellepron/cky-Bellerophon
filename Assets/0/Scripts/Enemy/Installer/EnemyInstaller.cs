using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyInstaller : MonoInstaller
    {
        // Hangi tip için kurulduðunu GameInstaller atar (BindInstance ile)
        [SerializeField] private EnemyTypes _enemyType;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EnemyHealthController>().AsSingle();

            // EnemyType'ý container'a göm — EnemyFacade inject olarak alýr
            Container.BindInstance(_enemyType).AsSingle();

            // Tipe özel baðýmlýlýklar
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