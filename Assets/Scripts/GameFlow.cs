using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameFlow
{
    // ⚠️ 실제 씬 이름과 똑같이 맞춰주세요
    public const string PrologueScene = "PrologueScene";
    public const string StageSelectScene = "StageSelectScene";

    private const string PrologueSeenKey = "PrologueSeen";

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

#if UNITY_EDITOR
    [UnityEditor.MenuItem("BlueBreath/프롤로그 본 기록 초기화")]
    private static void ResetPrologue()
    {
        PlayerPrefs.DeleteKey(PrologueSeenKey);
        Debug.Log("프롤로그 본 기록 초기화 완료");
    }
#endif
}