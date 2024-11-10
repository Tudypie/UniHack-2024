using UnityEngine;

public class MazeManager : MonoBehaviour
{
    public int cheesesToCollect;

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
