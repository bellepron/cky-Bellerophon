using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerDamageHandler
    {
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly TimeScaleManager _timeScaleManager;
        HashSet<Collider> _hitRegistry = new HashSet<Collider>();

        // Reset at start of attack
        public void ClearHitRegistry()
        {
            _hitRegistry.Clear();
        }

        // Sweep
        public void DamageEnemiesBetween(Vector3 from, Vector3 to, Vector3 forceDir, int damage, float knockback, LayerMask hitMask)
        {
            Vector3 dir = to - from;
            float dist = dir.magnitude;
            if (dist <= 0f) return;

            dir /= dist;
            float radius = _playerFacade.CapsuleRadius;

            RaycastHit[] hits = Physics.SphereCastAll(from, radius, dir, dist, hitMask);

            foreach (var hit in hits)
            {
                if (_hitRegistry.Contains(hit.collider)) continue;
                _hitRegistry.Add(hit.collider);

                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(damage);

                if (hit.collider.attachedRigidbody != null)
                    hit.collider.attachedRigidbody.AddForce(forceDir * knockback, ForceMode.Impulse);

                _timeScaleManager.StartHitStop();
            }
        }

        // Point
        public void DamageEnemiesInRadius(Vector3 center, float radius, int damage, float knockback, LayerMask hitMask)
        {
            Collider[] hits = Physics.OverlapSphere(center, radius, hitMask);
            foreach (var col in hits)
            {
                if (col.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(damage);
                if (col.attachedRigidbody != null)
                {
                    Vector3 forceDir = (col.transform.position - center).normalized;
                    col.attachedRigidbody.AddForce(forceDir * knockback, ForceMode.Impulse);
                }
            }

            if (hits.Length > 0)
            {
                _timeScaleManager.StartHitStop();
            }
        }
    }
}