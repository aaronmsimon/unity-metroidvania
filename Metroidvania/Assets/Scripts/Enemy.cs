using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Death FX")]
    [SerializeField] private GameObject[] deathParts;
    [SerializeField] private float spawnForce = 5;
    [SerializeField] private float spawnTorque = 5;
    [SerializeField] private float partLifetime = 2;
    [SerializeField] private Vector2 randomRot = new Vector2(0.5f, 1);
    [SerializeField] private Vector2 randomDirX = new Vector2(-1, 1);
    [SerializeField] private Vector2 randomDirY = new Vector2(0.5f, 1);

    private Animator animator;
    private Health health;

    private void Awake() {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    private void OnEnable() {
        health.OnDamaged += HandleDamage;
        health.OnDeath += HandleDeath;
    }

    private void OnDisable() {
        health.OnDamaged -= HandleDamage;
        health.OnDeath -= HandleDeath;
    }

    private void HandleDamage() {
        animator.SetTrigger("isDamaged");
    }

    private void HandleDeath() {
        foreach (GameObject prefab in deathParts) {
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(randomRot.x, randomRot.y)).normalized;
            GameObject part = Instantiate(prefab, transform.position, rotation);

            Rigidbody2D rb = part.GetComponent<Rigidbody2D>();
            Vector2 randomDir = new Vector2(Random.Range(randomDirX.x, randomDirX.y), Random.Range(randomDirY.x, randomDirY.y)).normalized;
            rb.linearVelocity = randomDir * spawnForce;
            rb.AddTorque(Random.Range(-spawnTorque, spawnTorque), ForceMode2D.Impulse);

            Destroy(part, partLifetime);
        }

        Destroy(gameObject);
    }
}
