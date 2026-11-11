using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private string fishName = "흰동가리";
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float destroyX = -12f;

    [Header("위아래 이동")]
    [SerializeField] private float verticalSpeed = 1f;
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 2f;
    [SerializeField] private float changeInterval = 1.5f;

    private float targetY;
    private float timer;
    private bool collected = false;

    private void Start()
    {
        ChooseTargetHeight();
    }

    private void Update()
    {
        // 일정 시간마다 새로운 목표 높이 선택
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            ChooseTargetHeight();
        }

        Vector3 position = transform.position;

        // 왼쪽으로 이동
        position.x -= moveSpeed * Time.deltaTime;

        // 목표 높이를 향해 부드럽게 이동
        position.y = Mathf.MoveTowards(
            position.y,
            targetY,
            verticalSpeed * Time.deltaTime
        );

        transform.position = position;

        // 화면 밖으로 나가면 삭제
        if (position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }

    private void ChooseTargetHeight()
    {
        targetY = Random.Range(minY, maxY);
        timer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    bool isPlayer = other.CompareTag("Player");

    if (!isPlayer && other.attachedRigidbody != null)
    {
        isPlayer = other.attachedRigidbody.CompareTag("Player");
    }

    if (!isPlayer || collected)
    {
        return;
    }

    collected = true;

    Debug.Log($"{fishName} 획득!");

    FishPickupPopup popup = FindFirstObjectByType<FishPickupPopup>();
    SpriteRenderer fishRenderer = GetComponentInChildren<SpriteRenderer>();

    if (popup != null)
    {
        popup.Show(fishName, fishRenderer != null ? fishRenderer.sprite : null);
    }

    FishCollectionHud collectionHud = FindFirstObjectByType<FishCollectionHud>();
    collectionHud?.RevealFish(fishName);

    Destroy(gameObject);
}
}