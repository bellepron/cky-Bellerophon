using Bellepron.Weapon;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerAnimationEventController : MonoBehaviour
    {
        [Inject] readonly PlayerFacade _playerFacade;

        public void BeginHit()
        {
            if (_playerFacade.Weapon is MeleeWeapon melee)
            {
                melee.BeginHit();
            }
        }

        public void EndHit()
        {
            if (_playerFacade.Weapon is MeleeWeapon melee)
            {
                melee.EndHit();
            }
        }

        public void Shoot()
        {
            if (_playerFacade.Weapon is RangedWeapon ranged)
            {
                ranged.Shoot();
            }
        }
    }
}