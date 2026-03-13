using UnityEngine;
using Zenject;
using System;

namespace Bellepron.Player
{
    public class PlayerMovementController
    {
        [Inject] readonly Settings _settings;
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly PlayerInputHandler _playerInputHandler;
        [Inject] readonly PlayerAnimatorController _playerAnimatorController;
        [Inject] readonly PlayerRotationController _playerRotationController;

        public void ApplyMovement()
        {
            var moveDir = _playerInputHandler.Get_MovementVectorSnapped;
            var vel = moveDir * _settings.moveSpeed;

            _playerFacade.SetVelocity(new Vector3(vel.x, _playerFacade.Velocity.y, vel.z));

            _playerRotationController.Rotate(moveDir);

            _playerAnimatorController.AnimateMovement(_playerInputHandler.Get_MovementVectorSnapped.magnitude);
        }

        [Serializable]
        public class Settings
        {
            public float moveSpeed = 9.0f;
        }
    }
}