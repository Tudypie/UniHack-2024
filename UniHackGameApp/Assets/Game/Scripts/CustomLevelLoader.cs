using UnityEngine;

public class CustomLevelLoader : MonoBehaviour
{
    [SerializeField] Transform mazeParent;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject playerPrefab;

    private void Start()
    {
        LevelData levelData = LevelData.OpenFileDialog();
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

        var player = Instantiate(playerPrefab);
        player.GetComponent<Player>().LoadData(levelData.player);

        LevelManager.Instance.SetCustomMazePrefab(mazeParent.gameObject);
    }
}
