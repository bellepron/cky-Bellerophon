using UnityEditor.Animations;
using UnityEngine;
using Bellepron.Player;
using Zenject;

namespace Bellepron.Weapon
{
    public abstract class WeaponAbstract : MonoBehaviour
    {
        [field: SerializeField] public WeaponSettingsAbstract SettingsAbstract { get; private set; }
        public AnimatorController AnimatorController => SettingsAbstract.AnimatorController;
        public PlayerFacade PlayerFacade { get; private set; }
        [Inject] protected readonly PlayerAttackController.Settings playerAttackControllerSettings;

        public void SetPlayerFacade(PlayerFacade playerFacade) => PlayerFacade = playerFacade;
    }
}