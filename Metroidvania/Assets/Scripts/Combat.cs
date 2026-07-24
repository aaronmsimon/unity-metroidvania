using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackCoolDown = 1.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator hitFX;

    public bool CanAttack => Time.time >= nextAttackTime;

    private Player player;
    
    private float nextAttackTime;

    private void Awake() {
        player = GetComponentInParent<Player>();
    }

    public void AttackAnimationFinished() {
        player.AnimationFinished();
    }

    public void Attack() {
        if (!CanAttack) return;

        nextAttackTime = Time.time + attackCoolDown;
        
        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer);

        if (enemy != null) {
            hitFX.Play("HitFX");
            enemy.gameObject.GetComponent<Health>().ChangeHealth(-damage);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
