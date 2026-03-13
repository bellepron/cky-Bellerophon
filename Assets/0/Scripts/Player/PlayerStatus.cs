using UnityEngine;

namespace Bellepron.Player
{
    public class PlayerStatus
    {
        public bool IsInvulnerable { get; private set; }
        public bool CanMove { get; private set; } = true;
        public bool CanAttack { get; private set; } = true;

        public void SetInvulnerable(bool value) => IsInvulnerable = value;
        public void SetCanMove(bool value) => CanMove = value;
        public void SetCanAttack(bool value) => CanAttack = value;
    }
}