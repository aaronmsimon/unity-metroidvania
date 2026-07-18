using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Health health;

    private void Awake() {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void OnEnable() {
        health.OnDamaged += HandleDamage;
    }

    private void OnDisable() {
        health.OnDamaged -= HandleDamage;
    }

    private void HandleDamage() {
        animator.SetTrigger("isDamaged");
    }
}
