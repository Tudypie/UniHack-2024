using Codice.Client.Common.GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Node;

public class Node : MonoBehaviour
{
    [Serializable]
    public class NodeSlot
    {
        public Node node;
        public RectTransform parentElement;
    }

    [Serializable]
    public class ConnectorToNode
    {
        public RectTransform connector;
        public Node node;
    }

    public IReadOnlyCollection<NodeSlot> inputNodes => _inputSlots;

    public NodeSlot outputNode { get => outputSlot; } 

    public bool defaultValue;
    public bool outputValue { get; protected set; }

    protected Image image;

    [SerializeField] RectTransform connectorPrefab;

    [SerializeField] List<NodeSlot> _inputSlots = new();

    [SerializeField] NodeSlot outputSlot;
    
    List<ConnectorToNode> spawnedConnectors = new();

    public bool TryAddInputNode(Node node, RectTransform parentElement)
    {
        var slot = _inputSlots.FirstOrDefault(slot => slot.parentElement == parentElement);

        if (slot == null)
            return false;

        slot.node = node;

        UpdateConnectors();
        return true;
    }

    public void RemoveInputNode(Node node)
    {
        var slot = _inputSlots.Find(sl => sl.node == node);
        if(slot != null)
        {
            slot.node = null;
        }
        UpdateConnectors();
    }

    void UpdateConnectorTransform(ConnectorToNode connToNode)
    {
        var conn = connToNode.connector;
        conn.localPosition = Vector3.zero;
        var outputPos = connToNode.node.outputSlot.parentElement.position;

        var dif = outputPos - connToNode.connector.position;

        conn.sizeDelta = new Vector2(dif.magnitude , conn.sizeDelta.y);
        conn.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg);

        conn.GetComponent<Image>().color = connToNode.node.outputValue ? Color.yellow : Color.white;
    }

    void UpdateConnectors()
    {

        // delete and update old slots 
        List<ConnectorToNode> tbd = new();
        foreach(var kv in spawnedConnectors)
        {
            if (kv.node && _inputSlots.Any(sl=>sl.node == kv.node))
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
            spawnedConnectors.Remove(obj);
            Destroy(obj.connector.gameObject);
        }

        // add new slots 
        foreach(var slot in _inputSlots)
        {
            if (slot.node == null || spawnedConnectors.Any(conn => conn.node == slot.node))
                continue;
            var inst = Instantiate(connectorPrefab);
            inst.SetParent(slot.parentElement);
            var connToNode = new ConnectorToNode() { node = slot.node, connector = inst };
            spawnedConnectors.Add(connToNode);
            UpdateConnectorTransform(connToNode);
        }
         
    }
     
    public virtual void UpdateValue()
    {
        if (inputNodes.Count == 0)
            outputValue = defaultValue;
        else 
            outputValue = inputNodes.Any(slot => slot.node ? slot.node.outputValue : false);
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