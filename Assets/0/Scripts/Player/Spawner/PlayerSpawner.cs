using Object = UnityEngine.Object;
using Bellepron.Managers;
using Bellepron.Player;
using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Spawners
{
    public class PlayerSpawner : IInitializable, IDisposable
    {
        [Inject] readonly PlayerFacade.Factory _factory;
        [Inject] readonly PlayerHolder _playerHolder;
        [Inject] readonly CameraManager _cameraManager;
        [Inject] readonly SignalBus _signalBus;
        [Inject] readonly PlayerSettings _playerSettings;

        public void Initialize()
        {
            Spawn(Vector3.zero);

            _signalBus.Subscribe<PlayerWeaponChangedSignal>(OnPlayerWeaponChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerWeaponChangedSignal>(OnPlayerWeaponChanged);
        }

        public void Spawn(Vector3 position)
        {
            PlayerFacade playerFacade = _factory.Create();
            playerFacade.transform.position = position;
            _playerHolder.SetPlayerFacade(playerFacade);

            SetWeapon();

            _cameraManager.SetTarget(playerFacade.transform);
            _signalBus.Fire<PlayerSpawnedSignal>();
        }

        void OnPlayerWeaponChanged(PlayerWeaponChangedSignal signal)
        {
            SaveCurrentWeaponId(signal.weaponId);
            SetWeapon();
        }

        void SaveCurrentWeaponId(int id)
        {
            _playerSettings.SaveCurrentWeaponId(id);
        }

        void SetWeapon()
        {
            PlayerFacade playerFacade = _playerHolder.PlayerFacade;
            var currentWeaponPrefab = _playerSettings.GetCurrentWeapon();
            var weaponHoldPoints =
                playerFacade.
                    GetWeaponHoldPoints(currentWeaponPrefab.SettingsAbtract.WeaponHoldType,
                                        currentWeaponPrefab.SettingsAbtract.WeaponStance);

            if (weaponHoldPoints.Count == 1)
            {
                var weapon = Object.Instantiate(currentWeaponPrefab, weaponHoldPoints[0]);
                playerFacade.SetWeapon(weapon);
            }
            //else if (weaponHoldPoints.Count == 2) // For Dual Wield
            //{
            //    Object.Instantiate(currentWeaponPrefab, weaponHoldPoints[0]);
            //    Object.Instantiate(currentWeaponPrefab, weaponHoldPoints[1]);
            //}
        }
    }
}