using Bellepron.Managers;
using Bellepron.Spawners;
using Bellepron.Player;
using Bellepron.Enemy;
using Bellepron.Cast;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron
{
    public class GameInstaller : MonoInstaller
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerSettings _playerSettings;
        [SerializeField] CameraManager cameraManager;
        [SerializeField] CoroutineRunner coroutineRunner;
        [SerializeField] EnemyPrefabSettings _enemyPrefabSettings;

        public override void InstallBindings()
        {
            #region General

            Container.Bind<CameraManager>().FromInstance(cameraManager).AsSingle();

            Container.Bind<CoroutineRunner>().FromInstance(coroutineRunner).AsSingle();

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
            BindEnemyPool(EnemyType.Satyr);
            BindEnemyPool(EnemyType.Minotaur);
            BindEnemyPool(EnemyType.Hydra);

            // Spawner interface
            Container.Bind<IEnemySpawnService>()
                     .To<EnemySpawnService>()
                     .AsSingle();

            #endregion

            #region Cast Projectile

            Container.BindFactory<IDamageable, LayerMask, GameObject, CastProjectile, CastProjectile.Factory>()
                   .FromPoolableMemoryPool<IDamageable, LayerMask, GameObject, CastProjectile, DefaultCastProjectilePool>(poolBinder => poolBinder
                       .WithInitialSize(1)
                       .FromComponentInNewPrefab(_playerSettings.PlayerAttackControllerSettings.defaultCastProjectilePrefab)
                       .UnderTransformGroup("Projectiles"));

            Container.BindFactory<Vector3, GameObject, CastProjectileResidue.Phase, CastProjectileResidue, CastProjectileResidue.Factory>()
                   .FromPoolableMemoryPool<Vector3, GameObject, CastProjectileResidue.Phase, CastProjectileResidue, CastProjectileResiduePool>(poolBinder => poolBinder
                       .WithInitialSize(1)
                       .FromComponentInNewPrefab(_playerSettings.PlayerAttackControllerSettings.castProjectileResiduePrefab)
                       .UnderTransformGroup("Projectiles"));

            Container.BindFactory<IDamageable, GameObject, CastProjectileEcho, CastProjectileEcho.Factory>()
                   .FromPoolableMemoryPool<IDamageable, GameObject, CastProjectileEcho, CastProjectileEchoPool>(poolBinder => poolBinder
                       .WithInitialSize(1)
                       .FromComponentInNewPrefab(_playerSettings.PlayerAttackControllerSettings.castProjectileEchoPrefab)
                       .UnderTransformGroup("Projectiles"));

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

        private void BindEnemyPool(EnemyType type)
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

        #region Cast Projectile Pools

        class DefaultCastProjectilePool : MonoPoolableMemoryPool<IDamageable, LayerMask, GameObject, IMemoryPool, CastProjectile>
        {
        }

        class CastProjectileResiduePool : MonoPoolableMemoryPool<Vector3, GameObject, CastProjectileResidue.Phase, IMemoryPool, CastProjectileResidue>
        {

        }

        class CastProjectileEchoPool : MonoPoolableMemoryPool<IDamageable, GameObject, IMemoryPool, CastProjectileEcho>
        {

        }

        #endregion
    }
}