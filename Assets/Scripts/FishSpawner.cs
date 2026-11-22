using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{
    [Serializable]
    private class SpawnEntry
    {
        public string memo;                 // 예: "3번 화면 흰동가리"
        public Transform marker;            // Stage1_Midground 안의 위치 표시
        public GameObject prefab;           // 등장할 물고기
        [Range(0, 100)] public int chance = 100;
        public bool placeOnMarker; //체크하면 마커 위치(돌 위)에 놓임

        [NonSerialized] public bool rolled; // 이미 확률 굴렸는지
    }

    [SerializeField] private SpawnEntry[] entries;
    [SerializeField] private Transform spawnPoint;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // 돌 위 아이템은 스테이지 시작할 때 미리 확률을 굴려서 놓아둠
        foreach (SpawnEntry entry in entries)
        {
            if (entry.placeOnMarker && entry.marker != null)
            {
                entry.rolled = true;
                TrySpawn(entry);
            }
        }
    }

    private void Update()
    {
        float cameraRight = mainCamera.transform.position.x
            + mainCamera.orthographicSize * mainCamera.aspect;

        foreach (SpawnEntry entry in entries)
        {
            // 이미 굴렸거나, 마커가 아직 화면 오른쪽 밖이면 넘어감
            if (entry.rolled || entry.marker == null || entry.marker.position.x > cameraRight)
            {
                continue;
            }

            entry.rolled = true;   // 마커당 딱 한 번만
            TrySpawn(entry);
        }
    }

    private void TrySpawn(SpawnEntry entry)
    {
        if (entry.prefab == null || spawnPoint == null)
        {
            Debug.LogWarning($"[{entry.memo}] 프리팹 또는 생성 위치 연결 필요");
            return;
        }

        int roll = Random.Range(0, 100);   // 0~99

        if (roll < entry.chance)
        {
                    if (roll < entry.chance)
        {
            if (entry.placeOnMarker)
            {
                // 마커의 자식으로 생성 → 돌이랑 같이 움직임
                GameObject item = Instantiate(
                    entry.prefab, entry.marker.position, Quaternion.identity, entry.marker);

                if (item.TryGetComponent(out Fish fish))
                {
                    fish.StayInPlace();
                }
            }
            else
            {
                Instantiate(entry.prefab, spawnPoint.position, Quaternion.identity);
            }

            Debug.Log($"[{entry.memo}] 등장! (주사위 {roll} < {entry.chance})");
        }
            Debug.Log($"[{entry.memo}] 등장! (주사위 {roll} < {entry.chance})");
        }
        else
        {
            Debug.Log($"[{entry.memo}] 등장 안 함 (주사위 {roll} ≥ {entry.chance})");
        }
    }
}