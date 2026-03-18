using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    [CreateAssetMenu(menuName = "Bellepron/Player Settings")]
    public class PlayerSettings : ScriptableObjectInstaller
    {
        [field: SerializeField] public GameObject PlayerPrefab { get; private set; }
        [field: SerializeField] public PlayerMovementController.Settings PlayerMovementControllerSettings { get; private set; }
        [field: SerializeField] public PlayerAnimatorController.Settings PlayerAnimatorControllerSettings { get; private set; }
        [field: SerializeField] public PlayerInteractController.Settings PlayerInteractControllerSettings { get; private set; }
        [field: SerializeField] public PlayerAttackController.Settings PlayerAttackControllerSettings { get; set; }
        [field: SerializeField] public PlayerDashController.Settings PlayerDashControllerSettings { get; set; }
        [field: SerializeField] public PlayerGhostTrailController.Settings PlayerGhostTrailControllerSettings { get; private set; }

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();

            Container.BindInstance(PlayerMovementControllerSettings).IfNotBound();
            Container.BindInstance(PlayerAnimatorControllerSettings).IfNotBound();
            Container.BindInstance(PlayerInteractControllerSettings).IfNotBound();
            Container.BindInstance(PlayerAttackControllerSettings).IfNotBound();
            Container.BindInstance(PlayerDashControllerSettings).IfNotBound();
            Container.BindInstance(PlayerGhostTrailControllerSettings).IfNotBound();
        }
    }
}