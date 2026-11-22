using UnityEngine;
using UnityEngine.UI;

public class StageProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform stageEnd;   // Stage1_Midground 안의 끝 지점
    [SerializeField] private int steps = 5;        // 20%씩 → 5단계 (0이면 부드럽게)

    private Camera mainCamera;
    private float startDistance;

    private void Start()
    {
        mainCamera = Camera.main;
        startDistance = DistanceToEnd();
        slider.value = 0f;
    }

    private void Update()
    {
        if (startDistance <= 0f)
        {
            return;
        }

        float progress = 1f - DistanceToEnd() / startDistance;
        progress = Mathf.Clamp01(progress);

        if (steps > 0)
        {
            progress = Mathf.Floor(progress * steps) / steps;
        }

        slider.value = progress;
    }

    // 스테이지 끝 지점이 화면 오른쪽 끝까지 남은 거리
    private float DistanceToEnd()
    {
        float cameraRight = mainCamera.transform.position.x
            + mainCamera.orthographicSize * mainCamera.aspect;

        return stageEnd.position.x - cameraRight;
    }
}