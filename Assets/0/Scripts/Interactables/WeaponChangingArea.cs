using Bellepron.Spawners;
using Bellepron.Player;
using Bellepron.Weapon;
using UnityEngine;
using Zenject;

namespace Bellepron.Interactables
{
    public class WeaponChangingArea : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public WeaponAbstract Weapon { get; private set; }

        [Inject] readonly PlayerSpawner _playerSpawner;
        [Inject] readonly PlayerHolder _playerHolder;
        [Inject] readonly PlayerSettings _playerSettings;
        [Inject] readonly SignalBus _signalBus;

        private void Awake()
        {
            gameObject.name = $"{gameObject.name} - {Weapon.SettingsAbtract.WeaponType}";
        }

        private void Start()
        {
            _signalBus.Subscribe<PlayerWeaponChangedSignal>(OnPlayerWeaponChanged);

            if (HasPlayerSameWeapon())
            {
                Weapon.gameObject.SetActive(false);
            }
            else
            {
                Weapon.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PlayerWeaponChangedSignal>(OnPlayerWeaponChanged);
        }

        public void Interact()
        {
            Debug.Log($"{gameObject.name} interacted!");

            if (HasPlayerSameWeapon()) return;

            if (!Weapon.gameObject.activeSelf) return;

            Weapon.gameObject.SetActive(false);

            _signalBus.Fire(new PlayerWeaponChangedSignal
            {
                weaponId = Weapon.SettingsAbtract.Id,
                weaponType = Weapon.SettingsAbtract.WeaponType
            });
        }

        void OnPlayerWeaponChanged(PlayerWeaponChangedSignal signal)
        {
            if (signal.weaponType == Weapon.SettingsAbtract.WeaponType && Weapon.gameObject.activeSelf)
            {
                Weapon.gameObject.SetActive(false);
            }
            if (signal.weaponType != Weapon.SettingsAbtract.WeaponType && !Weapon.gameObject.activeSelf)
            {
                Weapon.gameObject.SetActive(true);
            }
        }

        bool HasPlayerSameWeapon()
        {
            if (_playerHolder.PlayerFacade.Weapon != null)
            {
                if (_playerHolder.PlayerFacade.Weapon.SettingsAbtract.WeaponType == Weapon.SettingsAbtract.WeaponType)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}