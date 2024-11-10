
using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class TileEditorUI : MonoBehaviour
{
    [SerializeField] ButtonToPrefab[] buttonsToPrefabs;

    public ButtonToPrefab selectedPrefab;
    
    [Serializable]
    public class ButtonToPrefab
    {
        public Button button;
        public GameObject prefab;
    }


    void Start()
    {
        var defaultColor = new Color(1, 1, 1, .75f);
        var selectedColor = new Color(1, 1, 1, 1);
        foreach (var kv in buttonsToPrefabs)
        {
            kv.button.GetComponent<Image>().color = defaultColor;
            kv.button.onClick.AddListener(() =>
            {
                if(selectedPrefab != null)
                {
                    kv.button.GetComponent<Image>().color = defaultColor;
                }

                kv.button.GetComponent<Image>().color = selectedColor; 

                selectedPrefab = kv;
            });
        }    
    }

    void Update()
    {
        if (selectedPrefab != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                if(hitObject.tag == "Ground")
                {

                }
                else if (hitObject.GetComponent<Tile>())
                {
                    if (selectedPrefab.button.name == "Rat")
                    {

                    }
                    else if(selectedPrefab.button.name == "Cheese")
                    {

                    }
                    else if(selectedPrefab.button.name == "Trap")
                    {

                    }
                }
            }
        }
    }
}

