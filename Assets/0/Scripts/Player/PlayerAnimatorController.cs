using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerAnimatorController : IInitializable
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerFacade _playerFacade;
        Animator _animator;

        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int MovementHash = Animator.StringToHash("Movement");
        private static readonly int DashHash = Animator.StringToHash("Dash");

        private static readonly string AttackBase = "Attack";
        private static readonly int AttackTagHash = Animator.StringToHash("Attack");
        private static readonly int DashAttackHash = Animator.StringToHash("DashAttack");

        public void Initialize()
        {
            _animator = _playerFacade.Animator;
        }

        public void SetAnimatorController()
        {
            if (_playerFacade.Weapon != null && _playerFacade.Weapon.AnimatorController != null)
            {
                _animator.runtimeAnimatorController = _playerFacade.Weapon.AnimatorController;
            }
        }

        public void PlayAttack(int attackIndex)
        {
            _animator.CrossFade($"{AttackBase}{attackIndex}", _settings.attackFadeDuration);
        }

        public void PlayDashAttack()
        {
            _animator.CrossFade(DashAttackHash, _settings.dashAttackFadeDuration);
        }

        public void CrossMovement()
        {
            if (_animator == null) return; _animator.CrossFade(MovementHash, _settings.movementFadeDuration);
        }

        public void AnimateMovement(float moveSpeed)
        {
            _animator.SetFloat(MoveSpeedHash, moveSpeed);
        }

        public void AnimateDash()
        {
            _animator.CrossFade(DashHash, _settings.dashFadeDuration);
        }

        public float AttackFadeDuration => _settings.attackFadeDuration;
        public float DashAttackFadeDuration => _settings.dashAttackFadeDuration;

        public bool IsPlayingAttack() => _animator.GetCurrentAnimatorStateInfo(0).tagHash == AttackTagHash;

        public float GetCurrentAttackNormalizedTime() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        [Serializable]
        public class Settings
        {
            public float attackFadeDuration = 0.05f;
            public float dashAttackFadeDuration = 0.05f;
            public float movementFadeDuration = 0.1f;
            public float dashFadeDuration = 0.1f;
        }
    }
}