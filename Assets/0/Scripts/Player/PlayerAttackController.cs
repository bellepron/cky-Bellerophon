using UnityEngine;
using Bellepron.Weapon;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerAttackController
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly PlayerAnimatorController _playerAnimatorController;
        [Inject] readonly PlayerRotationController _playerRotationController;
        [Inject] readonly TimeScaleManager _timeScaleManager;

        public void Attack(int attackStep)
        {
            _playerRotationController.RotateToMouse();
            _playerAnimatorController.PlayAttack(attackStep);
            _playerFacade.SetVelocity(_playerFacade.Forward * _settings.attackForwardForce);

            if (_playerFacade.Weapon != null)
            {
                if (_playerFacade.Weapon is MeleeWeaponAbstract melee)
                {
                    melee.attackStep = attackStep;
                }
            }
        }

        public void DashAttack()
        {
            _playerRotationController.RotateToMouse();
            _playerAnimatorController.PlayDashAttack();
        }

        [Serializable]
        public class Settings
        {
            public float attackForwardForce = 3.5f;
            public LayerMask wallLayer;
            public LayerMask obstacleLayer;
            public LayerMask enemyLayer;
        }
    }
}