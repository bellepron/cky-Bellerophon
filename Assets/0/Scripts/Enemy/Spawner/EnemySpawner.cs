using Random = UnityEngine.Random;
using Bellepron.Enemy;
using UnityEngine;
using Zenject;

namespace Bellepron.Spawners
{
    public class EnemySpawner : IInitializable, ITickable
    {
        [Inject] private IEnemySpawnService _spawnService;
        [Inject] readonly SignalBus _signalBus;

        public void Initialize()
        {
            Spawn(EnemyType.Satyr, new Vector3(-2, 0, 0));
            // Spawn(EnemyType.Minotaur, new Vector3(-3, 0, 0));
            // Spawn(EnemyType.Hydra, new Vector3(-4, 0, 0));
            // Spawn(EnemyType.Satyr, new Vector3(-6, 0, 0));
            // Spawn(EnemyType.Minotaur, new Vector3(-7, 0, 0));
            // Spawn(EnemyType.Hydra, new Vector3(-8, 0, 0));
        }

        // API

        public EnemyFacade Spawn(EnemyType type, Vector3 position)
        {
            return _spawnService.Spawn(type, position);
        }

        public void Despawn(EnemyFacade enemy)
        {
            _spawnService.Despawn(enemy);
        }

        public void DespawnAll()
        {
            _spawnService.DespawnAll();
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Y))
                Spawn(EnemyType.Satyr, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            if (Input.GetKeyDown(KeyCode.U))
                Spawn(EnemyType.Minotaur, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            if (Input.GetKeyDown(KeyCode.I))
                Spawn(EnemyType.Hydra, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            if (Input.GetKeyDown(KeyCode.O))
                DespawnAll();
        }
    }
}