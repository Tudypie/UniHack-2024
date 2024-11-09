using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitEvaluator : MonoBehaviour
{
    public Action onBeforeTick = () => { };
    public Action onAfterTick = () => { };

    [Serializable]
    public class Entry
    {
        public string name;
        public bool value;
    }

    public List<Entry> inputs = new();
    public List<Entry> outputs = new();

    [SerializeField] float tickDuration = 1f;

    [SerializeField] Transform outputsContainer;
    [SerializeField] Transform inputsContainer;

    float lastTimeTicked;
    bool isRunning;

    public static CircuitEvaluator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCircuit(Node[] outNodes)
    {
        HashSet<Node> visited = new();
        Stack<Node> sortedNodes = new();

        // Perform DFS on each outNode to get a topological ordering of nodes.
        foreach (var outNode in outNodes)
        {
            if (!visited.Contains(outNode))
            {
                TopologicalSort(outNode, visited, sortedNodes);
            }
        }

        foreach (var node in sortedNodes.Reverse())
        {
            node.UpdateValue();
        }
    }

    void TopologicalSort(Node node, HashSet<Node> visited, Stack<Node> sortedNodes)
    {

        visited.Add(node);

        // Visit all input dependencies (neighbor nodes)
        foreach (var inputNode in node.inputNodes)
        {
            if (inputNode.node && !visited.Contains(inputNode.node))
            {
                TopologicalSort(inputNode.node, visited, sortedNodes);
            }
        }

        // Once all dependencies are visited, add this node to the stack
        sortedNodes.Push(node);
    }

    public void ResetCircuit()
    {
        if (isRunning == false) return;
        isRunning = false;
        lastTimeTicked = 0;
        LevelManager.Instance.RestartLevel();
        ResetAllNodeValues();
    }

    public void ResetAllNodeValues()
    {
        var children = inputsContainer.parent.GetComponentsInChildren<Node>();
        foreach (var child in children)
        {
            child.ResetColor();
        }
    }

    public void PauseCircuit()
    {
        if (isRunning == false) return;
        isRunning = false;
        lastTimeTicked = 0;
    }

    public void RunCircuit()
    {
        if (isRunning) return;
        isRunning = true;
        UpdateCircuit(outputsContainer.GetComponentsInChildren<Node>());
    }

    void UpdateInputs()
    {
        foreach (var input in inputsContainer.GetComponentsInChildren<Node>())
        {
            var entry = inputs.Find(ip => ip.name == input.name);
            if (entry != null)
            {
                input.defaultValue = entry.value;
            }
        }
    }

    void UpdateOutputs()
    {
        foreach (var output in outputsContainer.GetComponentsInChildren<Node>())
        {
            var entry = outputs.Find(op => op.name == output.name);
            if (entry != null)
            {
                entry.value = output.outputValue;
            }
        }
    }

    public void SetInput(string inputName, bool value)
    {
        if (InputExists(inputName))
        {
            inputs.First(ot => ot.name == inputName).value = value;
        }
    }

    public bool ReadOutput(string outputName)
    {
        return outputs.First(ot => ot.name == outputName).value;
    }

    public bool InputExists(string inputName)
    {
        return inputs.Any(inp => inp.name == inputName);
    }

    public bool OutputExists(string outputName)
    {
        return outputs.Any(inp => inp.name == outputName);
    }

    void OnTick()
    {
        lastTimeTicked = Time.time;
        onBeforeTick();
        UpdateInputs();
        UpdateCircuit(outputsContainer.GetComponentsInChildren<Node>());
        UpdateOutputs();
        onAfterTick();
    }

    void Update()
    {
        if (isRunning && lastTimeTicked + tickDuration < Time.time)
        {
            OnTick();
        }
    }
}