using Bellepron.Weapon;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    [CreateAssetMenu(menuName = "Bellepron/Player Settings")]
    public class PlayerSettings : ScriptableObjectInstaller
    {
        [field: SerializeField] public GameObject PlayerPrefab { get; private set; }
        [field: SerializeField] public WeaponAbstract[] Weapons { get; private set; }
        [field: SerializeField] public PlayerMovementController.Settings PlayerMovementControllerSettings { get; private set; }
        [field: SerializeField] public PlayerAnimatorController.Settings PlayerAnimatorControllerSettings { get; private set; }
        [field: SerializeField] public PlayerInteractController.Settings PlayerInteractControllerSettings { get; private set; }
        [field: SerializeField] public PlayerAttackController.Settings PlayerAttackControllerSettings { get; set; }
        [field: SerializeField] public PlayerDashController.Settings PlayerDashControllerSettings { get; set; }
        [field: SerializeField] public PlayerGhostTrailController.Settings PlayerGhostTrailControllerSettings { get; private set; }

        public void SaveCurrentWeaponId(int id)
        {
            PlayerPrefs.SetInt("WeaponID", id);
        }

        public WeaponAbstract GetCurrentWeapon()
        {
            int savedId = PlayerPrefs.GetInt("WeaponID", 0);

            if (Weapons == null || Weapons.Length == 0)
            {
                Debug.LogWarning("PlayerSettings: Weapons array is empty!");
                return null;
            }

            foreach (var weapon in Weapons)
            {
                if (weapon.SettingsAbtract.Id == savedId)
                    return weapon;
            }

            Debug.LogWarning($"PlayerSettings: Weapon with ID {savedId} not found. Using default weapon.");
            return Weapons[0];
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();

            Container.BindInstance(PlayerMovementControllerSettings).IfNotBound();
            Container.BindInstance(PlayerAnimatorControllerSettings).IfNotBound();
            Container.BindInstance(PlayerInteractControllerSettings).IfNotBound();
            Container.BindInstance(PlayerAttackControllerSettings).IfNotBound();
            Container.BindInstance(PlayerDashControllerSettings).IfNotBound();
            Container.BindInstance(PlayerGhostTrailControllerSettings).IfNotBound();
        }
    }
}