using UnityEngine;

public sealed class GameUI : MonoBehaviour
{
    private static string message = "Найди колокольчики, собери мелодию и успокой котят мурчанием.";
    private static float messageUntil;
    private GUIStyle titleStyle;
    private GUIStyle labelStyle;
    private GUIStyle smallStyle;
    private GUIStyle messageStyle;
    private GUIStyle panelStyle;
    private GUIStyle headerStyle;
    private GUIStyle progressBackStyle;
    private GUIStyle progressFillStyle;

    public static void ShowMessage(string text, float seconds = 3f)
    {
        message = text;
        messageUntil = Time.time + seconds;
    }

    private void OnGUI()
    {
        EnsureStyles();
        Rect panelRect = new Rect(18f, 18f, 1010f, 445f);
        Rect headerRect = new Rect(32f, 32f, 982f, 58f);
        GUI.Box(panelRect, string.Empty, panelStyle);
        GUI.Box(headerRect, string.Empty, headerStyle);
        GUI.color = Color.white;

        CatController cat = CatController.Instance;
        CatObjectInteractor interactor = CatObjectInteractor.Instance;
        StoryDirector story = StoryDirector.Instance;
        string note = cat != null ? cat.CurrentNoteName : "?";
        int kittens = cat != null ? cat.RescuedKittens : 0;
        bool purring = cat != null && cat.IsPurring;
        int cacheCount = interactor != null ? interactor.CacheCount : 0;
        string held = interactor != null && !string.IsNullOrEmpty(interactor.HeldObjectName) ? interactor.HeldObjectName : "нет";
        string objective = story != null ? story.CurrentObjective : "Исследуй крыши, слушай колокольчики и ищи котят.";
        string status = story != null ? story.StoryStatus : "Буря разбросала вещи по крышам.";

        GUI.Label(new Rect(54f, 40f, 930f, 44f), "1. Дождь, крыши и колокольчики", titleStyle);
        GUI.Label(new Rect(54f, 105f, 930f, 84f), "Цель: " + objective, labelStyle);
        GUI.Label(new Rect(54f, 200f, 930f, 38f), "1/2/3 - C/E/G, F - мяукнуть, Shift у котёнка - успокоить, Tab - сменить камеру", smallStyle);
        GUI.Label(new Rect(54f, 240f, 930f, 38f), "ПКМ - взять/положить предмет, R - убрать в кэш, T - вернуть", smallStyle);
        GUI.Label(new Rect(54f, 281f, 930f, 38f), "Нота: " + note + " | Котята: " + kittens + "/2 | В руках: " + held + " | Кэш: " + cacheCount, smallStyle);
        if (Time.time < KittenRescue.VisibleCalmingUntil)
        {
            float progress = KittenRescue.VisibleCalmingProgress;
            GUI.Label(new Rect(54f, 322f, 930f, 38f), "Успокоение котёнка: " + Mathf.RoundToInt(progress * 100f) + "%", smallStyle);
            GUI.Box(new Rect(390f, 330f, 520f, 18f), string.Empty, progressBackStyle);
            GUI.Box(new Rect(390f, 330f, 520f * progress, 18f), string.Empty, progressFillStyle);
        }
        else
        {
            GUI.Label(new Rect(54f, 322f, 930f, 38f), purring ? "Подойди ближе к котёнку и мурчи." : "Найди и успокой двух котят.", smallStyle);
        }

        if (Time.time < messageUntil)
        {
            GUI.Label(new Rect(54f, 374f, 930f, 64f), message, messageStyle);
        }
        else
        {
            GUI.Label(new Rect(54f, 374f, 930f, 64f), status, messageStyle);
        }
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
        {
            return;
        }

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 36,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(1f, 0.9f, 0.62f) }
        };
        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            normal = { textColor = new Color(1f, 0.96f, 0.82f) }
        };
        smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            wordWrap = true,
            normal = { textColor = new Color(0.88f, 0.94f, 1f) }
        };
        messageStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            normal = { textColor = new Color(1f, 0.86f, 0.42f) }
        };
        panelStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = CreateSolidTexture(new Color(0.015f, 0.018f, 0.03f, 0.9f)) }
        };
        headerStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = CreateSolidTexture(new Color(0.11f, 0.1f, 0.16f, 0.92f)) }
        };
        progressBackStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = CreateSolidTexture(new Color(0.05f, 0.06f, 0.08f, 0.95f)) }
        };
        progressFillStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = CreateSolidTexture(new Color(1f, 0.72f, 0.28f, 0.95f)) }
        };
    }

    private static Texture2D CreateSolidTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
