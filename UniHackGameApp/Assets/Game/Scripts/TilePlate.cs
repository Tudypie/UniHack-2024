using DG.Tweening.Core.Easing;
using UnityEngine;

public class TilePlate : MonoBehaviour
{
    [SerializeField] private string inputName;

    [SerializeField] private bool playerOnPlate;

    private CircuitEvaluator evaluator;

    private void Start()
    {
        evaluator = CircuitEvaluator.Instance;
        evaluator.onBeforeTick += OnBeforeTick;
    }

    private void OnBeforeTick()
    {
        evaluator.SetInput(inputName, playerOnPlate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerOnPlate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerOnPlate = false;
        }
    }
}
