using Bellepron.UI;
using UnityEngine;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyHealthController
    {
        [Inject] EnemySettings _settings;
        [Inject] EnemyFacade _enemyFacade;
        [Inject] HealthBarController _healthBarController;

        public bool IsAlive => _currentHealth > 0;

        int _currentHealth = 100;

        public void SetCurrentHealth(int newHealth) => _currentHealth = newHealth;

        public void OnSpawned()
        {
            _currentHealth = _settings.health;
            _healthBarController.OnSpawned(_currentHealth, _settings.health);
        }

        public void ChangeHealth(int delta, GameObject instigator)
        {
            var maxHealth = _settings.health;
            var newHealth = _currentHealth + delta;

            if (delta > 0)
            {
                if (newHealth > maxHealth)
                {
                    _currentHealth = maxHealth;
                }
            }
            else if (delta < 0)
            {
                if (newHealth > 0)
                {
                    _currentHealth = newHealth;
                    _healthBarController.UpdateValue(_currentHealth, maxHealth);

                    GetDamage();
                }
                else
                {
                    _currentHealth = 0;

                    Die();
                }
            }

            _healthBarController.UpdateValue(_currentHealth, maxHealth);
        }

        void GetDamage()
        {

        }

        void Die()
        {
            _enemyFacade.Despawn();
        }
    }
}