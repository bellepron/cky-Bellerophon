using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerGhostFacade : MonoBehaviour, IPoolable<IMemoryPool>
    {
        public MeshFilter MeshFilter => _meshFilter;
        public MeshRenderer MeshRenderer => _meshRenderer;

        [SerializeField] MeshFilter _meshFilter;
        [SerializeField] MeshRenderer _meshRenderer;

        IMemoryPool _pool;

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Despawn()
        {
            _pool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<PlayerGhostFacade>
        {

        }
    }
}