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
            // Spawn(EnemyTypes.Satyr, new Vector3(-2, 0, 0));
            // Spawn(EnemyTypes.Minotaur, new Vector3(-3, 0, 0));
            // Spawn(EnemyTypes.Hydra, new Vector3(-4, 0, 0));
            // Spawn(EnemyTypes.Satyr, new Vector3(-6, 0, 0));
            // Spawn(EnemyTypes.Minotaur, new Vector3(-7, 0, 0));
            // Spawn(EnemyTypes.Hydra, new Vector3(-8, 0, 0));

            Spawn(EnemyTypes.Satyr, new Vector3(-2.0f, 0, -0.5f));
            Spawn(EnemyTypes.Satyr, new Vector3(-3.25f, 0, -0.5f));
            Spawn(EnemyTypes.Satyr, new Vector3(-1.0f, 0, -4.0f));
        }

        // API

        public EnemyFacade Spawn(EnemyTypes type, Vector3 position)
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
                Spawn(EnemyTypes.Satyr, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            if (Input.GetKeyDown(KeyCode.U))
                Spawn(EnemyTypes.Minotaur, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            if (Input.GetKeyDown(KeyCode.I))
                Spawn(EnemyTypes.Hydra, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            if (Input.GetKeyDown(KeyCode.O))
                DespawnAll();
        }
    }
}