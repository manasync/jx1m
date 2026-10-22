using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoStartHelper
{
    static AutoStartHelper()
    {
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var scene = EditorSceneManager.GetActiveScene();
                if (scene.name != "MainGame")
                {
                    EditorSceneManager.OpenScene("Assets/Scenes/MainGame.unity");
                }
            }
        };
    }

    [MenuItem("JX1M/Launch MainGame")]
    public static void LaunchMainGame()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.Log("[JX1M] Currently playing, stopping first...");
            EditorApplication.isPlaying = false;
            EditorApplication.update += WaitForStopAndLaunch;
            return;
        }

        DoOpenAndPlay();
    }

    private static void WaitForStopAndLaunch()
    {
        if (EditorApplication.isPlaying) return;

        EditorApplication.update -= WaitForStopAndLaunch;
        DoOpenAndPlay();
    }

    private static void DoOpenAndPlay()
    {
        Debug.Log("[JX1M] Opening scene Assets/Scenes/MainGame.unity...");
        EditorSceneManager.OpenScene("Assets/Scenes/MainGame.unity");

        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
        if (type != null)
        {
            var win = EditorWindow.GetWindow(type);
            win.Focus();
        }

        Debug.Log("[JX1M] Entering Play mode...");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("JX1M/Stop Game")]
    public static void StopGame()
    {
        EditorApplication.isPlaying = false;
    }
}
