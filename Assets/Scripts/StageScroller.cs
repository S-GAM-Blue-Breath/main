using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StageScroller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.left * moveSpeed * Time.fixedDeltaTime);
    }
}