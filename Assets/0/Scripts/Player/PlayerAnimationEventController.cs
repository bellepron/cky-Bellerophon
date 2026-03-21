using Bellepron.Weapon;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerAnimationEventController : MonoBehaviour
    {
        [Inject] readonly PlayerFacade _facade;
        [Inject] readonly PlayerAttackController.Settings _attackControllerSettings;
        [Inject] readonly PlayerDamageHandler _damageHandler;

        public void BeginHit()
        {
            if (_facade.Weapon == null) return;

            if (_facade.Weapon is MeleeWeapon melee)
            {
                melee.BeginHit();
            }
        }

        public void EndHit()
        {
            if (_facade.Weapon == null) return;

            if (_facade.Weapon is MeleeWeapon melee)
            {
                melee.EndHit();
            }
        }

        public void SpecialHit()
        {
            if (_facade.Weapon == null) return;

            if (_facade.Weapon is MeleeWeapon melee)
            {
                var meleeSettings = melee.Settings;
                _damageHandler.DamageEnemiesInRadius(_facade.Position, meleeSettings.SpecialRadius, meleeSettings.SpecialDamage, meleeSettings.SpecialKnockback, _attackControllerSettings.enemyLayer);
                melee.SpecialHit();
            }
        }

        public void Cast()
        {

        }

        public void Shoot()
        {
            if (_facade.Weapon == null) return;

            if (_facade.Weapon is RangedWeapon ranged)
            {
                ranged.Shoot();
            }
        }
    }
}