using UnityEditor.Animations;
using Bellepron.Player;
using UnityEngine;
using Zenject;

namespace Bellepron.Weapon
{
    public abstract class WeaponAbstract : MonoBehaviour
    {
        [field: SerializeField] public WeaponSettingsAbstract SettingsAbstract { get; private set; }
        public AnimatorController AnimatorController => SettingsAbstract.AnimatorController;
        [Inject] protected readonly PlayerAttackController.Settings playerAttackControllerSettings;
        public PlayerFacade PlayerFacade { get; private set; }

        public void SetPlayerFacade(PlayerFacade playerFacade) => PlayerFacade = playerFacade;
    }
}