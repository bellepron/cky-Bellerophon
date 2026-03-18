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
        [Inject] readonly PlayerDamageHandler _playerDamageHandler;
        [Inject] readonly TimeScaleManager _timeScaleManager;

        int _dashCharges;
        float _rechargeTimer;
        Vector3 _dashStartPos;
        Vector3 _lastFramePos;
        HashSet<Collider> _hitThisDash = new HashSet<Collider>();
        DashCollisionChecker _dashCollisionChecker;

        float _totalDeltaDistance;
        Vector3 _projectedDir;

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
            return _dashCharges > 0 && !isDashing;
        }

        void StartDash()
        {
            _playerDamageHandler.ClearHitRegistry();

            var dashDir = _playerFacade.Forward;

            _playerStatus.SetInvulnerable(true);
            _cinemachineImpulseSource.GenerateImpulse(_settings.dashCameraImpulseForce);
            _playerGhostTrailController.StartSpawnGhostTrail();
            _playerAnimatorController.AnimateDash();
            _playerDashEffectFactory.Create(_playerFacade.Position, dashDir);

            _dashCharges--;
            _rechargeTimer = _settings.dashRechargeTime;

            isDashing = true;

            _dashStartPos = _playerFacade.Position;

            // --- 1. hedefi hesapla ---
            Vector3 firstTarget = ComputeDashTargetSafe(_dashStartPos, dashDir, _settings.dashDistance);
            float firstSegmentDistance = _totalDeltaDistance; // ComputeDashTargetSafe içinde set edilir

            // --- Duvara çarptıysa 2. hedefi hesapla ---
            bool hasSecondSegment = false;
            Vector3 secondTarget = firstTarget;
            float secondSegmentDistance = 0f;

            float remainingDistance = _settings.dashDistance - firstSegmentDistance;
            if (remainingDistance > 0.05f && _projectedDir.sqrMagnitude > 0.001f)
            {
                Vector3 slideDir = _projectedDir.normalized;
                secondTarget = ComputeDashTargetSafe(firstTarget, slideDir, remainingDistance);
                secondSegmentDistance = _totalDeltaDistance;
                hasSecondSegment = secondSegmentDistance > 0.05f;
            }

            _lastFramePos = _dashStartPos;
            _hitThisDash.Clear();

            _coroutineRunner.StartCoroutine(
                DashRoutine(dashDir, firstTarget, firstSegmentDistance, hasSecondSegment, secondTarget, secondSegmentDistance)
            );

            _signalBus.Fire(new DashChargeUsedSignal
            {
                currentDashCharges = _dashCharges
            });
        }

        IEnumerator DashRoutine(
            Vector3 dashDir,
            Vector3 firstTarget,
            float firstDistance,
            bool hasSecondSegment,
            Vector3 secondTarget,
            float secondDistance)
        {
            // --- 1. Segment ---
            float firstDuration = firstDistance / _settings.dashSpeed;

            if (firstDuration > 0f)
            {
                float segTimer = 0f;
                Vector3 segStart = _playerFacade.Position;

                while (segTimer < firstDuration)
                {
                    segTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(segTimer / firstDuration);
                    float curvedT = _settings.dashCurve.Evaluate(t);

                    Vector3 nextPos = Vector3.Lerp(segStart, firstTarget, curvedT);

                    // ResolveWallsSphere(ref nextPos, _lastFramePos);
                    _playerDamageHandler.DamageEnemiesBetween(_lastFramePos, nextPos, dashDir, _settings.dashDamage, _settings.dashKnockback, _settings.enemyLayer);

                    _playerFacade.MovePosition(nextPos);
                    _lastFramePos = nextPos;

                    yield return null;
                }

                _playerFacade.MovePosition(firstTarget);
                _lastFramePos = firstTarget;
            }

            // --- 2. Segment (wall slide) ---
            if (hasSecondSegment)
            {
                float secondDuration = secondDistance / _settings.dashSpeed;
                Vector3 slideDir = (secondTarget - firstTarget).normalized;

                if (secondDuration > 0f)
                {
                    float segTimer = 0f;
                    Vector3 segStart = firstTarget;

                    while (segTimer < secondDuration)
                    {
                        segTimer += Time.deltaTime;
                        float t = Mathf.Clamp01(segTimer / secondDuration);
                        float curvedT = _settings.dashCurve.Evaluate(t);

                        Vector3 nextPos = Vector3.Lerp(segStart, secondTarget, curvedT);

                        // ResolveWallsSphere(ref nextPos, _lastFramePos);
                        _playerDamageHandler.DamageEnemiesBetween(_lastFramePos, nextPos, dashDir, _settings.dashDamage, _settings.dashKnockback, _settings.enemyLayer);

                        _playerFacade.MovePosition(nextPos);
                        _lastFramePos = nextPos;

                        yield return null;
                    }

                    _playerFacade.MovePosition(secondTarget);
                    _lastFramePos = secondTarget;
                }
            }

            StopDash();
        }

        void StopDash()
        {
            isDashing = false;
            _playerGhostTrailController.StopSpawnGhostTrail();
            _playerStatus.SetInvulnerable(false);
        }

        // Use if walls are moving.
        void ResolveWallsSphere(ref Vector3 nextPos, Vector3 fromPos)
        {
            Vector3 dir = (nextPos - fromPos);
            float dist = dir.magnitude;
            if (dist <= 0f) return;

            dir /= dist;
            float radius = _playerFacade.CapsuleRadius;

            if (Physics.SphereCast(fromPos, radius, dir, out RaycastHit hit, dist, _settings.wallLayer | _settings.obstacleLayer))
            {
                nextPos = fromPos + dir * Mathf.Max(0f, hit.distance - _settings.skin);
            }
        }

        Vector3 ComputeDashTargetSafe(Vector3 start, Vector3 dir, float maxDistance)
        {
            var radius = _playerFacade.CapsuleRadius;

            Vector3 origin = start;
            Vector3 targetPoint = start + dir * maxDistance;

            Ray ray = new Ray(origin, dir);
            RaycastHit hit = default;

            if (Physics.Raycast(ray, out hit, maxDistance, _settings.wallLayer))
            {
                targetPoint = hit.point - dir * radius;
                _projectedDir = Vector3.ProjectOnPlane(dir, hit.normal);
            }

            int steps = 20;
            // if (isRayCollidedWithWall)
            // {
            //     steps = (Mathf.RoundToInt(Vector3.Distance(start, targetPoint)) + 1) * 2;
            // }
            bool isFinded = false;

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector3 stepPoint = Vector3.Lerp(targetPoint, start, t);

                if (_dashCollisionChecker.IsPositionAvailable(stepPoint, radius, _settings.wallLayer | _settings.obstacleLayer))
                {
                    _coroutineRunner.StartCoroutine(ShowDebugSphere(stepPoint, 3.0f, radius, Color.black));

                    targetPoint = stepPoint;
                    isFinded = true;
                    break;
                }
                else
                {
                    _coroutineRunner.StartCoroutine(ShowDebugSphere(stepPoint, 3.0f, radius, Color.black));
                }
            }

            if (!isFinded)
            {
                if (Physics.Raycast(ray, out hit, maxDistance, _settings.wallLayer | _settings.obstacleLayer))
                {
                    _projectedDir = Vector3.ProjectOnPlane(dir, hit.normal);
                }

                targetPoint = start;
            }

            _totalDeltaDistance = Vector3.Distance(start, targetPoint);

            _coroutineRunner.StartCoroutine(ShowDebugSphere(targetPoint, 1f, radius, isFinded ? Color.yellow : Color.blue));

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

        [Serializable]
        public class Settings
        {
            public GameObject playerDashEffectPrefab;
            public float dashSpeed = 22f;
            public float dashDistance = 4f;
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