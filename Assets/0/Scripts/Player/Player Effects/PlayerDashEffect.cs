using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerDashEffect : MonoBehaviour, IPoolable<Vector3, Vector3, IMemoryPool>
    {
        float _lifeTime;

        [SerializeField] ParticleSystem _particleSystem;

        float _startTime;

        IMemoryPool _pool;

        public void Update()
        {
            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _pool.Despawn(this);
            }
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(Vector3 pos, Vector3 dashDir, IMemoryPool pool)
        {
            if (_particleSystem == null) return;

            _pool = pool;

            transform.position = pos;
            _particleSystem.transform.forward = dashDir;

            _particleSystem.Clear();
            _particleSystem.Play();

            _lifeTime = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
            _startTime = Time.realtimeSinceStartup;
        }

        public class Factory : PlaceholderFactory<Vector3, Vector3, PlayerDashEffect>
        {
        }
    }
}