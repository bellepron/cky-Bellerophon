using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public interface IEnemySpawnService
    {
        EnemyFacade Spawn(EnemyTypes type, Vector3 position);
        void Despawn(EnemyFacade enemy);
        void DespawnAll();
    }

    // Her EnemyType için WithId ile bind edilmiþ pool'u resolve eder.
    public class EnemySpawnService : IEnemySpawnService
    {
        // Zenject, [Inject(Id = EnemyType.X)] ile doðru pool'u atar
        [Inject(Id = EnemyTypes.Satyr)] private EnemyFacadePool _satyrPool;
        [Inject(Id = EnemyTypes.Minotaur)] private EnemyFacadePool _minotaurPool;
        [Inject(Id = EnemyTypes.Hydra)] private EnemyFacadePool _hydraPool;

        public EnemyFacade Spawn(EnemyTypes type, Vector3 position)
        {
            var pool = GetPool(type);
            return pool.Spawn(position, pool);
        }

        private EnemyFacadePool GetPool(EnemyTypes type) => type switch
        {
            EnemyTypes.Satyr => _satyrPool,
            EnemyTypes.Minotaur => _minotaurPool,
            EnemyTypes.Hydra => _hydraPool,
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