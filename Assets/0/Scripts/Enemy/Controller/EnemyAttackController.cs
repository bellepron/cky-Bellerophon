using System.Collections;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyAttackController
    {
        [Inject] readonly EnemySettings _settings;
        [Inject] readonly EnemyFacade _facade;

        public bool IsInRange(Vector3 target)
        {
            Vector3 toTarget = target - _facade.Position;
            return toTarget.sqrMagnitude <= _settings.attackableDistance * _settings.attackableDistance;
        }

        // TEMP
        [Inject] CoroutineRunner _coroutineRunner;
        bool _isAttacking;
        public bool IsAttacking => _isAttacking;
        public void AttakTEMP(float duration)
        {
            _coroutineRunner.StartCoroutine(AttackRoutine(duration));
        }
        IEnumerator AttackRoutine(float duration)
        {
            _isAttacking = true;
            yield return new WaitForSeconds(duration);
            _isAttacking = false;
        }
    }
}