using System.Collections.Generic;
using System.Collections;
using Bellepron.Utility;
using UnityEngine;
using Zenject;

namespace Bellepron.Cast
{
    public class CastProjectile : MonoBehaviour, IPoolable<IDamageable, LayerMask, GameObject, IMemoryPool>
    {
        [SerializeField] CastProjectileSettings settings;
        [Inject] CastProjectileResidue.Factory _residueFactory;
        [Inject] CastProjectileEcho.Factory _echoFactory;

        bool _dead;
        IDamageable _target;
        LayerMask _hitMask;
        GameObject _instigator;
        int _bouncesLeft;
        Coroutine _lifetimeCoroutine;

        private readonly HashSet<IDamageable> _hitTargets
        = new HashSet<IDamageable>();

        IMemoryPool _pool;

        public void OnSpawned(IDamageable target, LayerMask hitMask, GameObject instigator, IMemoryPool pool)
        {
            _pool = pool;

            _dead = false;
            _target = target;
            _hitMask = hitMask;
            _instigator = instigator;

            _hitTargets.Clear();
            _bouncesLeft = settings.maxBounces;
            _lifetimeCoroutine = StartCoroutine(LifetimeRoutine(settings.lifetime));
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Despawn()
        {
            _pool.Despawn(this);
        }

        // ── Unity Lifecycle ───────────────────────────────────────────────────

        private void Start()
        {
            if (settings == null)
            {
                Despawn();
            }
        }

        private void Update()
        {
            if (_dead || settings == null) return;

            // Drop dead targets so we fly straight instead of chasing air
            if (_target != null && !_target.IsAlive)
            {
                _target = FindNextBounceTarget(transform.position);
            }

            HomingRotation();

            transform.position += transform.forward * (settings.speed * Time.deltaTime);
        }

        // ── Collision ─────────────────────────────────────────────────────────

        private void OnTriggerEnter(Collider other)
        {
            if (_dead) return;

            _lifetimeCoroutine = StartCoroutine(LifetimeRoutine(settings.bounceLifetime));

            if (!settings.hitMask.Contains(other.gameObject.layer)) return;

            if (other.TryGetComponent<IDamageable>(out var hitTarget))
            {
                // Ignore non-targets and already-hit enemies
                if (hitTarget == null || _hitTargets.Contains(hitTarget)) return;

                _hitTargets.Add(hitTarget);

                // ── Deal damage ───────────────────────────────────────────────────
                hitTarget.TakeDamage(settings.damage, _instigator);
                // SpawnVFX

                _bouncesLeft--;

                // ── Decide what happens next ──────────────────────────────────────
                if (_bouncesLeft <= 0)
                {
                    // Reached max chain depth → die cleanly
                    CreateCastProjectileEcho(hitTarget);
                    DestroySelf(wasHit: true);
                    return;
                }

                // Search for the nearest unhit enemy within the bounce radius
                var nextTarget = FindNextBounceTarget(transform.position);

                if (_lifetimeCoroutine != null)
                    StopCoroutine(_lifetimeCoroutine);

                if (nextTarget != null)
                {
                    _target = nextTarget;
                    // Re-point toward new target immediately so we don't U-turn awkwardly
                    Vector3 toNext = (nextTarget.Transform.position - transform.position).normalized;
                    if (toNext != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(toNext);
                }
                else
                {
                    // No target found nearby – keep flying for bounceLifetime and expire
                    _target = null;
                    CreateCastProjectileEcho(hitTarget);
                    DestroySelf(wasHit: false);
                }
            }
            else
            {
                CreateCastProjectileResidue(transform.position - transform.forward, CastProjectileResidue.Phase.Waiting);

                DestroySelf(wasHit: true);
            }
        }

        private void CreateCastProjectileResidue(Vector3 pos, CastProjectileResidue.Phase phase)
        {
            _residueFactory.Create(pos, _instigator, phase);
        }

        private void CreateCastProjectileEcho(IDamageable iDamageable)
        {
            if (iDamageable.IsAlive)
            {
                var echo = _echoFactory.Create(iDamageable, _instigator);
            }
            else
            {
                CreateCastProjectileResidue(iDamageable.Transform.position, CastProjectileResidue.Phase.Waiting);
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────

        /// <summary>Smoothly rotates the projectile toward the locked target.</summary>
        private void HomingRotation()
        {
            if (_target == null || !_target.IsAlive) return;

            Vector3 direction = _target.Transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            if (direction == Vector3.zero) return;

            Quaternion desiredRot = Quaternion.LookRotation(direction);
            float step = settings.turnSpeed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, step);
        }

        /// <summary>
        /// Searches an OverlapSphere for the closest valid bounce target,
        /// excluding any already hit enemy in this chain.
        /// </summary>
        private IDamageable FindNextBounceTarget(Vector3 origin)
        {
            Collider[] candidates = Physics.OverlapSphere(
                origin,
                settings.bounceSearchRadius,
                _hitMask,
                QueryTriggerInteraction.Collide);

            IDamageable best = null;
            float bestDistSq = float.MaxValue;

            foreach (Collider col in candidates)
            {
                var candidate = col.GetComponent<IDamageable>();
                if (candidate == null) continue;
                if (_hitTargets.Contains(candidate)) continue;
                if (!candidate.IsAlive) continue;

                float distSq = (col.transform.position - origin).sqrMagnitude;
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    best = candidate;
                }
            }

            return best;
        }

        /// <summary>Self-destruct after a duration without a hit.</summary>
        private IEnumerator LifetimeRoutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (!_dead)
            {
                CreateCastProjectileResidue(transform.position, CastProjectileResidue.Phase.Waiting);
                DestroySelf(wasHit: false);
            }
        }

        private void DestroySelf(bool wasHit)
        {
            if (_dead) return;
            _dead = true;

            // if (!wasHit)
            // SpawnVFX

            Despawn();
        }

        // ── Editor Gizmos ─────────────────────────────────────────────────────
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (settings == null) return;

            // Bounce search radius
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, settings.bounceSearchRadius);

            // Line to current target
            if (_target?.IsAlive == true)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, _target.Transform.position);
                Gizmos.DrawWireSphere(_target.Transform.position, 0.4f);
            }

            // Forward direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
#endif

        public class Factory : PlaceholderFactory<IDamageable, LayerMask, GameObject, CastProjectile>
        {

        }
    }
}