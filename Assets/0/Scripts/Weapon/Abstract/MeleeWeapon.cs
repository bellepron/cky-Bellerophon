using System.Collections.Generic;
using Bellepron.Player;
using UnityEngine;

namespace Bellepron.Weapon
{
    public class MeleeWeapon : WeaponAbstract
    {
        MeleeWeaponSettings _settings;
        public MeleeWeaponSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = SettingsAbstract as MeleeWeaponSettings;

                return _settings;
            }
        }

        [SerializeField] protected Transform[] hitPoints;
        [SerializeField] protected float radius = 0.35f;
        [SerializeField] protected ParticleSystem[] trails;

        bool hitActive;
        HashSet<IDamageable> hitEnemies = new();

        public int attackStep;

        void Awake()
        {
            OpenTrail(false);
        }

        protected virtual void Update()
        {
            if (PlayerFacade == null) return;

            bool isDashAttacking = PlayerFacade.State == Bellepron.Player.States.DashAttack;
            bool isAttacking = PlayerFacade.State == Bellepron.Player.States.Attack
                 || isDashAttacking;

            if (hitActive && !isAttacking)
            {
                EndHit();
                return;
            }

            if (!hitActive) return;

            foreach (var point in hitPoints)
            {
                if (point == null) continue;

                // OverlapSphere ile hitpoint etrafındaki enemyleri bul
                Collider[] hits = Physics.OverlapSphere(point.position, radius, playerAttackControllerSettings.enemyLayer);

                foreach (var hit in hits)
                {
                    if (!hit.TryGetComponent(out IDamageable iDamageable))
                        continue;

                    // Aynı enemyyi bir attack içinde tekrar vurma
                    if (hitEnemies.Contains(iDamageable))
                        continue;

                    // Line-of-sight kontrolü
                    if (IsBlocked(hit))
                        continue;

                    // Damage uygula
                    hitEnemies.Add(iDamageable);
                    iDamageable.TakeDamage(Settings.Damage[attackStep - 1]);
                    var dir = (hit.transform.position - PlayerFacade.Position).normalized;
                    if (hit != null && hit.attachedRigidbody != null)
                    {
                        var knockbackForce = dir * (isDashAttacking ? Settings.DashKnockback : Settings.Knockback[attackStep - 1]);
                        hit.attachedRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
                    }
                }
            }
        }

        bool IsBlocked(Collider enemy)
        {
            Vector3 origin = transform.position;
            Vector3 target = enemy.ClosestPoint(origin);

            // Linecast ile player → enemy arasında obstacle kontrolü
            return Physics.Linecast(origin, target, playerAttackControllerSettings.wallLayer | playerAttackControllerSettings.obstacleLayer);
        }

        public virtual void BeginHit()
        {
            hitEnemies.Clear();
            hitActive = true;
            OpenTrail(true);
        }

        public virtual void EndHit()
        {
            hitActive = false;
            OpenTrail(false);
        }

        void OpenTrail(bool open)
        {
            if (open)
            {
                foreach (ParticleSystem trail in trails)
                {
                    trail.Play();
                }
            }
            else
            {
                foreach (ParticleSystem trail in trails)
                {
                    trail.Stop();
                }
            }
        }

        void OnDrawGizmos()
        {
            if (hitPoints == null) return;

            Gizmos.color = Color.black;
            foreach (var p in hitPoints)
            {
                if (p != null)
                    Gizmos.DrawWireSphere(p.position, radius);
            }
        }
    }
}