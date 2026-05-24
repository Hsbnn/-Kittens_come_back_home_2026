using UnityEngine;

public sealed class StoryDirector : MonoBehaviour
{
    public static StoryDirector Instance { get; private set; }

    private bool introduced;
    private bool firstMeow;
    private bool firstObjectMoved;
    private bool firstCachedObject;
    private bool melodyOpened;
    private bool finalGateOpened;

    public string CurrentObjective { get; private set; } = "Доберись до первого колокольчика...";
    public string StoryStatus { get; private set; } = "Ночная буря разбудила город, котята застряли на крышах, помоги им.";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!introduced)
        {
            introduced = true;
            SetObjective(
                "Найди первый колокольчик. Ноты: 1, 2, 3.",
                "Колокольчики открывают путь.",
                8f);
        }
    }

    public static void NotifyMeow(string noteName)
    {
        if (Instance == null)
        {
            return;
        }

        if (Instance.firstMeow)
        {
            return;
        }

        Instance.firstMeow = true;
        Instance.SetObjective(
            "Открой ворота мелодией C -> E -> G у колокольчиков.",
            "Нота " + noteName + " не слышна. Подойди ближе или убери предмет.",
            7f);
    }

    public static void NotifyObjectMoved(string objectName)
    {
        if (Instance == null || Instance.firstObjectMoved)
        {
            return;
        }

        Instance.firstObjectMoved = true;
        Instance.SetObjective(
            "Расчищай крыши: ПКМ берёт предмет, R прячет в кеш, T возвращает.",
            "После бури предметы перекрывают проходы и глушат звук. Убери их.",
            7f);
    }

    public static void NotifyObjectCached(string objectName)
    {
        if (Instance == null || Instance.firstCachedObject)
        {
            return;
        }

        Instance.firstCachedObject = true;
        Instance.SetObjective(
            "Кешируй лишние предметы R и возвращай T, чтобы освобождать проход или делать ступеньки.",
            "Взрослый кот умеет прятать мелкие вещи и возвращать их...",
            8f);
    }

    public static void NotifyMelodyGateOpened()
    {
        if (Instance == null || Instance.melodyOpened)
        {
            return;
        }

        Instance.melodyOpened = true;
        Instance.SetObjective(
            "Найди испуганных котят. Держи Shift, пока котёнок не успокоится.",
            "Первый колокольчик зазвенел. Нужны остальные...",
            8f);
    }

    public static void NotifyKittenRescued(int rescued, int required)
    {
        if (Instance == null)
        {
            return;
        }

        if (rescued < required)
        {
            Instance.SetObjective(
                "Спасено котят: " + rescued + "/" + required + ". Найди следующего...",
                "Котёнок теперь доверяет тебе: твоё мяуканье звучит дальше и сильнее.",
                6f);
            return;
        }

        Instance.SetObjective(
            "Иди к большим финальным воротам.",
            "Два котёнка идут за тобой. Дом близко...",
            8f);
    }

    public static void NotifyFinalGateOpened()
    {
        if (Instance == null || Instance.finalGateOpened)
        {
            return;
        }

        Instance.finalGateOpened = true;
        Instance.SetObjective(
            "Финальные ворота открыты :3",
            "Остался последний шаг: большое оранжевое окно...",
            7f);
    }

    public static void CompleteStory()
    {
        if (Instance == null)
        {
            return;
        }

        Instance.SetObjective(
            "Котята дома!",
            "Они засыпают под твою колыбельную...",
            12f);
    }

    public static void ShowHint(string objective, string story, float seconds = 6f)
    {
        if (Instance == null)
        {
            GameUI.ShowMessage(story, seconds);
            return;
        }

        Instance.SetObjective(objective, story, seconds);
    }

    private void SetObjective(string objective, string story, float seconds)
    {
        CurrentObjective = objective;
        StoryStatus = story;
        GameUI.ShowMessage(story, seconds);
    }
}
