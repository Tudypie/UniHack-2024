using Codice.Client.BaseCommands;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] bool returnToPos = true;
    public Action onDropped { get; set; } = () => { };

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;
    Canvas canvas;


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
    }
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save original position in case of cancel
        originalPosition = rectTransform.anchoredPosition;
        // Reduce the opacity to indicate dragging
        canvasGroup.alpha = 0.6f;
        // Make it so the dragged item is not blocking raycasts (so we can detect the drop area underneath)
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the item along with the mouse pointer
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset opacity and enable raycasts again
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;


        onDropped();

        if (returnToPos)
        {
            // Optionally, reset position if not dropped in a valid drop zone
            rectTransform.anchoredPosition = originalPosition;
        }

    }
}
