using Bellepron.Weapon;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerFacade : MonoBehaviour
    {
        [field: SerializeField] public States State { get; set; }
        [field: SerializeField] public WeaponAbstract Weapon { get; private set; }

        [Inject] readonly PlayerInputHandler _playerInputHandler;
        [Inject] readonly PlayerAnimatorController _playerAnimatorController;

        [Space(10)]
        [SerializeField] Animator animator;
        [SerializeField] CapsuleCollider capsuleCollider;
        [SerializeField] Rigidbody rb;
        [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

        public Animator Animator => animator;
        public SkinnedMeshRenderer SkinnedMeshRenderer => skinnedMeshRenderer;

        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Position => rb.position;
        public Quaternion Rotation => rb.rotation;
        public Vector3 Velocity => rb.linearVelocity;
        public float CapsuleRadius => capsuleCollider.radius;
        public float CapsuleHeight => capsuleCollider.height;

        private void Awake()
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (Weapon != null) SetWeapon(Weapon);
        }

        public void SetWeapon(WeaponAbstract weapon)
        {
            Weapon = weapon;
            Weapon.SetPlayerFacade(this);
            _playerAnimatorController.SetAnimatorController();
        }

        public void MovePosition(Vector3 nextPos)
        {
            rb.MovePosition(nextPos);
        }

        public void SetRotation(Quaternion newRotation)
        {
            rb.MoveRotation(newRotation);
        }

        public void SetVelocity(Vector3 newVelocity)
        {
            rb.linearVelocity = newVelocity;
        }

        void OnValidate()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            if (!capsuleCollider) capsuleCollider = GetComponent<CapsuleCollider>();
        }

        public class Factory : PlaceholderFactory<PlayerFacade>
        {

        }
    }
}