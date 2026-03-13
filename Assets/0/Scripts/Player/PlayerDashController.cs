using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerDashController : ITickable, IInitializable
    {
        [Inject] readonly SignalBus _signalBus;
        [Inject] readonly CoroutineRunner _coroutineRunner;
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerStateMachine _playerStateMachine;
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly PlayerGhostTrailController _playerGhostTrailController;
        [Inject] readonly PlayerAnimatorController _playerAnimatorController;
        [Inject] readonly CinemachineImpulseSource _cinemachineImpulseSource;
        [Inject] readonly PlayerStatus _playerStatus;
        [Inject] readonly PlayerDashEffect.Factory _playerDashEffectFactory;
        [Inject] readonly TimeScaleManager _timeScaleManager;

        int _dashCharges;
        float _dashTimer;
        float _rechargeTimer;
        Vector3 _dashStartPos;
        Vector3 _dashTargetPos;
        Vector3 _lastFramePos;
        HashSet<Collider> hitThisDash = new HashSet<Collider>(); DashCollisionChecker _dashCollisionChecker;

        public bool isDashing;

        public int DashCharges => _dashCharges;
        public float RechargeTimer => _rechargeTimer;

        public void Initialize()
        {
            _signalBus.Fire<PlayerDashControllerCreatedSignal>();
            _dashCharges = _settings.maxDashCharges;

            SetDashCollisionController();
        }

        void SetDashCollisionController()
        {
            GameObject dashCollisionCheckerObject = new GameObject("DashCollisionChecker");

            dashCollisionCheckerObject.hideFlags =
                HideFlags.HideInHierarchy |
                HideFlags.HideInInspector |
                HideFlags.DontSaveInBuild |
                HideFlags.DontSaveInEditor;

            dashCollisionCheckerObject.transform.position = Vector3.one * 999999;

            _dashCollisionChecker = dashCollisionCheckerObject.AddComponent<DashCollisionChecker>();
        }

        public void Tick()
        {
            HandleRecharge();
        }

        public void HandleRecharge()
        {
            if (_dashCharges >= _settings.maxDashCharges) return;

            _rechargeTimer -= Time.deltaTime;
            if (_rechargeTimer <= 0f)
            {
                _dashCharges++;
                _rechargeTimer = _settings.dashRechargeTime;

                _signalBus.Fire(new DashChargeRestoredSignal
                {
                    currentDashCharges = _dashCharges
                });
            }

            float t = 1f - (_rechargeTimer / _settings.dashRechargeTime);
            _signalBus.Fire(new DashRechargeSignal
            {
                currentDash = _dashCharges,
                progress = t
            });
        }

        public void TryDash()
        {
            if (CanDash())
                StartDash();
        }

        public bool CanDash()
        {
            if (_dashCharges > 0)
            {
                if (!isDashing)
                {
                    return true;
                }
            }

            return false;
        }

        void StartDash()
        {
            var dashDir = _playerFacade.Forward;

            _playerStatus.SetInvulnerable(true);
            _cinemachineImpulseSource.GenerateImpulse(_settings.dashCameraImpulseForce);
            _playerGhostTrailController.StartSpawnGhostTrail();
            _playerAnimatorController.AnimateDash();
            _playerDashEffectFactory.Create(_playerFacade.Position, dashDir);

            _dashCharges--;
            _rechargeTimer = _settings.dashRechargeTime;

            isDashing = true;
            _dashTimer = 0f;

            _dashStartPos = _playerFacade.Position;
            _dashTargetPos = ComputeDashTargetSafe(_dashStartPos, dashDir, _settings.dashSpeed * _settings.dashDuration);

            _lastFramePos = _dashStartPos;

            hitThisDash.Clear();

            _coroutineRunner.StartCoroutine(DashRoutine(dashDir));

            _signalBus.Fire(new DashChargeUsedSignal
            {
                currentDashCharges = _dashCharges
            });
        }

        IEnumerator DashRoutine(Vector3 dashDir)
        {
            while (_dashTimer < _settings.dashDuration)
            {
                _dashTimer += Time.deltaTime;
                float t = Mathf.Clamp01(_dashTimer / _settings.dashDuration);
                float curvedT = _settings.dashCurve.Evaluate(t);

                Vector3 nextPos = Vector3.Lerp(_dashStartPos, _dashTargetPos, curvedT);

                ResolveWallsSphere(ref nextPos);
                DamageEnemiesBetween(_lastFramePos, nextPos, dashDir);

                _playerFacade.MovePosition(nextPos);
                _lastFramePos = nextPos;

                yield return null;
            }

            StopDash();
        }

        void StopDash()
        {
            isDashing = false;

            _playerGhostTrailController.StopSpawnGhostTrail();
            _playerStatus.SetInvulnerable(false);
        }

        void ResolveWallsSphere(ref Vector3 targetPos)
        {
            Vector3 dir = (targetPos - _playerFacade.Position).normalized;
            float dist = Vector3.Distance(_playerFacade.Position, targetPos);
            float radius = _playerFacade.CapsuleRadius;

            if (Physics.SphereCast(_playerFacade.Position, radius, dir, out RaycastHit hit, dist, _settings.wallLayer))
            {
                targetPos = _playerFacade.Position + dir * Mathf.Max(0f, hit.distance - _settings.skin);
            }
        }

        Vector3 ComputeDashTargetSafe(Vector3 start, Vector3 dir, float maxDistance)
        {
            float radius = _playerFacade.CapsuleRadius;
            float height = Mathf.Max(_playerFacade.CapsuleHeight, radius * 2f);

            Vector3 origin = start;
            Vector3 targetPoint = start + dir * maxDistance;

            Ray ray = new Ray(origin, dir);
            bool isRayCollidedWithWall = false;

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, _settings.wallLayer))
            {
                isRayCollidedWithWall = true;
                targetPoint = hit.point - dir * radius;
            }

            int steps = 20;
            bool isFinded = false;

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector3 stepPoint = Vector3.Lerp(targetPoint, start, t);

                if (_dashCollisionChecker.IsPositionAvailable(stepPoint, radius, _settings.wallLayer | _settings.obstacleLayer))
                {
                    _coroutineRunner.StartCoroutine(ShowDebugSphere(stepPoint, 3.0f, 0.25f, Color.black));
                    targetPoint = stepPoint;
                    isFinded = true;
                    break;
                }
                else
                {
                    _coroutineRunner.StartCoroutine(ShowDebugSphere(stepPoint, 3.0f, 0.25f, Color.black));
                }
            }

            if (!isFinded)
            {
                targetPoint = _playerFacade.Position;
            }

            if (isRayCollidedWithWall)
            {
                _coroutineRunner.StartCoroutine(ShowDebugSphere(targetPoint, 1f, radius, isFinded ? Color.yellow : Color.red));
            }
            else
            {
                _coroutineRunner.StartCoroutine(ShowDebugSphere(targetPoint, 1f, radius, isFinded ? Color.yellow : Color.blue));
            }

            return targetPoint;
        }

        IEnumerator ShowDebugSphere(Vector3 center, float duration, float radius, Color color, int segments = 12)
        {
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                GizmoHelper.DrawWireSphere(center, radius, color, segments);

                yield return null;
            }
        }

        void DamageEnemiesBetween(Vector3 from, Vector3 to, Vector3 dashDir)
        {
            Vector3 dir = to - from;
            float dist = dir.magnitude;
            if (dist <= 0f) return;

            dir /= dist;
            float radius = _playerFacade.CapsuleRadius;

            RaycastHit[] hits = Physics.SphereCastAll(from, radius, dir, dist, _settings.enemyLayer);

            foreach (var hit in hits)
            {
                if (hitThisDash.Contains(hit.collider)) continue;
                hitThisDash.Add(hit.collider);

                if (hit.collider.TryGetComponent<IDamageable>(out var iDamageable))
                {
                    iDamageable.TakeDamage(_settings.dashDamage);
                }

                if (hit.collider.attachedRigidbody != null)
                {
                    hit.collider.attachedRigidbody.AddForce(dashDir * _settings.dashKnockback, ForceMode.Impulse);
                }

                _timeScaleManager.StartHitStop();
            }
        }

        [Serializable]
        public class Settings
        {
            public GameObject playerDashEffectPrefab;
            public float dashSpeed = 22f;
            public float dashDuration = 0.18f;
            public int maxDashCharges = 3;
            public float dashRechargeTime = 0.6f;
            public AnimationCurve dashCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public float skin = 0.05f;
            public LayerMask wallLayer;
            public LayerMask obstacleLayer;
            public LayerMask enemyLayer;
            public int dashDamage = 10;
            public float dashKnockback = 5f;
            public float dashCameraImpulseForce = 0.45f;
        }
    }
}