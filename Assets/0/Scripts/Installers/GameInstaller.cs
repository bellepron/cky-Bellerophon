using Bellepron.Spawners;
using Bellepron.Player;
using Bellepron.Enemy;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron
{
    public class GameInstaller : MonoInstaller
    {
        [Inject] Settings _settings;
        [Inject] PlayerSettings _playerSettings;
        [SerializeField] CoroutineRunner coroutineRunner;
        [SerializeField] EnemyPrefabSettings _enemyPrefabSettings;

        public override void InstallBindings()
        {
            #region General

            Container.Bind<CoroutineRunner>()
            .FromInstance(coroutineRunner)
            .AsSingle();

            Container.Bind<TimeScaleManager>().AsSingle();

            #endregion

            #region Player

            Container.BindInterfacesAndSelfTo<PlayerSpawner>().AsSingle();
            Container.Bind<PlayerHolder>().AsSingle();

            Container.BindFactory<PlayerFacade, PlayerFacade.Factory>()
                .FromSubContainerResolve()
                .ByNewContextPrefab(_playerSettings.PlayerPrefab);

            Container.BindFactory<PlayerGhostFacade, PlayerGhostFacade.Factory>()
                   .FromPoolableMemoryPool<PlayerGhostFacade, PlayerGhostFacadePool>(poolBinder => poolBinder
                   .WithInitialSize(5)
                   .FromComponentInNewPrefab(_playerSettings.PlayerGhostTrailControllerSettings.playerGhostPrefab)
                   .UnderTransformGroup("PlayerGhostPool"));

            Container.BindFactory<Vector3, Vector3, PlayerDashEffect, PlayerDashEffect.Factory>()
                .FromPoolableMemoryPool<Vector3, Vector3, PlayerDashEffect, PlayerDashEffectPool>(poolBinder => poolBinder
                    .WithInitialSize(3)
                    .FromComponentInNewPrefab(_playerSettings.PlayerDashControllerSettings.playerDashEffectPrefab)
                    .UnderTransformGroup("Effects"));

            #endregion

            #region Enemies

            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle();
            var _enemiesParent = new GameObject("Enemies").transform;
            Container.BindInstance(_enemiesParent).WithId("EnemiesParent").AsSingle();

            //// Resolve yok — Zenject constructor injection ile yapıyorum.
            BindEnemyPool(EnemyTypes.Satyr);
            BindEnemyPool(EnemyTypes.Minotaur);
            BindEnemyPool(EnemyTypes.Hydra);

            // Spawner interface
            Container.Bind<IEnemySpawnService>()
                     .To<EnemySpawnService>()
                     .AsSingle();

            #endregion

            #region Projectiles



            #endregion

            #region Signals

            GameSignalsInstaller.Install(Container);

            #endregion
        }

        [Serializable]
        public class Settings
        {

        }

        #region Player Pools

        class PlayerGhostFacadePool : MonoPoolableMemoryPool<IMemoryPool, PlayerGhostFacade>
        {
        }

        class PlayerDashEffectPool : MonoPoolableMemoryPool<Vector3, Vector3, IMemoryPool, PlayerDashEffect>
        {
        }

        #endregion

        #region Enemy Pool

        private void BindEnemyPool(EnemyTypes type)
        {
            var prefab = _enemyPrefabSettings.GetPrefab(type);
            var poolSize = _enemyPrefabSettings.GetInitialPoolSize(type);

            Container.BindMemoryPool<EnemyFacade, EnemyFacadePool>()
                    .WithId(type)
                    .WithInitialSize(poolSize)
                    .FromSubContainerResolve()
                    .ByNewContextPrefab(prefab);
        }

        #endregion
    }

    public class EnemyFacadePool : MonoPoolableMemoryPool<Vector3, IMemoryPool, EnemyFacade>
    {
        [Inject(Id = "EnemiesParent")] private Transform _enemiesParent;

        protected override void OnCreated(EnemyFacade item)
        {
            base.OnCreated(item);
            item.transform.SetParent(_enemiesParent);
        }
    }
}