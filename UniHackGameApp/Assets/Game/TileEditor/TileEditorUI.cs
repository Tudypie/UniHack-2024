
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileEditorUI : MonoBehaviour
{
    [SerializeField] ButtonToPrefab[] buttonsToPrefabs;

    public ButtonToPrefab selectedPrefab = null;

    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
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
            Save();
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
    }

    void Save()
    {
        var tiles = maze.GetComponentsInChildren<Tile>();
        var levelData = new LevelData()
        {
            player = player.GetData(),
            tiles = tiles.Select(tile => tile.GetData()).ToList()
        };
        LevelData.SaveFileDialog(levelData);
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
                        var clone = Instantiate(selectedPrefab.prefab,maze);
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
                        if(player == null)
                        {
                            player = Instantiate(selectedPrefab.prefab).GetComponent<Player>();
                            player.enabled = false;
                        }
                        player.transform.position = p;
                    }
                    else if(selectedPrefab.button.name == "Plate")
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

