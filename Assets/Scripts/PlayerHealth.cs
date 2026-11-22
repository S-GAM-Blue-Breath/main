using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    [SerializeField] private float invincibleTime = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    public int MaxHp => maxHp;
    public int CurrentHp { get; private set; }

    public event Action<int> OnHpChanged;   // 체력 UI가 구독
    public event Action OnDead;             // 나중에 게임오버 화면이 구독

    private bool isInvincible;
    private bool isDead;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        CurrentHp = maxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead)
        {
            return;
        }

        // 물방울이 다 빈 상태에서 또 맞으면 = 4번째 충돌 → 종료
        if (CurrentHp <= 0)
        {
            Die();
            return;
        }

        CurrentHp = Mathf.Max(CurrentHp - damage, 0);
        Debug.Log($"피격! 남은 물방울: {CurrentHp}");
        OnHpChanged?.Invoke(CurrentHp);

        StartCoroutine(InvincibleRoutine());
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        Color original = spriteRenderer.color;
        Color faded = new Color(original.r, original.g, original.b, 0.3f);

        float elapsed = 0f;
        while (elapsed < invincibleTime)
        {
            spriteRenderer.color = spriteRenderer.color.a < 1f ? original : faded;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.color = original;
        isInvincible = false;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("스테이지 종료 (4번째 충돌)");
        OnDead?.Invoke();
        Time.timeScale = 0f;
    }
}