using Codice.Client.Common.GameUI;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public enum GateType
{
    or,
    nor,
    and,
    nand,
    xor,
    xnor,
}

public class Gate : Node
{
    public GateType type;

    public override void UpdateValue()
    {
        bool outValue = false;

        switch (type)
        {
            case GateType.or:
                outValue = inputs.Aggregate(false, (acc, input) => acc || input.outputValue);
                break;

            case GateType.nor:
                outValue = !inputs.Aggregate(false, (acc, input) => acc || input.outputValue);
                break;

            case GateType.and:
                outValue = inputs.Aggregate(true, (acc, input) => acc && input.outputValue);
                break;

            case GateType.nand:
                outValue = !inputs.Aggregate(true, (acc, input) => acc && input.outputValue);
                break;

            case GateType.xor:
                outValue = inputs.Aggregate(false, (acc, input) => acc ^ input.outputValue);
                break;

            case GateType.xnor:
                outValue = !inputs.Aggregate(false, (acc, input) => acc ^ input.outputValue);
                break;
        }

        outputValue = outValue;
    }
}
