using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 12f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool isSpacePressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera=Camera.main;
    }

    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        isSpacePressed = Keyboard.current != null
        && Keyboard.current.spaceKey.isPressed;
#else
        isSpacePressed = Input.GetKey(KeyCode.Space);
#endif
    }

    void FixedUpdate()
    {
       if (isSpacePressed)
        {
            rb.linearVelocity = new Vector2(0f, moveSpeed);
        } 
    }

    void LateUpdate()
    {
        if (mainCamera == null || spriteRenderer == null)
        {
            return;
        }

        float cameraY = mainCamera.transform.position.y;
        float screenBottom = cameraY - mainCamera.orthographicSize;
        float screenTop = cameraY + mainCamera.orthographicSize;

        Bounds spriteBounds = spriteRenderer.bounds;
        float correctionY = 0f;

        if (spriteBounds.min.y < screenBottom)
        {
            correctionY = screenBottom - spriteBounds.min.y;
        }
        else if (spriteBounds.max.y > screenTop)
        {
            correctionY = screenTop - spriteBounds.max.y;
        }

        if (correctionY == 0f)
        {
            return;
        }

        rb.position += new Vector2(0f, correctionY);

        Vector2 velocity = rb.linearVelocity;
        if (velocity.y * correctionY < 0f)
        {
            rb.linearVelocity = new Vector2(velocity.x, 0f);
        }
    } 
}
