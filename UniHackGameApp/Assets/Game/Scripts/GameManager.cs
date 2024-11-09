using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform losePanel;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowLosePanel()
    {
        losePanel.gameObject.SetActive(true);
        losePanel.localScale = Vector3.zero;
        losePanel.DOScale(1, 1).SetEase(Ease.OutBack).SetDelay(1f);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
