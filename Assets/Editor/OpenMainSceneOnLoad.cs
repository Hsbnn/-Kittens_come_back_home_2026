using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class OpenMainSceneOnLoad
{
    private const string ScenePath = "Assets/Scenes/RoofsAndBells.unity";

    static OpenMainSceneOnLoad()
    {
        EditorApplication.delayCall += EnsureMainSceneOpen;
    }

    private static void EnsureMainSceneOpen()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        if (!System.IO.File.Exists(ScenePath))
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.path == ScenePath)
        {
            return;
        }

        if (string.IsNullOrEmpty(activeScene.path) || activeScene.rootCount == 0)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }
    }
}
