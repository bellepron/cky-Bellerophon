using Bellepron.Weapon;
using Bellepron.Cast;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerAttackController
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerFacade _facade;
        [Inject] readonly PlayerAnimatorController _animatorController;
        [Inject] readonly PlayerRotationController _rotationController;
        [Inject] readonly CastProjectile.Factory _defaultCastFactory;
        [Inject] readonly WeaponHolder _weaponHolder;

        public void Attack(int attackStep)
        {
            _rotationController.RotateToMouse();
            _animatorController.PlayAttack(attackStep);
            _facade.SetVelocity(_facade.Forward * _settings.attackForwardForce);

            if (_facade.Weapon != null)
            {
                if (_facade.Weapon is MeleeWeapon melee)
                {
                    melee.attackStep = attackStep;
                }
            }
        }

        public void DashAttack()
        {
            _rotationController.RotateToMouse();
            _animatorController.PlayDashAttack();
        }

        public void Special()
        {
            _rotationController.RotateToMouse();
            _animatorController.PlaySpecial();
        }

        public void TriggerCast()
        {
            _rotationController.RotateToMouse();
            _animatorController.PlayCast();
        }

        public void Cast()
        {
            var castProjectile = _defaultCastFactory.Create(null, _settings.enemyLayer, _facade.gameObject);
            castProjectile.transform.SetPositionAndRotation(_weaponHolder.leftHandCastTransform.position, _facade.Rotation);
        }

        [Serializable]
        public class Settings
        {
            public float attackForwardForce = 3.5f;
            public LayerMask wallLayer;
            public LayerMask obstacleLayer;
            public LayerMask enemyLayer;
            public GameObject defaultCastProjectilePrefab;
            public GameObject castProjectileResiduePrefab;
            public GameObject castProjectileEchoPrefab;
        }
    }
}