using Bellepron.UI;
using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyHealthController : IInitializable
    {
        [Inject] EnemyFacade _enemyFacade;
        [Inject] HealthBarController _healthBarController;

        int maxHealth = 100;
        int _currentHealth = 100;

        public void Initialize()
        {
            _healthBarController.Initialize(_currentHealth, maxHealth);
        }

        public void ChangeHealth(int delta)
        {
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