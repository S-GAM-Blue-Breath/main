using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 12f;

    private Rigidbody2D rb;
    private bool isSpacePressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
