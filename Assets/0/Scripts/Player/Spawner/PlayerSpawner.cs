using Bellepron.Player;
using UnityEngine;
using Zenject;

namespace Bellepron.Spawners
{
    public class PlayerSpawner : IInitializable
    {
        [Inject] readonly PlayerFacade.Factory _factory;
        [Inject] readonly PlayerHolder _playerHolder;
        [Inject] readonly SignalBus _signalBus;

        public void Initialize()
        {
            Spawn(Vector3.zero);
        }

        public void Spawn(Vector3 position)
        {
            PlayerFacade playerFacade = _factory.Create();
            playerFacade.transform.position = position;
            _playerHolder.SetPlayerFacade(playerFacade);

            _signalBus.Fire<PlayerSpawnedSignal>();
        }
    }
}