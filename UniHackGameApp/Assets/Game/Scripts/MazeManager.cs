using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [SerializeField] private int cheesesToCollect;

    [SerializeField] private int collectedCheeses = 0;

    public void CollectCheese()
    {
        collectedCheeses++;
        if (collectedCheeses >= cheesesToCollect)
        {
            LevelManager.Instance.Win();
        }
    }
}
