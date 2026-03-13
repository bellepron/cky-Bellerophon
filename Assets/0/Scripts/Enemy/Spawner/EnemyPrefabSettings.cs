using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bellepron.Enemy
{
    [CreateAssetMenu(menuName = "Bellepron/Enemy Prefab Settings")]
    public class EnemyPrefabSettings : ScriptableObject
    {
        [SerializeField] private List<EnemyPrefabEntry> _entries = new();

        public GameObject GetPrefab(EnemyTypes type)
        {
            foreach (var entry in _entries)
            {
                if (entry.Type == type)
                    return entry.Prefab;
            }

            throw new ArgumentOutOfRangeException(
                nameof(type), $"No prefab registered for EnemyType: {type}");
        }

        public int GetInitialPoolSize(EnemyTypes type)
        {
            foreach (var entry in _entries)
                if (entry.Type == type) return entry.InitialPoolSize;

            return 1;
        }

        [Serializable]
        private class EnemyPrefabEntry
        {
            public EnemyTypes Type;
            public GameObject Prefab;
            public int InitialPoolSize = 1;
        }
    }
}