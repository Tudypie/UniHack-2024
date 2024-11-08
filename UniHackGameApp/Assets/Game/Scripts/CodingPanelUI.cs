
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CodingPanelUI : MonoBehaviour
{
    [SerializeField] Draggable[] gatesButtons;
    [SerializeField] GameObject[] gatesPrefabs;

    void SpawnGate(string gateName, Vector3 position)
    {
        var clone = Instantiate(gatesPrefabs.First(g => g.name == gateName));
        clone.transform.parent = transform;
        clone.transform.position = position;
    }


    void Start()
    {
        foreach(var but in gatesButtons)
        {
            but.onDropped += () =>
            {
                SpawnGate(but.name, but.transform.position);
            };
        }
    }
}