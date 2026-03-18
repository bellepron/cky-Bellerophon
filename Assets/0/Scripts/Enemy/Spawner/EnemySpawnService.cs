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

    // Her EnemyType için WithId ile bind edilmiþ pool'u resolve eder.
    public class EnemySpawnService : IEnemySpawnService
    {
        // Zenject, [Inject(Id = EnemyType.X)] ile doðru pool'u atar
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
            // Pool kendi aktif listesini tutmaz; despawn çaðrýsý enemy üzerinden yapýlýr.
            // Sahnedeki tüm aktif enemy'leri bulmak için ayrý bir tracker tutulabilir.
            // Basit yaklaþým: EnemyFacade.Despawn() kendi pool'unu zaten biliyor.
            var active = Object.FindObjectsByType<EnemyFacade>(FindObjectsSortMode.None);
            foreach (var enemy in active)
                enemy.Despawn();
        }
    }
}