using UnityEngine;
using UnityEngine.SceneManagement;

// 화면 이동 규칙과 진행 기록(프롤로그 봤는지, 몇 스테이지까지 깼는지)을 한 곳에서 관리
public static class GameFlow
{
    // ⚠️ 실제 씬 이름과 똑같이 맞춰주세요
    public const string PrologueScene = "PrologueScene";
    public const string StageSelectScene = "StageSelectScene";
    public const string GameScene = "gameScene";

    public const int TotalStages = 5;

    // 12월 최종발표용: 첫번째 숨만 클리어해도 5개 스테이지가 전부 열림
    // 발표 끝나고 원래 규칙(하나씩 열림)으로 돌리려면 false로 바꾸기
    public static bool PresentationMode = true;

    private const string PrologueSeenKey = "PrologueSeen";
    private const string ClearedStageKey = "ClearedStage";

    // ───────────── 프롤로그 ─────────────

    public static bool HasSeenPrologue => PlayerPrefs.GetInt(PrologueSeenKey, 0) == 1;

    // Game Start 버튼: 처음이면 스토리, 아니면 바로 레벨 선택
    public static void StartGame()
    {
        SceneManager.LoadScene(HasSeenPrologue ? StageSelectScene : PrologueScene);
    }

    // Story 버튼: 항상 스토리
    public static void PlayStory()
    {
        SceneManager.LoadScene(PrologueScene);
    }

    // 스토리 마지막 대사가 끝나면 호출
    public static void FinishPrologue()
    {
        PlayerPrefs.SetInt(PrologueSeenKey, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(StageSelectScene);
    }

    // ───────────── 스테이지 ─────────────

    // 지금까지 클리어한 가장 높은 스테이지 번호 (0이면 아직 없음)
    public static int ClearedStage => PlayerPrefs.GetInt(ClearedStageKey, 0);

    // 스테이지 선택창에 보여줄 스테이지 개수
    public static int UnlockedStageCount
    {
        get
        {
            if (PresentationMode && ClearedStage >= 1)
            {
                return TotalStages;
            }
            return Mathf.Clamp(ClearedStage + 1, 1, TotalStages);
        }
    }

    // 지금 고른 스테이지 번호 (게임 씬에서 읽어서 쓸 수 있음)
    public static int SelectedStage { get; private set; } = 1;

    // 스테이지 선택창 버튼에서 호출
    public static void SelectStage(int stage)
    {
        SelectedStage = stage;

        // TODO: 도감 팝업(수민님 담당)이 완성되면,
        //       여기서 바로 시작하지 말고 도감 팝업을 띄운 뒤
        //       팝업의 "게임 시작" 버튼에서 StartSelectedStage()를 호출하도록 바꾸기
        StartSelectedStage();
    }

    public static void StartSelectedStage()
    {
        // 지금은 Stage 1 게임 씬만 있어서 모든 스테이지가 같은 씬으로 감
        SceneManager.LoadScene(GameScene);
    }

    // 진행도 100% 달성 시 게임 씬에서 호출 (예: GameFlow.MarkStageCleared(1))
    public static void MarkStageCleared(int stage)
    {
        if (stage > ClearedStage)
        {
            PlayerPrefs.SetInt(ClearedStageKey, stage);
            PlayerPrefs.Save();
        }
    }

#if UNITY_EDITOR
    // ───────────── 테스트용 메뉴 (유니티 상단 BlueBreath 메뉴) ─────────────

    [UnityEditor.MenuItem("BlueBreath/프롤로그 본 기록 초기화")]
    private static void ResetPrologue()
    {
        PlayerPrefs.DeleteKey(PrologueSeenKey);
        Debug.Log("프롤로그 본 기록 초기화 완료");
    }

    [UnityEditor.MenuItem("BlueBreath/스테이지 1 클리어한 걸로 만들기")]
    private static void ClearStage1ForTest()
    {
        MarkStageCleared(1);
        Debug.Log("스테이지 1 클리어 처리 완료");
    }

    [UnityEditor.MenuItem("BlueBreath/스테이지 클리어 기록 초기화")]
    private static void ResetStages()
    {
        PlayerPrefs.DeleteKey(ClearedStageKey);
        Debug.Log("스테이지 클리어 기록 초기화 완료");
    }
#endif
}
