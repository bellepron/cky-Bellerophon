using UnityEngine.AI;
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

        [Space(10)]
        [SerializeField] Animator animator;
        [SerializeField] CapsuleCollider capsuleCollider;
        [SerializeField] Rigidbody rb;
        [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] NavMeshAgent navMeshAgent;

        public override void InstallBindings()
        {
            Container.Bind<EnemyFacade>().FromInstance(enemyFacade).AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyStateMachine>().AsSingle().NonLazy();

            Container.Bind<IdleState>().AsSingle();
            Container.Bind<ChaseState>().AsSingle();
            Container.Bind<AttackState>().AsSingle();
            Container.Bind<PatrolState>().AsSingle();

            Container.BindInterfacesAndSelfTo<EnemyMovementController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyAttackController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyAnimationController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyHealthController>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyDetectionController>().AsSingle();
            Container.Bind<HealthBarController>().FromInstance(_healthBarController).AsSingle();

            // Container.Bind<Animator>().FromInstance(animator).AsSingle();
            Container.Bind<CapsuleCollider>().FromInstance(capsuleCollider).AsSingle();
            Container.Bind<Rigidbody>().FromInstance(rb).AsSingle();
            // Container.Bind<SkinnedMeshRenderer>().FromInstance(skinnedMeshRenderer).AsSingle();
            Container.Bind<NavMeshAgent>().FromInstance(navMeshAgent).AsSingle();

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