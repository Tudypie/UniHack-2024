
using UnityEngine;
using UnityEngine.EventSystems;

public class Connector : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Destroy(gameObject);
        }
    }
}