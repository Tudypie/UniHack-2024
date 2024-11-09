using Codice.Client.BaseCommands;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DraggablePanel : MonoBehaviour, IScrollHandler, IDragHandler
{
    [SerializeField] RectTransform targetPanel;

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Canvas canvas;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the item along with the mouse pointer
        targetPanel.anchoredPosition += (Vector2)canvas.transform.InverseTransformVector(eventData.delta);
    }

    public void OnScroll(PointerEventData eventData)
    {
        //targetPanel.localScale += eventData.scrollDelta.y * Vector3.one / 100;

    }
}
