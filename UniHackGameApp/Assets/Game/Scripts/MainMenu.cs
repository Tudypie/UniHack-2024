using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private CircuitEvaluator evaluator;

    private void Start()
    {
        evaluator = CircuitEvaluator.Instance;
        evaluator.onAfterTick += OnAfterTick;
    }

    private void OnDestroy()
    {
        if (evaluator)
        {
            evaluator.onAfterTick -= OnAfterTick;
        }
    }

    private void OnAfterTick()
    {
        if (evaluator.ReadOutput("Play")) { Play(); }
        if (evaluator.ReadOutput("Quit")) { Quit(); }
    }

    private void Play()
    {
        SceneManager.LoadScene(1);
    }

    private void Quit()
    {
        Application.Quit();
    }
}
