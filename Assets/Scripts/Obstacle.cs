using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    // 닿아 있는 동안 계속 체크하고, 무적 시간은 PlayerHealth에서 막아요
    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

        // 단단한 장애물(돌)에 부딪혔을 때
    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerHealth health = collision.collider.GetComponentInParent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}