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

        Transform _enemiesParent;
        Transform _projectilesParent;
        const string _nameEnemiesParent = "Enemies";
        const string _nameProjectilesParent = "Projectiles";
        const string _nameEffectsParent = "Effects";
        const string _namePlayerGhostsParent = "PlayerGhosts";

        public override void InstallBindings()
        {
            #region General

            Container.Bind<CameraManager>().FromInstance(cameraManager).AsSingle();

            Container.Bind<CoroutineRunner>().FromInstance(coroutineRunner).AsSingle();

            Container.Bind<TimeScaleManager>().AsSingle();

            Container.Bind<GameStateController>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIManager>().AsSingle();
            Container.Bind<CursorController>().AsSingle();

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
                   .UnderTransformGroup(_namePlayerGhostsParent));

            Container.BindFactory<Vector3, Vector3, PlayerDashEffect, PlayerDashEffect.Factory>()
                .FromPoolableMemoryPool<Vector3, Vector3, PlayerDashEffect, PlayerDashEffectPool>(poolBinder => poolBinder
                    .WithInitialSize(3)
                    .FromComponentInNewPrefab(_playerSettings.PlayerDashControllerSettings.playerDashEffectPrefab)
                    .UnderTransformGroup(_nameEffectsParent));

            #endregion

            #region Enemies

            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle();
            _enemiesParent = new GameObject(_nameEnemiesParent).transform;
            Container.Bind<EnemiesParent>().FromInstance(new EnemiesParent(_enemiesParent)).AsSingle();

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

            _projectilesParent = new GameObject(_nameProjectilesParent).transform;
            Container.Bind<ProjectilesParent>().FromInstance(new ProjectilesParent(_projectilesParent)).AsSingle();

            Container.BindFactory<IDamageable, LayerMask, GameObject, CastProjectile, CastProjectile.Factory>()
                   .FromPoolableMemoryPool<IDamageable, LayerMask, GameObject, CastProjectile, DefaultCastProjectilePool>(poolBinder => poolBinder
                       .WithInitialSize(1)
                       .FromComponentInNewPrefab(_playerSettings.PlayerCastControllerSettings.defaultCastProjectilePrefab)
                       .UnderTransformGroup(_nameProjectilesParent));

            Container.BindFactory<Vector3, GameObject, CastProjectileResidue.Phase, CastProjectileResidue, CastProjectileResidue.Factory>()
                   .FromPoolableMemoryPool<Vector3, GameObject, CastProjectileResidue.Phase, CastProjectileResidue, CastProjectileResiduePool>(poolBinder => poolBinder
                       .WithInitialSize(1)
                       .FromComponentInNewPrefab(_playerSettings.PlayerCastControllerSettings.castProjectileResiduePrefab)
                       .UnderTransformGroup(_nameProjectilesParent));

            Container.BindFactory<IDamageable, GameObject, CastProjectileEcho, CastProjectileEcho.Factory>()
                   .FromPoolableMemoryPool<IDamageable, GameObject, CastProjectileEcho, CastProjectileEchoPool>(poolBinder => poolBinder
                       .WithInitialSize(1)
                       .FromComponentInNewPrefab(_playerSettings.PlayerCastControllerSettings.castProjectileEchoPrefab)
                       .UnderTransformGroup(_nameProjectilesParent));

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

    public class EnemiesParent
    {
        public Transform Transform { get; }

        public EnemiesParent(Transform transform)
        {
            Transform = transform;
        }
    }

    public class ProjectilesParent
    {
        public Transform Transform { get; }

        public ProjectilesParent(Transform transform)
        {
            Transform = transform;
        }
    }
}