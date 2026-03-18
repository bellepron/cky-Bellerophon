using Bellepron.Player;
using Bellepron.Weapon;
using UnityEngine;

namespace Bellepron.Interactables
{
    public class WeaponChangingArea : MonoBehaviour, IInteractable
    {
        [SerializeField] WeaponTypes weaponType;

        public void Interact(PlayerFacade player)
        {
            Debug.Log($"{gameObject.name} interacted!");
        }
    }
}