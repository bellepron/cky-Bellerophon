using UnityEngine;
using Zenject;

namespace Bellepron
{
    [CreateAssetMenu(menuName = "Bellepron/Game Settings")]
    public class GameSettings : ScriptableObjectInstaller
    {
        public GameInstaller.Settings GameInstallerSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(GameInstallerSettings).IfNotBound();
        }
    }
}