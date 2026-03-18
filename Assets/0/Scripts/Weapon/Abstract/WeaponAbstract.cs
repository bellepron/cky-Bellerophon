using UnityEditor.Animations;
using Bellepron.Player;
using UnityEngine;

namespace Bellepron.Weapon
{
    public abstract class WeaponAbstract : MonoBehaviour
    {
        [field: SerializeField] public WeaponSettingsAbstract SettingsAbtract { get; private set; }
        public AnimatorController AnimatorController => SettingsAbtract.AnimatorController;
        public PlayerFacade PlayerFacade { get; private set; }
        public PlayerAttackController.Settings PlayerAttackControllerSettings { get; private set; }

        public void SetPlayerFacade(PlayerFacade playerFacade, PlayerAttackController.Settings playerAttackControllerSettings)
        {
            PlayerFacade = playerFacade;
            PlayerAttackControllerSettings = playerAttackControllerSettings;
        }
    }
}