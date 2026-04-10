using UnityEngine;
using Zenject;

namespace Bellepron.Cast
{
    public class CastProjectileEcho : MonoBehaviour, IPoolable<IDamageable, GameObject, IMemoryPool>
    {
        [SerializeField] CastProjectileSettings settings;
        [Inject] readonly CastProjectileResidue.Factory _residueFactory;
        [Inject] readonly SignalBus _signalBus;
        IDamageable _host;
        GameObject _instigator;
        IMemoryPool _pool;

        public bool IsHostAlive => _host.IsAlive;

        float _waitTimer;

        // ── IPoolable ──────────────────────────────────────────────────────────

        public void OnSpawned(IDamageable host, GameObject instigator, IMemoryPool pool)
        {
            _pool = pool;
            _host = host;
            _instigator = instigator;

            _waitTimer = settings != null ? settings.waitDurationOnEnemy : 2f;

            // Attach to the enemy
            transform.SetParent(_host.Transform, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _signalBus.Fire(new CastProjectileEchoSpawnedSignal { castProjectileEcho = this, pool = _pool });

            // TODO: Play "attached" VFX / idle animation
        }

        public void OnDespawned()
        {
            _pool = null;
            _host = null;
            _instigator = null;

            transform.SetParent(null, worldPositionStays: false);
            // TODO: Stop any looping VFX
        }

        // ── Unity Lifecycle ────────────────────────────────────────────────────

        private void Update()
        {
            if (_pool == null) return;

            UpdateWaiting();
        }

        // ── Phase logic ────────────────────────────────────────────────────────

        private void UpdateWaiting()
        {
            // Enemy died or wait timer expired → start flying back
            bool hostDead = _host == null || !_host.IsAlive;
            bool timedOut = (_waitTimer -= Time.deltaTime) <= 0f;

            if (hostDead || timedOut)
            {
                CreateCastProjectileResidue(transform.position);
                _pool.Despawn(this);
            }
        }

        private void CreateCastProjectileResidue(Vector3 pos)
        {
            _residueFactory.Create(pos, _instigator, CastProjectileResidue.Phase.Waiting);
        }

        // ── Editor Gizmos ─────────────────────────────────────────────────────
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_instigator == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _instigator.transform.position);
            Gizmos.DrawWireSphere(_instigator.transform.position, 0.3f);
        }
#endif

        // ── Factory ────────────────────────────────────────────────────────────

        public class Factory : PlaceholderFactory<IDamageable, GameObject, CastProjectileEcho> { }
    }
}