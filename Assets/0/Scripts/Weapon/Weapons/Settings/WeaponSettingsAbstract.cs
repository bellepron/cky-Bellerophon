using UnityEditor.Animations;
using UnityEngine;

namespace Bellepron.Weapon
{
    public class WeaponSettingsAbstract : ScriptableObject
    {
        [field: SerializeField] public WeaponTypes WeaponType;
        [field: SerializeField] public AnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public int[] Damage { get; private set; } = new int[] { 10, 15, 25 };
        [field: SerializeField] public int[] Knockback { get; private set; } = new int[] { 5000000, 5000000, 6000000 };
        [field: SerializeField] public int DashKnockback { get; private set; } = 6000000;
    }
}