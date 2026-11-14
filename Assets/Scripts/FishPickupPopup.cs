using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishPickupPopup : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Image fishIcon;
    [SerializeField] private TMP_Text fishLabel;

    [SerializeField, Min(0f)] private float holdDuration = 3f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.5f;

    [SerializeField, Min(0.01f)] private float slideDuration = 0.25f;
    private Vector2 visiblePosition;

    private readonly HashSet<string> shownFishNames = new();
    private readonly Queue<(string Name, Sprite Icon)> pending = new();
    private Coroutine playback;

    private void Awake()
    {
        visiblePosition = panel.anchoredPosition;
        panel.gameObject.SetActive(false);
    }

    public void Show(string fishName, Sprite icon)
    {
        // 같은 종류의 물고기는 첫 획득 때만 팝업에 추가합니다.
        if (!isActiveAndEnabled || !shownFishNames.Add(fishName))
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
        while (pending.Count > 0)
        {
            var fish = pending.Dequeue();

            fishIcon.sprite = fish.Icon;
            fishIcon.enabled = fish.Icon != null;
            fishLabel.text = $"{fish.Name} 획득!";

            panelGroup.alpha = 1f;

            // 기존 팝업의 오른쪽 위 앵커와 피벗을 기준으로 화면 밖에서 시작합니다.
            Vector2 hiddenPosition = new Vector2(panel.rect.width + 8f, visiblePosition.y);
            panel.anchoredPosition = hiddenPosition;
            panel.gameObject.SetActive(true);

            float slideElapsed = 0f;
            while (slideElapsed < slideDuration)
            {
                slideElapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(slideElapsed / slideDuration);
                float eased = Mathf.SmoothStep(0f, 1f, progress);
                panel.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, eased);
                yield return null;
            }

            panel.anchoredPosition = visiblePosition;
            yield return new WaitForSecondsRealtime(holdDuration);

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                panelGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }

            panel.gameObject.SetActive(false);
        }

        playback = null;
    }
}