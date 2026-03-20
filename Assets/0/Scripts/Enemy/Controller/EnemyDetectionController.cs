using Bellepron.Player;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyDetectionController : IInitializable, ITickable, IFixedTickable
    {
        [Inject] readonly EnemySettings _settings;
        [Inject] readonly EnemyFacade _facade;
        [Inject] readonly PlayerHolder _playerHolder;

        Transform _playerTransform;
        float _rangeCheckTimer;
        float _loseTargetTimer;

        public Transform CurrentTarget { get; private set; }
        public bool HasTarget => CurrentTarget != null;
        public bool HasLineOfSight { get; private set; }

        public void Initialize()
        {
            _playerTransform = _playerHolder.PlayerFacade.transform;
        }

        public void Tick()
        {
            if (!HasTarget)
            {
                _rangeCheckTimer += Time.deltaTime;
                if (_rangeCheckTimer < _settings.checkPlayerInterval) return;

                _rangeCheckTimer = 0f;

                if (IsPlayerInRange())
                    CurrentTarget = _playerTransform;
            }
            else
            {
                _loseTargetTimer += Time.deltaTime;
                if (_loseTargetTimer >= _settings.losePlayerDuration)
                {
                    LoseTarget();
                }
            }
        }

        public void FixedTick()
        {
            if (!HasTarget)
            {
                HasLineOfSight = false;
                return;
            }

            HasLineOfSight = CheckLineOfSight();

            if (HasLineOfSight)
                _loseTargetTimer = 0f;
        }

        public void ResetDetection()
        {
            LoseTarget();
            _rangeCheckTimer = _settings.checkPlayerInterval;
        }

        // ── Private ──────────────────────────────────────────────

        bool IsPlayerInRange()
        {
            var player = _playerHolder.PlayerFacade;
            if (player == null) return false;

            Vector3 toPlayer = player.Position - _facade.Position;
            return toPlayer.sqrMagnitude <= _settings.detectionRange * _settings.detectionRange;
        }

        bool CheckLineOfSight()
        {
            if (!_settings.useLosCheck)
                return true;

            if (_playerTransform == null)
            {
                Debug.LogWarning("Player transform bulunamadı!");
                return false;
            }

            Vector3 origin = _facade.Position + Vector3.up;
            Vector3 toPlayer = _playerTransform.position - _facade.Position;
            origin += toPlayer.normalized;

            if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, _settings.losRange))
                return hit.collider.transform == _playerTransform;

            return false;
        }

        void LoseTarget()
        {
            CurrentTarget = null;
            HasLineOfSight = false;
            _loseTargetTimer = 0f;
            _rangeCheckTimer = _settings.checkPlayerInterval;
        }
    }
}