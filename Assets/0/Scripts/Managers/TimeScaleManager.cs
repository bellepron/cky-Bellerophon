using System.Collections;
using UnityEngine;
using Zenject;

namespace Bellepron
{
    public class TimeScaleManager
    {
        [Inject] CoroutineRunner _coroutineRunner;

        public void StartHitStop()
        {
            _coroutineRunner.StartRoutine(HitStop());
        }
        IEnumerator HitStop()
        {
            Time.timeScale = 0.05f;
            yield return new WaitForSecondsRealtime(0.03f);
            Time.timeScale = 1f;
        }
    }
}