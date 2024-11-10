using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isPlayPressed = false;
    private bool isQuitPressed = false;
    private bool isEditorPressed = false;

    private CircuitEvaluator evaluator;

    private void Start()
    {
        evaluator = CircuitEvaluator.Instance;
        evaluator.onBeforeTick += OnBeforeTick;
        evaluator.onAfterTick += OnAfterTick;
        evaluator.RunCircuit();
    }

    private void OnDestroy()
    {
        if (evaluator)
        {
            evaluator.onBeforeTick -= OnBeforeTick;
            evaluator.onAfterTick -= OnAfterTick;
        }
    }

    private void OnBeforeTick()
    {
        evaluator.SetInput("play_press", isPlayPressed);
        evaluator.SetInput("editor_press", isEditorPressed);
        evaluator.SetInput("quit_press", isQuitPressed);
    }

    private void OnAfterTick()
    {
        if (evaluator.ReadOutput("load_lvl_1")) { Play(); }
        if (evaluator.ReadOutput("load_editor")) { LoadEditor(); }
        if (evaluator.ReadOutput("quit")) { Quit(); }
    }

    private void Play()
    {
        StartCoroutine(LoadSceneWithDelay("Level00"));
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void LoadEditor()
    {
        StartCoroutine(LoadSceneWithDelay("LevelEditor"));
    }

    public void OnClickPlay()
    {
        StartCoroutine(PressPlay());
    }

    public void OnClickQuit()
    {
        StartCoroutine(PressQuit());
    }

    public void OnClickEditor()
    {
        StartCoroutine(PressEditor());
    }

    private IEnumerator LoadSceneWithDelay(string scene)
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(scene);
    }

    private IEnumerator PressPlay()
    {
        isPlayPressed = true;
        yield return new WaitForSeconds(0.5f);
        isPlayPressed = false;
    }

    private IEnumerator PressQuit()
    {
        isQuitPressed = true;
        yield return new WaitForSeconds(0.5f);
        isQuitPressed = false;
    }

    private IEnumerator PressEditor()
    {
        isEditorPressed = true;
        yield return new WaitForSeconds(0.5f);
        isEditorPressed = false;
    }
}
