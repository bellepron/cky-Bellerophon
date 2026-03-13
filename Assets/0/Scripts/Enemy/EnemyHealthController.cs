using Zenject;

namespace Bellepron.Enemy
{
    public class EnemyHealthController : IInitializable
    {
        [Inject] EnemyFacade _enemyFacade;

        int maxHealth = 100;
        int _currentHealth = 100;

        public void Initialize()
        {

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

                    GetDamage();
                }
                else
                {
                    _currentHealth = 0;

                    Die();
                }
            }
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