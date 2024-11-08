using Codice.Client.Common.GameUI;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    [SerializeField] RectTransform connectorPrefab;

    [SerializeField] List<Node> _inputs = new();
    [SerializeField] RectTransform[] inputTransforms;
    
    public virtual RectTransform outputTransform => GetComponent<RectTransform>();
    public IReadOnlyCollection<Node> inputs => _inputs;

    public bool defaultValue;
    public bool outputValue { get; protected set; }

    protected Image image;

    public class ConnectorToNode
    {
        public RectTransform connector;
        public Node node;
    }

    List<ConnectorToNode> connectorToNode = new();

    public void AddInputNode()
    {
        UpdateConnectors();
    }

    public void RemoveInputNode()
    {
        UpdateConnectors();
    }

    void UpdateConnectorTransform(ConnectorToNode connToNode)
    {
        var conn = connToNode.connector;
        conn.localPosition = Vector3.zero;
        var outputPos = connToNode.node.outputTransform.position;

        var dif = outputPos - connToNode.connector.position;

        conn.sizeDelta = new Vector2(dif.magnitude , conn.sizeDelta.y);
        conn.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg);

        conn.GetComponent<Image>().color = connToNode.node.outputValue ? Color.yellow : Color.white;
    }

    void UpdateConnectors()
    {
        List<ConnectorToNode> tbd = new();
        foreach(var kv in connectorToNode)
        {
            if (kv.node && inputs.Contains(kv.node))
            {
                UpdateConnectorTransform(kv);
            }
            else
            {
                tbd.Add(kv);
            }
        }

        foreach(var obj in tbd)
        {
            connectorToNode.Remove(obj);
            Destroy(obj.connector.gameObject);
        }

        for(int i=0;i<inputs.Count;i++)
        {
            if (_inputs[i] && connectorToNode.Any(conn => conn.node == _inputs[i]) == false)
            {
                var inst = Instantiate(connectorPrefab);
                if(i < inputTransforms.Length)
                {
                    inst.parent = inputTransforms[i]; 
                } 
                else
                {
                    inst.parent = transform;
                }
                var connToNode = new ConnectorToNode() { node = _inputs[i], connector = inst };
                connectorToNode.Add(connToNode);
                UpdateConnectorTransform(connToNode);
            }
        }
         
    }
     
    public virtual void UpdateValue()
    {
        if (inputs.Count == 0)
            outputValue = defaultValue;
        else 
            outputValue = inputs.Any(node => node.outputValue);
    }

    public virtual void Start()
    {
        image = GetComponent<Image>();
        UpdateConnectors();
        var text = GetComponentInChildren<TextMeshProUGUI>();
        if(text != null)
        {
            text.text = transform.name;
        }
    }

    public virtual void Update()
    {
        image.color = outputValue ? Color.yellow : Color.white;
        UpdateConnectors();
    }
}