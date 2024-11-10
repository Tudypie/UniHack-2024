using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomLevelLoader : MonoBehaviour
{
    [SerializeField] Transform mazeParent;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject playerPrefab;

    private void Start()
    {
        if(TileEditorUI.lastSaved != null)
        {
            LoadLevel(TileEditorUI.lastSaved);
        }
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

        mazeParent.GetComponent<MazeManager>().cheesesToCollect = levelData.cheeseWin;

        LevelManager.Instance.SetCustomMazePrefab(mazeParent.gameObject);
    }

    public void BackToEditor()
    {
        SceneManager.LoadScene("LevelEditor");
    }
}
