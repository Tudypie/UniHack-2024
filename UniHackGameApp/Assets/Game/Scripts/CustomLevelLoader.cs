using UnityEngine;

public class CustomLevelLoader : MonoBehaviour
{
    [SerializeField] private Transform mazeParent;
    [SerializeField] private GameObject tilePrefab;

    private void Start()
    {
        LevelData levelData = LevelData.LoadFromFile();
        LoadLevel(levelData);
    }

    private void LoadLevel(LevelData levelData)
    {
        for (int i = 0; i < levelData.tiles.Count; i++)
        {
            GameObject newTile = Instantiate(tilePrefab, levelData.tiles[i].position, levelData.tiles[i].rotation);
            newTile.transform.parent = mazeParent;
            newTile.GetComponent<Tile>().LoadData(levelData.tiles[i]);
        }
        LevelManager.Instance.SetCustomMazePrefab(mazeParent.gameObject);
    }
}
