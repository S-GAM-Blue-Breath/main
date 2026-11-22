using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Player : MonoBehaviour
{
    [SerializeField]
    private float jumpSpeed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private bool isSpacePressed;
    [SerializeField] private float returnSpeed = 3f;   // 밀려난 뒤 돌아오는 속도
    private float homeX;                                // 원래 가로 위치

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera=Camera.main;
        homeX = rb.position.x;
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
        Vector2 velocity = rb.linearVelocity;

        if (isSpacePressed)
        {
            velocity.y = jumpSpeed;   // 가로 속도는 건드리지 않음
        }

        // 돌에 밀려났으면 원래 자리로 천천히 복귀
        velocity.x = (homeX - rb.position.x) * returnSpeed;

        rb.linearVelocity = velocity;
    }

    void ClampToScreen() {

    }
    
    // 현재 방법 : 보정값을 더하는 방식
    // 위에 만들어둔 ClampToScreen 함수에 아래 LateUpdate 함수를 목표 위치로 바로 고정하는 방식으로 바꿔 코드를 작성해보세요
    // 컴비 수업 전까지

    // isTrigger과 onCollision의 차이를 공부하시오
        // isTrigger : 통과됨으로써 물리충격 없이 충돌 시 효과를 발생시킴
        // onCollision : 통과되지 않고 실제 현실처럼 부딪히면서 서로 반대방향으로 밀려나면서 충돌 효과를 발생시킴
    // 시네마틱 효과에 대해서 알아보세요
        // 영화같은 효과를 줌 -> 바람의 영향이나 특히! 중력의 영항을 뺄 수 있음

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
