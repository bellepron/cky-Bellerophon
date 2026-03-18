using Bellepron.Weapon;
using Bellepron.Enemy;

namespace Bellepron
{
    public struct PlayerSpawnedSignal
    {
    }

    public struct PlayerWeaponChangedSignal
    {
        public int weaponId;
        public WeaponType weaponType;
    }

    public struct EnemySpawnedSignal
    {
        public EnemyType spawnedEnemyType;
    }

    #region Dash Signals

    public struct PlayerDashControllerCreatedSignal
    {
    }

    public struct DashChargeUsedSignal
    {
        public int currentDashCharges;
    }

    public struct DashChargeRestoredSignal
    {
        public int currentDashCharges;
    }

    public struct DashRechargeSignal
    {
        public int currentDash;
        public float progress;
    }

    #endregion
}