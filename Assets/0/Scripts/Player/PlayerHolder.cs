using UnityEngine;

namespace Bellepron.Player
{
    public class PlayerHolder
    {
        public PlayerFacade PlayerFacade { get; private set; }

        public void SetPlayerFacade(PlayerFacade playerFacade)
        {
            PlayerFacade = playerFacade;
        }
    }
}