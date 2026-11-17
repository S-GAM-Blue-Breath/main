using UnityEngine;

public class Background : MonoBehaviour
{
    private const float MoveSpeed = 3f;

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.position += Vector3.left * MoveSpeed * Time.deltaTime;

        if (spriteRenderer == null || mainCamera == null)
        {
            return;
        }

        float cameraLeft = mainCamera.transform.position.x
            - mainCamera.orthographicSize * mainCamera.aspect;

        Bounds bounds = spriteRenderer.bounds;
        float repeatDistance = bounds.size.x * 2f;

        if (repeatDistance <= 0f)
        {
            return;
        }

        float distancePastLeft = cameraLeft - bounds.max.x;
        if (distancePastLeft >= 0f)
        {
            int repeats = Mathf.FloorToInt(distancePastLeft / repeatDistance) + 1;
            transform.position += Vector3.right * (repeatDistance * repeats);
        }
    }
}