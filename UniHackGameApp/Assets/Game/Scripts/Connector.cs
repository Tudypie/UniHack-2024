
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Connector : MonoBehaviour, IPointerClickHandler
{
    public Action onDestroy { get; set; } = () => { };

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            onDestroy();
            Destroy(gameObject);
        }
    }
}