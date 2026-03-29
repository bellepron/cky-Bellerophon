using UnityEngine;
using Zenject;

namespace Bellepron.Cast
{
    public class CastProjectileResidue : MonoBehaviour, IPoolable<Vector3, GameObject, CastProjectileResidue.Phase, IMemoryPool>
    {
        [SerializeField] CastProjectileSettings settings;

        public enum Phase { Waiting, Returning }

        Phase _phase;
        GameObject _instigator;
        IMemoryPool _pool;
        float _waitTimer;
        float _returnSpeed;

        // ── IPoolable ──────────────────────────────────────────────────────────

        public void OnSpawned(Vector3 position, GameObject instigator, Phase phase, IMemoryPool pool)
        {
            _pool = pool;

            transform.position = position;
            _instigator = instigator;
            _phase = phase;
            _returnSpeed = settings.residueReturnSpeed + UnityEngine.Random.Range(-1, 1);

            _waitTimer = settings != null ? settings.residueWaitTime : 2f;
        }

        public void OnDespawned()
        {
            _pool = null;
            // TODO: Stop any looping VFX here
        }

        // ── Unity Lifecycle ────────────────────────────────────────────────────

        private void Update()
        {
            if (_pool == null) return;

            if (InInstigatorScope())
            {
                _pool.Despawn(this);
            }
            else
            {
                if (_phase == Phase.Waiting)
                {
                    _waitTimer -= Time.deltaTime;
                    if (_waitTimer <= 0f)
                    {
                        _phase = Phase.Returning;
                    }
                }
                else if (_phase == Phase.Returning)
                {
                    transform.position += (_instigator.transform.position - transform.position).normalized * settings.residueReturnSpeed * Time.deltaTime;
                }
            }
        }

        bool InInstigatorScope()
        {
            var instigatorPos = _instigator.transform.position;
            instigatorPos.y = 0;
            var thisPos = transform.position;
            thisPos.y = 0;
            var inScope = Vector3.Distance(instigatorPos, thisPos) <= settings.residueRadius;

            return inScope;
        }

        // ── Factory ────────────────────────────────────────────────────────────

        public class Factory : PlaceholderFactory<Vector3, GameObject, Phase, CastProjectileResidue> { }
    }
}