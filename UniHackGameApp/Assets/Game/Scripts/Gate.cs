using Codice.Client.Common.GameUI;
using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GateType
{
    OR,
    NOR,
    AND,
    NAND,
    XOR,
    NOT,
}

public class Gate : Node, IPointerClickHandler
{
    [SerializeField] Sprite[] gateImages;

    [SerializeField] GateType _type;

    public GateType type
    {
        get => _type; 
        set
        {
            if(_type == value) return;
            UpdateGateImage();
            _type = value;
        }
    }

    void UpdateGateImage()
    {
        image.sprite = gateImages.FirstOrDefault(img => img.name == Enum.GetName(typeof(GateType), type));  
    }

    public override void UpdateValue()
    {
        bool outValue = false;

        switch (type)
        {
            case GateType.OR:
                outValue = inputNodes.Where(nd=>nd.node).Aggregate(false, (acc, input) => acc || input.node.outputValue);
                break;
            case GateType.NOR:
                outValue = !inputNodes.Where(nd => nd.node).Aggregate(false, (acc, input) => acc || input.node.outputValue);
                break;

            case GateType.AND:
                outValue = inputNodes.Where(nd => nd.node).Aggregate(true, (acc, input) => acc && input.node.outputValue);
                break;

            case GateType.NAND:
                outValue = !inputNodes.Where(nd => nd.node).Aggregate(true, (acc, input) => acc && input.node.outputValue);
                break;

            case GateType.XOR:
                outValue = inputNodes.Where(nd => nd.node).Aggregate(false, (acc, input) => acc ^ input.node.outputValue);
                break;

            case GateType.NOT:
                outValue = !inputNodes.Where(nd => nd.node).Aggregate(false, (acc, input) => acc || input.node.outputValue);
                break;
        }

        outputValue = outValue;
    }

    public override void Start()
    {
        base.Start();
        UpdateGateImage();    
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Destroy(gameObject);
        }
    }
}
