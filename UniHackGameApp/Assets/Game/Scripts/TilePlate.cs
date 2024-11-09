using DG.Tweening.Core.Easing;
using UnityEngine;

public class TilePlate : MonoBehaviour
{
    [SerializeField] private string inputName;

    [SerializeField] private bool oneTimeTrigger = true;

    [SerializeField, Space] private bool triggeredTile;
    [SerializeField] private bool onTile;

    private CircuitEvaluator evaluator;

    private void Start()
    {
        evaluator = CircuitEvaluator.Instance;
        evaluator.onBeforeTick += OnBeforeTick;
    }

    private void OnBeforeTick()
    {
        evaluator.SetInput(inputName, oneTimeTrigger ? triggeredTile : onTile);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            triggeredTile = true;
            onTile = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onTile = false;   
        }
    }
}
