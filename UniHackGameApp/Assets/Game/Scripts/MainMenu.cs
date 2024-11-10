using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isPlayPressed = false;
    private bool isQuitPressed = false;

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
        evaluator.SetInput("play_pressed", isPlayPressed);
        evaluator.SetInput("quit_pressed", isQuitPressed);
    }

    private void OnAfterTick()
    {
        if (evaluator.ReadOutput("play_game")) { Play(); }
        if (evaluator.ReadOutput("quit_game")) { Quit(); }
    }

    private void Play()
    {
        StartCoroutine(LoadFirstScene());
    }

    private void Quit()
    {
        Application.Quit();
    }

    public void OnClickPlay()
    {
        StartCoroutine(PressPlay());
    }

    public void OnClickQuit()
    {
        StartCoroutine(PressQuit());
    }

    private IEnumerator LoadFirstScene()
    {
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(1);
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
}
