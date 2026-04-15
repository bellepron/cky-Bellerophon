using System.Collections.Generic;
using Bellepron.Cast;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerCastController : IInitializable, IDisposable, ITickable
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerFacade _facade;
        [Inject] readonly CastProjectile.Factory _defaultCastFactory;
        [Inject] readonly WeaponHolder _weaponHolder;
        [Inject] readonly PlayerRotationController _rotationController;
        [Inject] readonly PlayerAnimatorController _animatorController;
        [Inject] readonly CastTargeter _castTargeter;
        [Inject] readonly SignalBus _signalBus;

        List<CastProjectileEcho> _castProjectileEchoes = new List<CastProjectileEcho>();
        IMemoryPool _castEchoPool;

        public bool IsCasting { get; private set; }

        public void TriggerCast()
        {
            IsCasting = true;
            _castTargeter.Activate(true);
            _rotationController.RotateToMouse();
            _animatorController.PlayCast();
        }

        public void Interrupt()
        {
            IsCasting = false;
            _castTargeter.Activate(false);
        }

        public void CreateCast()
        {
            _castTargeter.Activate(false);
            var castProjectile = _defaultCastFactory.Create(_castTargeter.Target, _settings.targetLayer, _facade.gameObject);
            castProjectile.transform.SetPositionAndRotation(_weaponHolder.leftHandCastTransform.position, _facade.Rotation);
        }

        public void Initialize()
        {
            _signalBus.Subscribe<CastProjectileEchoSpawnedSignal>(OnCastProjectileEchoSpawned);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<CastProjectileEchoSpawnedSignal>(OnCastProjectileEchoSpawned);
        }

        private void OnCastProjectileEchoSpawned(CastProjectileEchoSpawnedSignal signal)
        {
            if (_castEchoPool == null) _castEchoPool = signal.pool;
            // _castProjectileEchoes.Add(signal.castProjectileEcho);
        }

        public void Tick()
        {
            // foreach (var cpe in _castProjectileEchoes)
            // {
            //     if (!cpe.IsHostAlive)
            //     {
            //         _castProjectileEchoes.Remove(cpe);
            //         _castEchoPool.Despawn(cpe);
            //     }
            // }
        }

        [Serializable]
        public class Settings
        {
            public float castBackwardForce = 3.5f;
            public float pointerMaxDistance = 10f;
            public float pointerMaxTargetAngle = 30f;
            public float pointerStartOffset = 0.5f;
            public float pointerHeight = 1f;
            public LayerMask obstacleLayer;
            public LayerMask targetLayer;
            public GameObject defaultCastProjectilePrefab;
            public GameObject castProjectileResiduePrefab;
            public GameObject castProjectileEchoPrefab;
        }
    }
}