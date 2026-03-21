using UnityEngine;

public interface IDamageable
{
    Transform Transform { get; }
    bool IsAlive { get; }
    void TakeDamage(int amount, GameObject instigator);
}