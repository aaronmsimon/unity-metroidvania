using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action OnDamaged;
    public event Action OnDeath;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    private void Start() {
        health = MaxHealth;
    }

    public void ChangeHealth(int amount) {
        health += amount;

        if (health > MaxHealth) {
            health = MaxHealth;
        } else if (health <= 0) {
            OnDeath?.Invoke();
        } else if (amount < 0) {
            OnDamaged?.Invoke();
        }
    }

    public int CurrentHealth => health;
    public int MaxHealth => maxHealth;
}
