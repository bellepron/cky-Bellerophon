using UnityEngine;
using Bellepron.Weapon;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerAnimationEventController : MonoBehaviour
    {
        [Inject] readonly PlayerFacade _playerFacade;

        public void BeginHit()
        {
            if (_playerFacade.Weapon is MeleeWeaponAbstract melee)
            {
                melee.BeginHit();
            }
        }

        public void EndHit()
        {
            if (_playerFacade.Weapon is MeleeWeaponAbstract melee)
            {
                melee.EndHit();
            }
        }

        public void Shoot()
        {
            if (_playerFacade.Weapon is RangedWeaponAbstract ranged)
            {
                ranged.Shoot();
            }
        }
    }
}