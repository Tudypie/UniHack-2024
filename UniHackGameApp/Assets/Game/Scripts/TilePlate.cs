using DG.Tweening.Core.Easing;
using UnityEngine;

public class TilePlate : MonoBehaviour
{
    [SerializeField] private string inputName;

    [SerializeField] private bool playerIsOnPlate;
    [SerializeField] private bool playerSteppedOnPlate;

    private CircuitEvaluator evaluator;

    private void Start()
    {
        evaluator = CircuitEvaluator.Instance;
        evaluator.onBeforeTick += OnBeforeTick;
    }

    private void OnBeforeTick()
    {
        evaluator.SetInput("is_on_" + inputName, playerIsOnPlate);
        evaluator.SetInput("stepped_on_" + inputName, playerSteppedOnPlate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsOnPlate = true;
            playerSteppedOnPlate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsOnPlate = false;
        }
    }
}
