using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomLevelLoader : MonoBehaviour
{
    [SerializeField] Transform mazeParent;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject playerPrefab;

    private GameObject playerClone;

    public static CustomLevelLoader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(TileEditorUI.lastSaved != null)
        {
            LoadLevel(TileEditorUI.lastSaved);
        }
    }

    private void LoadLevel(LevelData levelData)
    {
        for (int i = 0; i < mazeParent.childCount; i++)
        {
            Destroy(mazeParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < levelData.tiles.Count; i++)
        {
            GameObject newTile = Instantiate(tilePrefab, levelData.tiles[i].position, levelData.tiles[i].rotation);
            newTile.transform.parent = mazeParent;
            newTile.GetComponent<Tile>().LoadData(levelData.tiles[i]);
        }

        if (playerClone != null) { Destroy(playerClone); }
        playerClone = Instantiate(playerPrefab);
        playerClone.GetComponent<Player>().LoadData(levelData.player);

        mazeParent.GetComponent<MazeManager>().cheesesToCollect = levelData.cheeseWin;

        LevelManager.Instance.SetCustomMazePrefab(mazeParent.gameObject);
    }

    public void ReloadLevel()
    {
        if (TileEditorUI.lastSaved != null)
        {
            LoadLevel(TileEditorUI.lastSaved);
        }
    }

    public void BackToEditor()
    {
        SceneManager.LoadScene("LevelEditor");
    }
}
