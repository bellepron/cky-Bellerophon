using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    [CreateAssetMenu(menuName = "Bellepron/Enemy Settings")]
    public class EnemySettings : ScriptableObjectInstaller
    {
        [Header("Movement")]
        public float movementSpeed = 2.5f;

        [Header("Health")]
        public int health = 100;

        [Header("Attack")]
        public float attackableDistance = 2.0f;

        [Header("Player Detection")]
        public float checkPlayerInterval = 2f;
        public float losePlayerDuration = 2f;

        [Header("Line of Sight")]
        public bool useLosCheck = true;
        public float detectionRange = 5f;
        public float losRange = 10f;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }
    }
}