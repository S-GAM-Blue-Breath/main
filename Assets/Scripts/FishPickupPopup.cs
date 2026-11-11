using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishPickupPopup : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Image fishIcon;
    [SerializeField] private TMP_Text fishLabel;

    [SerializeField, Min(0.01f)] private float slideDuration = 0.25f;
    [SerializeField, Min(0f)] private float holdDuration = 1.5f;

    private readonly Queue<(string Name, Sprite Icon)> pending = new();
    private Vector2 visiblePosition;
    private Coroutine playback;

    private void Awake()
    {
        visiblePosition = panel.anchoredPosition;
        panel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (playback != null)
        {
            StopCoroutine(playback);
            playback = null;
        }

        pending.Clear();
        panel.gameObject.SetActive(false);
    }

    public void Show(string fishName, Sprite icon)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        pending.Enqueue((fishName, icon));

        if (playback == null)
        {
            playback = StartCoroutine(PlayPopups());
        }
    }

    private IEnumerator PlayPopups()
    {
        // 오른쪽 위 앵커와 피벗을 기준으로 패널 전체를 화면 밖에 둡니다.
        Vector2 hiddenPosition = new Vector2(
            panel.rect.width + 8f,
            visiblePosition.y
        );

        while (pending.Count > 0)
        {
            var fish = pending.Dequeue();
            fishIcon.sprite = fish.Icon;
            fishIcon.enabled = fish.Icon != null;
            fishLabel.text = $"{fish.Name} 획득!";

            panel.anchoredPosition = hiddenPosition;
            panel.gameObject.SetActive(true);

            yield return Slide(hiddenPosition, visiblePosition);
            yield return new WaitForSecondsRealtime(holdDuration);
            yield return Slide(visiblePosition, hiddenPosition);

            panel.gameObject.SetActive(false);
        }

        playback = null;
    }

    private IEnumerator Slide(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / slideDuration);
            float eased = Mathf.SmoothStep(0f, 1f, progress);
            panel.anchoredPosition = Vector2.Lerp(from, to, eased);
            yield return null;
        }

        panel.anchoredPosition = to;
    }
}