using UnityEngine;

namespace Bellepron.Weapon
{
    [CreateAssetMenu(menuName = "Bellepron/Weapon/Melee Weapon Settings")]
    public class MeleeWeaponSettings : WeaponSettingsAbstract
    {
        [field: SerializeField] public int SpecialRadius { get; private set; } = 3;
        [field: SerializeField] public int SpecialDamage { get; private set; } = 15;
        [field: SerializeField] public int SpecialKnockback { get; private set; } = 6000000;
    }
}