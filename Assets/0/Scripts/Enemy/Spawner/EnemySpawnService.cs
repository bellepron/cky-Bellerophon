using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public interface IEnemySpawnService
    {
        EnemyFacade Spawn(EnemyType type, Vector3 position);
        void Despawn(EnemyFacade enemy);
        void DespawnAll();
    }

    // Her EnemyType i�in WithId ile bind edilmi� pool'u resolve eder.
    public class EnemySpawnService : IEnemySpawnService
    {
        // Zenject, [Inject(Id = EnemyType.X)] ile do�ru pool'u atar
        [Inject(Id = EnemyType.Satyr)] private EnemyFacadePool _satyrPool;
        [Inject(Id = EnemyType.Minotaur)] private EnemyFacadePool _minotaurPool;
        [Inject(Id = EnemyType.Hydra)] private EnemyFacadePool _hydraPool;

        public EnemyFacade Spawn(EnemyType type, Vector3 position)
        {
            var pool = GetPool(type);
            return pool.Spawn(position, pool);
        }

        private EnemyFacadePool GetPool(EnemyType type) => type switch
        {
            EnemyType.Satyr => _satyrPool,
            EnemyType.Minotaur => _minotaurPool,
            EnemyType.Hydra => _hydraPool,
            _ => throw new System.ArgumentOutOfRangeException(nameof(type))
        };

        public void Despawn(EnemyFacade enemy)
        {
            GetPool(enemy.EnemyType).Despawn(enemy);
        }

        public void DespawnAll()
        {
            var active = Object.FindObjectsByType<EnemyFacade>(FindObjectsSortMode.None);
            foreach (var enemy in active)
                enemy.Despawn();
        }
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