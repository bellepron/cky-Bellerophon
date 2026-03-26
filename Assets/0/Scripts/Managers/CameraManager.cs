using Unity.Cinemachine;
using UnityEngine;

namespace Bellepron.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] CinemachineCamera defaultCam;

        private CinemachineCamera activeCam;

        public void SetTarget(Transform target)
        {
            defaultCam.Follow = target;
        }

        public void SwitchCamera(CinemachineCamera newCam)
        {
            if (activeCam != null)
                activeCam.Priority = 10;

            newCam.Priority = 15;
            activeCam = newCam;
        }

        public void ResetToDefault()
        {
            SwitchCamera(defaultCam);
        }
    }
}