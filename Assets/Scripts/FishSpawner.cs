using System.Collections;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("물고기 프리팹")]
    [SerializeField] private GameObject clownfishPrefab;
    [SerializeField] private GameObject blueTangPrefab;
    [SerializeField] private GameObject stingrayPrefab;

    [Header("생성 설정")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float screenDuration = 5f;

    private IEnumerator Start()
    {
        // 1번 화면: 등장 없음
        Debug.Log("1번 화면");
        yield return new WaitForSeconds(screenDuration);

        // 2번 화면: 흰동가리 20%
        TrySpawn(clownfishPrefab, 100, 2);
        yield return new WaitForSeconds(screenDuration);

        // 3번 화면: 흰동가리 100%
        TrySpawn(clownfishPrefab, 100, 3);
        yield return new WaitForSeconds(screenDuration);

        // 4번 화면: 블루탱 20%
        TrySpawn(blueTangPrefab, 100, 4);
        yield return new WaitForSeconds(screenDuration);

        // 5번 화면: 블루탱 80%
        TrySpawn(blueTangPrefab, 100, 5);
        yield return new WaitForSeconds(screenDuration);

        // 6번 화면: 가오리 50%
        TrySpawn(stingrayPrefab, 100, 6);
        yield return new WaitForSeconds(screenDuration);

        // 7번 화면: 없음
        Debug.Log("7번 화면: Stage 2 전환 시점");
    }

    private void TrySpawn(GameObject prefab, int chance, int screen)
    {
        if (prefab == null || spawnPoint == null)
        {
            Debug.LogError($"{screen}번 화면: 프리팹 또는 생성 위치 연결 필요");
            return;
        }

        if (Random.Range(0, 100) < chance)
        {
            Instantiate(
                prefab,
                spawnPoint.position,
                Quaternion.identity
            );

            Debug.Log($"{screen}번 화면: 물고기 등장!");
        }
        else
        {
            Debug.Log($"{screen}번 화면: 물고기 등장 안 함");
        }
    }
}