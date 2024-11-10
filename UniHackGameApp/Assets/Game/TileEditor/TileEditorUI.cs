
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TileEditorUI : MonoBehaviour
{
    [SerializeField] ButtonToPrefab[] buttonsToPrefabs;

    public ButtonToPrefab selectedPrefab = null;

    public static LevelData lastSaved;

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject playerPrefab;

    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button playButton;
    [SerializeField] Button homeButton;
    [SerializeField] Transform maze;


    public Player player;

    [Serializable]
    public class ButtonToPrefab
    {
        public Button button;
        public GameObject prefab;
    }

    void Start()
    {
        saveButton.onClick.AddListener(() =>
        {
            lastSaved = GetSave();
            if (lastSaved != null)
            {
                LevelData.SaveFileDialog(lastSaved);
            }
        });

        playButton.onClick.AddListener(() =>
        {
            lastSaved = GetSave();
            Play();
        });

        loadButton.onClick.AddListener(() =>
        {
            var levelData = LevelData.OpenFileDialog();
            if (levelData == null) return;
            Load(levelData);
        });

        homeButton.onClick.AddListener(() =>
        {
            Home();
        });

        var defaultColor = new Color(1, 1, 1, 1);
        var selectedColor = new Color(.9f, .9f, .9f, 1);
        foreach (var kv in buttonsToPrefabs)
        {
            kv.button.GetComponent<Image>().color = defaultColor;
            kv.button.onClick.AddListener(() =>
            {
                if (selectedPrefab != null && selectedPrefab.button)
                {
                    selectedPrefab.button.GetComponent<Image>().color = defaultColor;
                }

                kv.button.GetComponent<Image>().color = selectedColor;

                selectedPrefab = kv;
            });
        }

        if (lastSaved != null)
        {
            Load(lastSaved);
        }
    }

    void Play()
    {
        GetSave();
        SceneManager.LoadScene("LevelCustom");
    }

    void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Load(LevelData levelData)
    {

        for (int i = 0; i < maze.childCount; i++)
        {
            Destroy(maze.GetChild(i).gameObject);
        }

        for (int i = 0; i < levelData.tiles.Count; i++)
        {
            GameObject newTile = Instantiate(tilePrefab, levelData.tiles[i].position, levelData.tiles[i].rotation);
            newTile.transform.parent = maze;
            newTile.GetComponent<Tile>().LoadData(levelData.tiles[i]);
        }

        if (player != null) { Destroy(player.gameObject); }
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.LoadData(levelData.player);

        lastSaved = levelData;
    }

    LevelData GetSave()
    {
        if (player == null)
        {
            return null;
        }

        var tiles = maze.GetComponentsInChildren<Tile>();
        var levelData = new LevelData()
        {
            player = player.GetData(),
            tiles = tiles.Select(tile => tile.GetData()).ToList(),
            cheeseWin = tiles.Count(tile => tile.GetData().hasCheese && !tile.GetData().hasTrap)
        };

        return levelData;
    }

    public static bool IsMouseOverUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        return raycastResults.Count > 0;
    }


    void Update()
    {
        var leftMouse = Input.GetMouseButtonDown(0);
        var rightMouse = Input.GetMouseButtonDown(1);
        var mouseOverGui = IsMouseOverUI();
        var rotate = Input.GetKeyDown(KeyCode.R);

        if (rotate)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                Tile tile = hitObject.GetComponent<Tile>() ?? hitObject.GetComponentInParent<Tile>();

                Player player = hitObject.GetComponent<Player>() ?? hitObject.GetComponentInParent<Player>();
                if (tile)
                {
                    tile.transform.Rotate(Vector3.up, 90);
                }
                if (player)
                {
                    player.transform.Rotate(Vector3.up, 90);
                }
            }
        }

        if (mouseOverGui == false && selectedPrefab != null && (rightMouse || leftMouse))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var p = hit.point;
                p.y = 0;
                p.z = Mathf.RoundToInt(p.z);
                p.x = Mathf.RoundToInt(p.x);

                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.tag == "Ground")
                {

                    if (leftMouse && selectedPrefab.prefab && selectedPrefab.prefab.GetComponent<Tile>() != null)
                    {
                        var clone = Instantiate(selectedPrefab.prefab, maze);
                        clone.transform.position = p;
                    }
                }
                else if (hitObject.GetComponent<Tile>() || hitObject.GetComponentInParent<Tile>())
                {
                    var tile = hitObject.GetComponent<Tile>() ?? hitObject.GetComponentInParent<Tile>();

                    if (rightMouse)
                    {
                        if (hitObject.layer == LayerMask.NameToLayer("Cheese") || hitObject.layer == LayerMask.NameToLayer("Trap") || hitObject.layer == LayerMask.NameToLayer("Trap") || hitObject.tag == "TilePlate")
                        {
                            hitObject.SetActive(false);
                            return;
                        }

                        Destroy(tile.gameObject);
                    }

                    if (selectedPrefab.button.name == "Rat")
                    {
                        if (player == null)
                        {
                            player = Instantiate(selectedPrefab.prefab).GetComponent<Player>();
                            player.enabled = false;
                        }
                        p.y = .14f;
                        player.transform.position = p;
                    }
                    else if (selectedPrefab.button.name == "Plate")
                    {
                        tile.plate.SetActive(true);
                    }
                    else if (selectedPrefab.button.name == "Cheese")
                    {
                        tile.cheese.SetActive(true);
                    }
                    else if (selectedPrefab.button.name == "Trap")
                    {
                        tile.trap.SetActive(true);
                    }
                }
            }
        }
    }
}

