using Codice.Client.BaseCommands;
using GluonGui.WorkspaceWindow.Views;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ConnSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] GameObject connPrefab;

    RectTransform displayConn;
    Canvas canvas;
    public Node parentNode { get; private set; }    
    
    public Action onDropped { get; set; } = () => { };

    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;
    Vector2 movedPos;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
        parentNode = GetComponentInParent<Node>();
    } 

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save original position in case of cancel
        originalPosition = rectTransform.anchoredPosition;
        movedPos = rectTransform.anchoredPosition;
        // Reduce the opacity to indicate dragging
        canvasGroup.alpha = 0.6f;
        // Make it so the dragged item is not blocking raycasts (so we can detect the drop area underneath)
        canvasGroup.blocksRaycasts = false;

        if (displayConn) 
        {
            Destroy(displayConn.gameObject);  
        }
        displayConn = Instantiate(connPrefab).GetComponent<RectTransform>();
        displayConn.transform.SetParent(transform.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        // Move the item along with the mouse pointer
        movedPos += (Vector2)canvas.transform.InverseTransformVector(eventData.delta);

        var otherConn = GetConnectorOverPointer(eventData);
        if(otherConn != null)
        {
            rectTransform.position = otherConn.GetComponent<RectTransform>().position;
        }
        else
        {
            rectTransform.anchoredPosition = movedPos;
        }




        displayConn.transform.localPosition = Vector3.zero;
        var outputPos = rectTransform.position;

        var dif = outputPos - displayConn.transform.position;
        const float offset = -3f;

        var difCanvas = canvas.transform.InverseTransformVector(dif);

        displayConn.sizeDelta = new Vector2(difCanvas.magnitude - offset, displayConn.sizeDelta.y);
        displayConn.localPosition = difCanvas.normalized * offset;
        displayConn.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(difCanvas.y, difCanvas.x) * Mathf.Rad2Deg);
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

    private ConnSpawner GetConnectorOverPointer(PointerEventData eventData)
    {
        // Raycast to check if the pointer is over any UI object with ConnectorSpawner
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var spawn = result.gameObject.GetComponent<ConnSpawner>();
            if (spawn && spawn != this)
                return spawn;
        }
        return null;
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null && dropped.GetComponent<ConnSpawner>())
        {
            // cannot drop on output, can only draw from input to output 
            if (parentNode.outputNode.parentElement == transform.parent)
                return;
            var otherSpawner = dropped.GetComponent<ConnSpawner>();

            parentNode.TryAddInputNode(otherSpawner.parentNode, transform.parent.GetComponent<RectTransform>());

            // Reparent the dragged object to this drop zone
            dropped.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
