using UnityEngine;
using Zenject;

namespace Bellepron
{
    public class PlayerSpawnedSignalObserver
    {
        public void OnTest()
        {
            Debug.Log("<b><color=#FFD700>Controls</color></b>: \n" +
                "<b><color=white>WASD-Movement,</color></b> " +
                "<b><color=white>Q-Special,</color></b> " +
                "<b><color=white>Mouse Left-Attack</color></b>, " +
                "<b><color=white>Dash-Space</color></b>, " +
                "<b><color=white>Interact-E</color></b>");
        }
    }

    public class GameSignalsInstaller : Installer<GameSignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<PlayerSpawnedSignal>();
            Container.DeclareSignal<PlayerWeaponChangedSignal>();
            Container.DeclareSignal<PlayerDashControllerCreatedSignal>();
            Container.DeclareSignal<DashChargeUsedSignal>();
            Container.DeclareSignal<DashChargeRestoredSignal>();
            Container.DeclareSignal<DashRechargeSignal>();
            Container.DeclareSignal<CastProjectileEchoSpawnedSignal>();
            Container.DeclareSignal<EnemySpawnedSignal>();

            Container.BindSignal<PlayerSpawnedSignal>().ToMethod<PlayerSpawnedSignalObserver>(x => x.OnTest).FromNew();
        }
    }
}