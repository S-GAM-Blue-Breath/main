using UnityEngine;

// 스테이지 선택창: 열린 스테이지 줄만 보여주고, 버튼을 누르면 해당 스테이지로 이동
public class StageSelectManager : MonoBehaviour
{
    [Tooltip("위에서부터 첫번째~마지막 숨 줄 (StageRow1 ~ StageRow5)")]
    [SerializeField] private GameObject[] stageRows;

    private void Start()
    {
        int unlocked = GameFlow.UnlockedStageCount;

        for (int i = 0; i < stageRows.Length; i++)
        {
            stageRows[i].SetActive(i < unlocked);
        }
    }

    // 각 시작 버튼의 On Click에 연결하고, 숫자 칸에 스테이지 번호(1~5)를 입력
    public void OnClickStage(int stage)
    {
        GameFlow.SelectStage(stage);
    }
}
