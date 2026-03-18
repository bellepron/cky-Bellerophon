using UnityEngine;

namespace Bellepron.Weapon
{
    public class RangedWeapon : WeaponAbstract
    {
        RangedWeaponSettings _settings;
        public RangedWeaponSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = base.SettingsAbtract as RangedWeaponSettings;

                return _settings;
            }
        }

        public virtual void Shoot()
        {

        }
    }
}