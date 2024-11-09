using Codice.Client.BaseCommands;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] bool returnToPos = true;
    [SerializeField] bool allowDragChildren = false;
    public Action onDropped { get; set; } = () => { };

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;
    Canvas canvas;
    bool cancelThisDrag = false;

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

        if (allowDragChildren == false)
        {
            cancelThisDrag = IsPointerOverGameObject(eventData) == false;
            if (cancelThisDrag)
                return;
        } 

        // Save original position in case of cancel
        originalPosition = rectTransform.anchoredPosition;
        // Reduce the opacity to indicate dragging
        canvasGroup.alpha = 0.6f;
        // Make it so the dragged item is not blocking raycasts (so we can detect the drop area underneath)
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cancelThisDrag)
            return;
        // Move the item along with the mouse pointer
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (cancelThisDrag)
            return;
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


    private bool IsPointerOverGameObject(PointerEventData eventData)
    {
        // Check if the pointer is over the current GameObject
        return eventData.pointerEnter == gameObject;
    }
}
