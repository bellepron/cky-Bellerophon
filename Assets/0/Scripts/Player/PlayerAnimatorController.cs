using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerAnimatorController : IInitializable
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerFacade _facade;
        Animator _animator;

        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int MovementHash = Animator.StringToHash("Movement");
        private static readonly int DashHash = Animator.StringToHash("Dash");
        private static readonly int SpecialHash = Animator.StringToHash("Special");
        private static readonly int CastHash = Animator.StringToHash("Cast");

        private static readonly string AttackBase = "Attack";
        private static readonly int AttackTagHash = Animator.StringToHash("Attack");
        private static readonly int DashAttackHash = Animator.StringToHash("DashAttack");

        public void Initialize()
        {
            _animator = _facade.Animator;
        }

        public void SetAnimatorController()
        {
            if (_facade.Weapon != null && _facade.Weapon.AnimatorController != null)
            {
                _animator.runtimeAnimatorController = _facade.Weapon.AnimatorController;
            }
        }

        public void PlayAttack(int attackIndex)
        {
            _animator.CrossFade($"{AttackBase}{attackIndex}", _settings.attackFadeDuration);
        }

        public void PlaySpecial()
        {
            _animator.CrossFade(SpecialHash, _settings.specialFadeDuration);
        }

        public void PlayCast()
        {
            _animator.CrossFade(CastHash, _settings.castFadeDuration);
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
        public float SpecialFadeDuration => _settings.specialFadeDuration;
        public float CastFadeDuration => _settings.castFadeDuration;
        public float DashAttackFadeDuration => _settings.dashAttackFadeDuration;

        public bool IsPlayingAttack() => _animator.GetCurrentAnimatorStateInfo(0).tagHash == AttackTagHash;
        public bool IsPlayingSpecial() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == SpecialHash;
        public bool IsPlayingCast() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == CastHash;

        public float GetNormalizedTime() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        [Serializable]
        public class Settings
        {
            public float attackFadeDuration = 0.05f;
            public float specialFadeDuration = 0.05f;
            public float castFadeDuration = 0.05f;
            public float dashAttackFadeDuration = 0.05f;
            public float movementFadeDuration = 0.1f;
            public float dashFadeDuration = 0.1f;
        }
    }
}