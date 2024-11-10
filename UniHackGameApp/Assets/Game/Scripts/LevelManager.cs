using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private GameObject mazePrefab;
    [SerializeField] private Transform losePanel;
    [SerializeField] private Transform winPanel;
    [SerializeField] AudioSource completedAudio;
   

    public Action onLevelRestart { get; set; } = () => { };

    private GameObject mazeClone;

    public MazeManager mazeManager { get; private set; }

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        InstantiateMaze();
    }
    
    private void InstantiateMaze()
    {
        if (mazePrefab == null) { return; }
        if (mazeClone != null) { Destroy(mazeClone); }
        mazeClone = Instantiate(mazePrefab);
        mazeManager = mazeClone.GetComponent<MazeManager>();
    }

    public void Lose()
    {
        CircuitEvaluator.Instance.PauseCircuit();
        losePanel.gameObject.SetActive(true);
        losePanel.localScale = Vector3.zero;
        losePanel.DOScale(1, 1).SetEase(Ease.OutBack).SetDelay(1f);
    }

    public void Win()
    {
        CircuitEvaluator.Instance.PauseCircuit();
        winPanel.gameObject.SetActive(true);
        winPanel.localScale = Vector3.zero;
        winPanel.DOScale(1, 1).SetEase(Ease.OutBack).SetDelay(1f);
        completedAudio.Play();
    }

    public void RestartLevel()
    {
        InstantiateMaze();
        winPanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
        CircuitEvaluator.Instance.ResetCircuit();
        onLevelRestart();
    }

    public void NextLevel()
    {
        string nextSceneName = "Level0" + (currentLevel + 1).ToString();
        SceneManager.LoadScene(nextSceneName);

    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowSolution()
    {
        string sceneToLoad = SceneManager.GetActiveScene().name + "Solved";
        SceneManager.LoadScene(sceneToLoad);
    }
}
