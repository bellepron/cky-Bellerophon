using UnityEngine;
using Bellepron.Weapon;

namespace Bellepron.Weapon
{
    public abstract class RangedWeaponAbstract : WeaponAbstract
    {
        RangedWeaponSettings _settings;
        public RangedWeaponSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = SettingsAbstract as RangedWeaponSettings;

                return _settings;
            }
        }

        public virtual void Shoot()
        {

        }
    }
}