using System.Collections;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerGhostTrailController
    {
        [Inject] readonly CoroutineRunner _coroutineRunner;
        [Inject] readonly PlayerGhostTrailController.Settings _settings;
        [Inject] readonly PlayerGhostSpawner _ghostSpawner;

        SkinnedMeshRenderer _smr;
        Coroutine _ghostRoutine;

        [Inject]
        public void Construct(PlayerFacade playerFacade)
        {
            _smr = playerFacade.SkinnedMeshRenderer;
        }

        public void StartSpawnGhostTrail()
        {
            _ghostRoutine = _coroutineRunner.StartCoroutine(SpawnGhostTrail());
        }

        public void StopSpawnGhostTrail()
        {
            if (_ghostRoutine != null)
            {
                _coroutineRunner.StopRoutine(_ghostRoutine);
                _ghostRoutine = null;
            }
        }

        IEnumerator SpawnGhostTrail()
        {
            if (!_smr || !_settings.ghostTrailMaterial) yield break;

            Vector3 lastSpawnPosition = _smr.transform.position;

            while (true)
            {
                float distance = Vector3.Distance(_smr.transform.position, lastSpawnPosition);

                while (distance >= _settings.ghostSpawnDistance)
                {
                    lastSpawnPosition += (_smr.transform.position - lastSpawnPosition).normalized * _settings.ghostSpawnDistance;
                    _ghostSpawner.SpawnGhost(lastSpawnPosition);
                    distance -= _settings.ghostSpawnDistance;
                }

                yield return null;
            }
        }

        [System.Serializable]
        public class Settings
        {
            public GameObject playerGhostPrefab;
            public Material ghostTrailMaterial;
            public float ghostLifeTime = 0.2f;
            public float ghostSpawnDistance = 0.5f;
            public Color ghostColor = new Color(0.3f, 0.8f, 1f, 0.8f);
        }
    }
}