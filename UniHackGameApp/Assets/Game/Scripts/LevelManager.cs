using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject mazePrefab;
    [SerializeField] private Transform losePanel;

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
        if (mazeClone != null) { Destroy(mazeClone); }
        mazeClone = Instantiate(mazePrefab);
        mazeManager = mazeClone.GetComponent<MazeManager>();
    }

    public void ShowLosePanel()
    {
        losePanel.gameObject.SetActive(true);
        losePanel.localScale = Vector3.zero;
        losePanel.DOScale(1, 1).SetEase(Ease.OutBack).SetDelay(1f);
    }

    public void RestartLevel()
    {
        InstantiateMaze();
        losePanel.gameObject.SetActive(false);
        CircuitEvaluator.Instance.ResetCircuit();
        onLevelRestart();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
