using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public List<Node> inputs = new();

    public bool defaultValue;
    public bool outputValue { get; protected set; }

    Image image;

    public virtual void UpdateValue()
    {
        if (inputs.Count == 0)
            outputValue = defaultValue;
        else 
            outputValue = inputs.Any(node => node.outputValue);
        print("node");
    }

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.color = outputValue ? Color.yellow : Color.white;    
    }
}