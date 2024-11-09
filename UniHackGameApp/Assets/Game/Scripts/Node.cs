using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    RectTransform relativeTransform;

    List<ConnectorToNode> spawnedConnectors = new();
     
    public bool TryAddInputNode(Node node, RectTransform parentElement)
    {
        var slot = _inputSlots.FirstOrDefault(slot => slot.parentElement == parentElement);

        if (slot == null || node == slot.node)
            return false;

        slot.node = node;

        UpdateConnectors();
        return true;
    }

    public void RemoveInputNode(Node node)
    {
        var slot = _inputSlots.Find(sl => sl.node == node);
        if (slot != null)
        {
            slot.node = null;
        }
        UpdateConnectors();
    }

    public void ResetColor()
    {
        outputValue = false;
    }

    void UpdateConnectorTransform(ConnectorToNode connToNode)
    {
        var conn = connToNode.connector;
        const float offset = -3f;
        var outputPos = connToNode.node.outputSlot.parentElement.position;

        var dif = outputPos - connToNode.connector.position;
        var difCanvas = relativeTransform.transform.InverseTransformVector(dif);

        conn.sizeDelta = new Vector2(difCanvas.magnitude-offset, conn.sizeDelta.y);
        conn.localPosition = difCanvas.normalized * offset;
        conn.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(difCanvas.y, difCanvas.x) * Mathf.Rad2Deg);

        conn.GetComponent<Image>().color = connToNode.node.outputValue ? Color.yellow : Color.white;
    }

    void UpdateConnectors()
    {

        // delete and update old slots 
        List<ConnectorToNode> tbd = new();
        foreach (var kv in spawnedConnectors)
        {
            if (kv.node && kv.connector && _inputSlots.Any(sl => sl.node == kv.node))
            {
                UpdateConnectorTransform(kv);
            }
            else
            {
                tbd.Add(kv);
            }
        }

        foreach (var obj in tbd)
        {
            spawnedConnectors.Remove(obj);
            if (obj.connector)
            {
                Destroy(obj.connector.gameObject);
            }
        }

        // add new slots 
        foreach (var slot in _inputSlots)
        {
            if (slot.node == null || spawnedConnectors.Any(conn => conn.node == slot.node))
                continue;
            var inst = Instantiate(connectorPrefab);
            inst.SetParent(slot.parentElement);
            inst.GetComponent<Connector>().onDestroy += () =>
            {
                slot.node = null;
            };
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
        relativeTransform = GetComponentInParent<CodingPanelUI>().GetComponent<RectTransform>();
        image = GetComponent<Image>();
        UpdateConnectors();
        var text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
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