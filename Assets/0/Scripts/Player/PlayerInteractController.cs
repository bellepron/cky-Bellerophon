using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerInteractController : IFixedTickable
    {
        [Inject] Settings _settings;
        [Inject] PlayerFacade _playerFacade;
        [Inject] PlayerInputHandler _playerInputHandler;

        IInteractable _currentInteractable;

        public void TryInteract()
        {
            _currentInteractable?.Interact();
        }

        public void FixedTick()
        {
            Detect();
        }

        void Detect()
        {
            _currentInteractable = null;

            Ray ray = new Ray(_playerFacade.Position + Vector3.up * _settings.offsetY, _playerFacade.Forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _settings.interactDistance, _settings.interactLayer))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    _currentInteractable = interactable;
                }
            }
        }

        [System.Serializable]
        public class Settings
        {
            public float interactDistance = 1.0f;
            public float offsetY = 0.1f;
            public LayerMask interactLayer;
        }
    }
}