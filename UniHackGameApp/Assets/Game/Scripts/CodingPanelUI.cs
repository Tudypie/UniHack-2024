using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodingPanelUI : MonoBehaviour
{
    [SerializeField] RectTransform inputsPanel;
    [SerializeField] RectTransform outputsPanel;

    [SerializeField] GameObject inputPrefab;
    [SerializeField] GameObject outputPrefab;

    [SerializeField] Draggable[] gatesButtons;
    [SerializeField] GameObject[] gatesPrefabs;

    [SerializeField] Button runButton;
    [SerializeField] Button resetButton;

    [SerializeField] List<string> enabledGates = new List<string> { "NAND", "XOR", "AND", "NOT", "NOR" };

    CircuitEvaluator circuitEvaluator => CircuitEvaluator.Instance;
    bool hasStarted;
    bool isPaused;

    void SpawnGate(string gateName, Vector3 position)
    {
        var clone = Instantiate(gatesPrefabs.First(g => g.name == gateName));
        clone.transform.SetParent(transform);
        clone.transform.position = position;
    }

    void UpdateOutputs()
    {
        // delete and update old 

        foreach (var node in outputsPanel.GetComponentsInChildren<Node>())
        {
            var entry = circuitEvaluator.outputs.Find(ent => ent.name == node.name);

            if (circuitEvaluator.outputs.Any(n => n.name == node.name) == false)
            {
                Destroy(node.gameObject);
            }
        }

        // add new 

        foreach (var entry in circuitEvaluator.outputs)
        {
            if (outputsPanel.Find(entry.name) == null)
            {
                var clone = Instantiate(outputPrefab);
                clone.transform.SetParent(outputsPanel);
                clone.name = entry.name;
            }
        }
    }

    void UpdateInputs()
    {
        // delete and update old 

        foreach (var node in inputsPanel.GetComponentsInChildren<Node>())
        {
            var entry = circuitEvaluator.inputs.Find(ent => ent.name == node.name);

            if (circuitEvaluator.inputs.Any(n => n.name == node.name) == false)
            {
                Destroy(node.gameObject);
            }
        }

        // add new 

        foreach (var entry in circuitEvaluator.inputs)
        {
            if (inputsPanel.Find(entry.name) == null)
            {
                var clone = Instantiate(inputPrefab);
                clone.transform.SetParent(inputsPanel);
                clone.name = entry.name;
            }
        }
    }

    void UpdateEntries()
    {
        UpdateOutputs();
        UpdateInputs();
    }


    void OnReset()
    {
        hasStarted = false;
        circuitEvaluator.ResetCircuit();
        runButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        LevelManager.Instance.RestartLevel();
    }

    void OnRun()
    {
        if (hasStarted == false)
        {
            hasStarted = true;
            isPaused = false;
            circuitEvaluator.RunCircuit();
            runButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        }
        else if(isPaused && hasStarted)
        {
            isPaused = false;
            circuitEvaluator.RunCircuit();
            runButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        }

        else if(isPaused == false && hasStarted)
        { 
            isPaused = true;
            circuitEvaluator.PauseCircuit();
            runButton.GetComponentInChildren<TextMeshProUGUI>().text = "Resume";
        }
    }

    void UpdateEnabledGates()
    {
        foreach (var but in gatesButtons)
        {
            if (enabledGates.Contains(but.name) == false)
            {
                but.gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {

        foreach (var but in gatesButtons)
        {
            but.onDropped += () =>
            {
                SpawnGate(but.name, but.transform.position);
            };
        }

        UpdateEnabledGates();

        runButton.onClick.AddListener(OnRun);
        resetButton.onClick.AddListener(OnReset);
        LevelManager.Instance.onLevelRestart += OnReset;

        UpdateEntries();

    }
}