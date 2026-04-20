using Bellepron.Enemy;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class CastTargeter : MonoBehaviour
    {
        [Inject] readonly PlayerFacade _facade;
        [Inject] readonly PlayerCastController.Settings _settingsCastController;
        [Inject] readonly PlayerRotationController _rotationController;

        [SerializeField] private GameObject pointer;
        [SerializeField] private Camera mainCamera;

        public bool IsActive { get; private set; }
        public EnemyFacade TargetEnemy { get; private set; }
        public IDamageable Target
        {
            get
            {
                if (TargetEnemy == null) return null;
                TargetEnemy.TryGetComponent<IDamageable>(out var damageable);
                return damageable;
            }
        }

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            Activate(false);

            pointer.transform.position = new Vector3(0, _settingsCastController.pointerHeight, _settingsCastController.pointerStartOffset);
        }

        void SetPointerScale(float z)
        {
            var newZ = z - _settingsCastController.pointerStartOffset;
            newZ = newZ > 0 ? newZ : 0;
            pointer.transform.localScale = new Vector3(1, 1, newZ);
        }

        public void Activate(bool active)
        {
            IsActive = active;
            pointer.SetActive(active);
        }

        private void Update()
        {
            if (!IsActive) return;

            Vector3 mouseDir = GetMouseDirection();

            TargetEnemy = GetBestEnemyByAngle(mouseDir);

            Vector3 lookDir = mouseDir;
            float pointerDistance;

            if (TargetEnemy != null)
            {
                var enemyPos = TargetEnemy.transform.position;
                enemyPos.y = 0;
                var thisPos = transform.position;
                thisPos.y = 0;
                lookDir = (enemyPos - thisPos).normalized;
            }

            pointerDistance = GetPhysicsRayHitDistance(lookDir);

            SetPointerScale(pointerDistance);

            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);

            _rotationController.Rotate(lookDir);
        }

        private float GetPhysicsRayHitDistance(Vector3 dir)
        {
            int mask = _settingsCastController.targetLayer | _settingsCastController.obstacleLayer;

            var rayStartPos = _facade.Position + _facade.transform.up * _settingsCastController.pointerHeight;
            var rayLength = _settingsCastController.pointerMaxDistance + _settingsCastController.pointerStartOffset;
            if (Physics.Raycast(rayStartPos, dir, out RaycastHit hit, rayLength, mask))
            {
                // Debug.Log($"Ray hit: {hit.collider.name} | Layer: {hit.collider.gameObject.layer} | Distance: {hit.distance}");
                return hit.distance;
            }

            return _settingsCastController.pointerMaxDistance;
        }

        private Vector3 GetMouseDirection()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPoint = ray.GetPoint(distance);
                return (worldPoint - transform.position).normalized;
            }

            return transform.forward;
        }

        private EnemyFacade GetBestEnemyByAngle(Vector3 mouseDir)
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                _settingsCastController.pointerMaxDistance,
                _settingsCastController.targetLayer
            );

            EnemyFacade best = null;
            float bestAngle = float.MaxValue;

            foreach (Collider hit in hits)
            {
                EnemyFacade facade = hit.GetComponentInParent<EnemyFacade>();
                if (facade == null) continue;
                if (!facade.IsAlive) continue;

                Vector3 toEnemy = hit.transform.position - transform.position;
                toEnemy.y = 0f;

                float angle = Vector3.Angle(mouseDir, toEnemy.normalized);
                if (angle > _settingsCastController.pointerMaxTargetAngle) continue;

                if (angle >= bestAngle) continue;

                if (HasObstacleBetween(transform.position, hit.transform.position)) continue;

                bestAngle = angle;
                best = facade;
            }

            return best;
        }

        public IDamageable GetBestEnemyByAngle(Transform projectileTransform, float maxTargetFindingAngle)
        {
            Collider[] hits = Physics.OverlapSphere(
               projectileTransform.position,
                _settingsCastController.pointerMaxDistance,
                _settingsCastController.targetLayer
            );

            IDamageable best = null;
            float bestAngle = float.MaxValue;

            foreach (Collider hit in hits)
            {
                EnemyFacade facade = hit.GetComponentInParent<EnemyFacade>();
                if (facade == null) continue;
                if (!facade.IsAlive) continue;

                Vector3 toEnemy = hit.transform.position - transform.position;
                toEnemy.y = 0f;

                float angle = Vector3.Angle(projectileTransform.forward, toEnemy.normalized);
                if (angle > maxTargetFindingAngle) continue;

                if (angle >= bestAngle) continue;

                if (HasObstacleBetween(projectileTransform.position, hit.transform.position)) continue;

                bestAngle = angle;

                if (facade.TryGetComponent<IDamageable>(out var iDamageable))
                    best = iDamageable;
            }

            return best;
        }

        private bool HasObstacleBetween(Vector3 origin, Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - origin;
            float distance = direction.magnitude;

            return Physics.Raycast(origin, direction.normalized, distance, _settingsCastController.obstacleLayer);
        }
    }
}