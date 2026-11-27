using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PrologueManager : MonoBehaviour
{
    private class DialogueLine
    {
        public string id;
        public string speaker;
        public string text;
        public string note;
        public string[] images;
    }

    [Header("대본 (CSV UTF-8로 저장한 파일)")]
    [SerializeField] private TextAsset dialogueCsv;     // Dialogue_table.csv
    [SerializeField] private TextAsset charactersCsv;   // Characters.csv
    [SerializeField] private string heroName = "주인공"; // 이름 정해지면 여기만 바꾸면 됨

    [Header("이미지 폴더 (Assets/Resources 기준)")]
    [SerializeField] private string backgroundFolder = "Prologue/Backgrounds";
    [SerializeField] private string portraitFolder = "Prologue/Portraits";

    [Header("화면 요소")]
    [SerializeField] private Image background;
    [SerializeField] private Image portraitLeft;
    [SerializeField] private Image portraitRight;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject nameTagLeft;
    [SerializeField] private TMP_Text nameTextLeft;
    [SerializeField] private GameObject nameTagRight;
    [SerializeField] private TMP_Text nameTextRight;
    [SerializeField] private CanvasGroup fadePanel;     // 검은 Image + CanvasGroup

    [Header("연출")]
    [SerializeField] private float charsPerSecond = 30f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private Color missingBackgroundColor = new Color(0.55f, 0.8f, 0.9f); // 배경 미완성일 때 임시 색

    private readonly List<DialogueLine> lines = new();
    private readonly Dictionary<string, string> speakerNames = new();

    private int index = -1;
    private bool isTyping;
    private bool isBusy;          // 페이드 중에는 입력 무시
    private bool finished;
    private Coroutine typingRoutine;

    private void Start()
    {
        LoadCharacters();
        LoadDialogue();

        if (lines.Count == 0)
        {
            Debug.LogError("대사를 하나도 못 읽었어요. CSV 연결이나 UTF-8 저장을 확인하세요.");
            return;
        }

        fadePanel.alpha = 1f;
        StartCoroutine(GoToLine(0, fadeOutFirst: false));
    }

    private void Update()
    {
        if (finished || isBusy || !WasAdvancePressed())
        {
            return;
        }

        if (isTyping)
        {
            CompleteTyping();   // 타자 중이면 문장 전체를 바로 보여주기
            return;
        }

        bool fadeOut = lines[index].note.Contains("페이드아웃");
        StartCoroutine(GoToLine(index + 1, fadeOut));
    }

    // Skip 버튼에 연결
    public void Skip()
    {
        if (finished) return;
        finished = true;
        GameFlow.FinishPrologue();
    }

    // ───────────── 진행 ─────────────

    private IEnumerator GoToLine(int nextIndex, bool fadeOutFirst)
    {
        isBusy = true;

        if (fadeOutFirst)
        {
            yield return Fade(0f, 1f);
        }

        // 마지막 대사가 끝났으면 레벨 선택창으로
        if (nextIndex >= lines.Count)
        {
            if (!fadeOutFirst)
            {
                yield return Fade(fadePanel.alpha, 1f);
            }
            finished = true;
            GameFlow.FinishPrologue();
            yield break;
        }

        index = nextIndex;
        DialogueLine line = lines[index];
        ShowLine(line);

        if (line.note.Contains("페이드인"))
        {
            yield return Fade(1f, 0f);
        }
        else
        {
            fadePanel.alpha = 0f;
        }

        isBusy = false;
    }

    private void ShowLine(DialogueLine line)
    {
        ApplyImages(line.images);
        ApplySpeaker(line.speaker);

        string text = line.text.Replace("(-)", heroName);
        if (text == "(빈 대화창)")
        {
            text = string.Empty;
        }

        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
        }
        typingRoutine = StartCoroutine(TypeText(text));
    }

    // ───────────── 이미지 ─────────────

    private void ApplyImages(string[] images)
    {
        // 캐릭터 그림은 대사마다 새로 정함, 배경은 바뀔 때까지 유지
        portraitLeft.gameObject.SetActive(false);
        portraitRight.gameObject.SetActive(false);

        foreach (string name in images)
        {
            if (name == "black")
            {
                background.sprite = null;
                background.color = Color.black;
                continue;
            }

            Sprite bg = Resources.Load<Sprite>($"{backgroundFolder}/{name}");
            if (bg != null)
            {
                background.sprite = bg;
                background.color = Color.white;
                continue;
            }

            Sprite portrait = Resources.Load<Sprite>($"{portraitFolder}/{name}");
            if (portrait != null)
            {
                // 파일 이름에 _hero_ 가 있으면 오른쪽, 나머지는 왼쪽 (Sample.png 기준)
                Image target = name.Contains("_hero_") ? portraitRight : portraitLeft;
                target.sprite = portrait;
                target.preserveAspect = true;
                target.gameObject.SetActive(true);
                continue;
            }

            // 어느 폴더에도 없으면 아직 안 그려진 배경으로 보고 임시 색 표시
            Debug.LogWarning($"[프롤로그] 이미지 없음: {name} → 임시 배경색으로 표시");
            background.sprite = null;
            background.color = missingBackgroundColor;
        }
    }

    // ───────────── 이름표 ─────────────

    private void ApplySpeaker(string speaker)
    {
        nameTagLeft.SetActive(false);
        nameTagRight.SetActive(false);

        if (speaker == "NARR" || string.IsNullOrEmpty(speaker))
        {
            return; // 나레이션은 이름표 없음
        }

        string displayName = speaker == "HERO"
            ? heroName
            : speakerNames.TryGetValue(speaker, out string n) ? n : speaker;

        if (speaker == "HERO")
        {
            nameTextRight.text = displayName;
            nameTagRight.SetActive(true);
        }
        else
        {
            nameTextLeft.text = displayName;
            nameTagLeft.SetActive(true);
        }
    }

    // ───────────── 타자 효과 ─────────────

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        int total = dialogueText.textInfo.characterCount;
        float shown = 0f;

        while (dialogueText.maxVisibleCharacters < total)
        {
            shown += charsPerSecond * Time.deltaTime;
            dialogueText.maxVisibleCharacters = Mathf.Min(total, Mathf.FloorToInt(shown));
            yield return null;
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
        }
        dialogueText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
    }

    // ───────────── 페이드 ─────────────

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = to;
    }

    // ───────────── 입력 ─────────────

    private bool WasAdvancePressed()
    {
#if ENABLE_INPUT_SYSTEM
        bool key = Keyboard.current != null
            && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame);
        bool click = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        return key || click;
