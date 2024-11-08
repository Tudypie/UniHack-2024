using Codice.Client.BaseCommands;
using GluonGui.WorkspaceWindow.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ConnSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] GameObject connPrefab;

    RectTransform displayConn;

    public Action onDropped { get; set; } = () => { };

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save original position in case of cancel
        originalPosition = rectTransform.anchoredPosition;
        // Reduce the opacity to indicate dragging
        canvasGroup.alpha = 0.6f;
        // Make it so the dragged item is not blocking raycasts (so we can detect the drop area underneath)
        canvasGroup.blocksRaycasts = false;

        if (displayConn) 
        {
            Destroy(displayConn.gameObject);  
        }
        displayConn = Instantiate(connPrefab).GetComponent<RectTransform>();
        displayConn.transform.parent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the item along with the mouse pointer
        rectTransform.anchoredPosition += eventData.delta;

        displayConn.transform.localPosition = Vector3.zero;
        var outputPos = rectTransform.position;

        var dif = outputPos - displayConn.transform.position;

        displayConn.GetComponent<RectTransform>().sizeDelta = new Vector2(dif.magnitude, displayConn.sizeDelta.y);
        displayConn.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset opacity and enable raycasts again
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (displayConn)
        {
            Destroy(displayConn.gameObject);
        }

        onDropped();

        // Optionally, reset position if not dropped in a valid drop zone
        rectTransform.anchoredPosition = originalPosition;

    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null && dropped.GetComponent<ConnSpawner>())
        {
            // Reparent the dragged object to this drop zone
            dropped.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
