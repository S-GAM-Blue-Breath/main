using UnityEngine;
using UnityEngine.UI;

public class HealthHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] drops;        // 왼쪽부터 순서대로
    [SerializeField] private Sprite fullSprite;    // water_full
    [SerializeField] private Sprite emptySprite;   // water_empty

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHpChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHpChanged -= Refresh;
        }
    }

    private void Start()
    {
        Refresh(playerHealth.CurrentHp);
    }

    private void Refresh(int hp)
    {
        // 왼쪽 hp개는 파란 물방울, 나머지는 빈 물방울
        for (int i = 0; i < drops.Length; i++)
        {
            drops[i].sprite = i < hp ? fullSprite : emptySprite;
        }
    }
}