#else
        return Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetMouseButtonDown(0);
#endif
    }

    // ───────────── CSV 읽기 ─────────────

    private void LoadCharacters()
    {
        if (charactersCsv == null) return;

        List<string[]> rows = CsvReader.Parse(charactersCsv.text);
        for (int i = 1; i < rows.Count; i++) // 0번 줄은 제목줄
        {
            string key = CsvReader.Get(rows[i], 0);
            if (key.Length > 0)
            {
                speakerNames[key] = CsvReader.Get(rows[i], 1);
            }
        }
    }

    private void LoadDialogue()
    {
        List<string[]> rows = CsvReader.Parse(dialogueCsv.text);

        // 칸 순서: Scene, ID, Speaker, Text_KR, Note, Ilust
        for (int i = 1; i < rows.Count; i++)
        {
            string id = CsvReader.Get(rows[i], 1);
            if (id.Length == 0) continue; // 빈 줄 건너뛰기

            string ilust = CsvReader.Get(rows[i], 5);
            var images = new List<string>();
            foreach (string part in ilust.Split('/'))
            {
                string trimmed = part.Trim();
                if (trimmed.Length > 0) images.Add(trimmed);
            }

            lines.Add(new DialogueLine
            {
                id = id,
                speaker = CsvReader.Get(rows[i], 2),
                text = CsvReader.Get(rows[i], 3),
                note = CsvReader.Get(rows[i], 4),
                images = images.ToArray(),
            });
        }
    }
}
