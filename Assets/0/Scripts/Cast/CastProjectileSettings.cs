using UnityEngine;

namespace Bellepron.Cast
{
    [CreateAssetMenu(fileName = "CastProjectileSettings", menuName = "Bellepron/Player/Cast Projectile Settings")]
    public class CastProjectileSettings : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Travel speed in units/second.")]
        public float speed = 18f;

        [Tooltip("Rotation speed toward target in degrees/second. Lower = lazier homing.")]
        public float turnSpeed = 120f;

        [Header("Damage")]
        [Tooltip("Damage dealt per hit.")]
        public LayerMask hitMask;
        public int damage = 25;

        [Header("Lifetime")]
        [Tooltip("Total seconds before the projectile self-destructs when it misses everything.")]
        public float lifetime = 6f;
        public float lifetimeOnEnemy = 2f;

        [Header("Bounce / Chain")]
        [Tooltip("How many enemies it will chain through (inclusive of first hit). " +
                 "0 = no chaining, just a single-target projectile.")]
        public int maxBounces = 3;

        [Tooltip("Search radius around an impact point for the next bounce target.")]
        public float bounceSearchRadius = 2.5f;

        [Tooltip("After each bounce this timer resets to this value. " +
                 "Should be less than `lifetime`. Projectile dies when it expires without hitting.")]
        public float bounceLifetime = 1f;

        [Header("Residue")]
        public float residueWaitTime = 2f;
        public float residueRadius = 1f;
        public float residueReturnSpeed = 8f;

        [Header("Echo")]
        public float waitDurationOnEnemy = 10f;
    }
}