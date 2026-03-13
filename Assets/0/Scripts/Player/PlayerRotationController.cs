using UnityEngine.InputSystem;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerRotationController
    {
        [Inject] readonly PlayerFacade _playerFacade;
        Camera _mainCam;

        [Inject]
        public void Construct()
        {
            _mainCam = Camera.main;
        }

        public void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            Quaternion targetRot = Quaternion.LookRotation(direction);

            _playerFacade.SetRotation(targetRot);
        }

        public void RotateTowards(Vector3 direction, float rotationSpeed)
        {
            if (direction == Vector3.zero) return;

            Quaternion targetRot = Quaternion.LookRotation(direction);
            Quaternion newRot = Quaternion.Slerp(_playerFacade.Rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);

            _playerFacade.SetRotation(newRot);
        }

        public void RotateToMouse()
        {
            if (!_mainCam)
            {
                Debug.Log("Couldn't find Main Camera!");
                return;
            }

            Ray ray = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, _playerFacade.Position);

            if (plane.Raycast(ray, out float dist))
            {
                Vector3 point = ray.GetPoint(dist);
                Vector3 dir = point - _playerFacade.Position;
                dir.y = 0f;
                if (dir != Vector3.zero)
                    _playerFacade.SetRotation(Quaternion.LookRotation(dir));
            }
        }
    }
}