using Bellepron.Weapon;
using Bellepron.Enemy;
using Zenject;
using Bellepron.Cast;

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

    #region Cast Signals

    public struct CastProjectileEchoSpawnedSignal
    {
        public CastProjectileEcho castProjectileEcho;
        public IMemoryPool pool;
    }

    #endregion

    public struct EnemySpawnedSignal
    {
        public EnemyType spawnedEnemyType;
    }
}