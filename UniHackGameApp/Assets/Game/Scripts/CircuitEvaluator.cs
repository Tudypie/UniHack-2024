using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircuitEvaluator : MonoBehaviour
{
    [SerializeField] List<Node> outNodes;
    public void UpdateCircuit(List<Node> outNodes)
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

        foreach(var node in sortedNodes.Reverse())
        {
            node.UpdateValue();
        }
    }

    void TopologicalSort(Node node, HashSet<Node> visited, Stack<Node> sortedNodes)
    {
        visited.Add(node);

        // Visit all input dependencies (neighbor nodes)
        foreach (var inputNode in node.inputs)
        {
            if (!visited.Contains(inputNode))
            {
                TopologicalSort(inputNode, visited, sortedNodes);
            }
        }

        // Once all dependencies are visited, add this node to the stack
        sortedNodes.Push(node);
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        UpdateCircuit(outNodes);
    }
}