using System.Collections;
using UnityEngine;
using Zenject;

namespace Bellepron.Player
{
    public class PlayerGhostSpawner
    {
        [Inject] readonly PlayerGhostFacade.Factory _playerGhostFactory;
        [Inject] readonly PlayerFacade _playerFacade;
        [Inject] readonly CoroutineRunner _coroutineRunner;
        [Inject] readonly PlayerGhostTrailController.Settings _settings;

        SkinnedMeshRenderer _smr;

        public void SpawnGhost(Vector3 spawnPos)
        {
            if (_smr == null) _smr = _playerFacade.SkinnedMeshRenderer;

            Mesh bakedMesh = new Mesh();
            _smr.BakeMesh(bakedMesh);

            var ghost = _playerGhostFactory.Create();
            ghost.transform.position = spawnPos;
            ghost.transform.rotation = _smr.transform.rotation;
            ghost.transform.localScale = _smr.transform.lossyScale;

            ghost.MeshFilter.mesh = bakedMesh;
            ghost.MeshRenderer.material = _settings.ghostTrailMaterial;

            _coroutineRunner.StartCoroutine(FadeAndDespawn(ghost));
        }

        IEnumerator FadeAndDespawn(PlayerGhostFacade playerGhostFacade)
        {
            float time = 0f;
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            var renderer = playerGhostFacade.MeshRenderer;

            while (time < _settings.ghostLifeTime)
            {
                float t = time / _settings.ghostLifeTime;
                float alpha = Mathf.Lerp(_settings.ghostColor.a, 0f, t);

                renderer.GetPropertyBlock(mpb);
                mpb.SetColor("_BaseColor", new Color(
                    _settings.ghostColor.r,
                    _settings.ghostColor.g,
                    _settings.ghostColor.b,
                    alpha));
                renderer.SetPropertyBlock(mpb);

                time += Time.deltaTime;
                yield return null;
            }

            playerGhostFacade.Despawn();
        }
    }
}