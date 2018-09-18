using UnityEngine;
using System.Collections;

public class DragHandler : MonoBehaviour
{

    public static GameObject draggedObject;

    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 curScreenPoint;
    private Vector3 curPosition;
    //private Vector3 startPosition;
    private Vector3 dragOffset = new Vector3(0f,0.85f,0f);

    void Start()
    {
        if (gameObject.GetComponent<BoxCollider2D>()==null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        //startPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (GameManager.instance.pause)
            return;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        draggedObject = gameObject;
    }

    void OnMouseDrag()
    {
        if (GameManager.instance.pause)
            return;

        if (Input.touchCount>0)
            curScreenPoint = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, screenPoint.z);
        else
            curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition + dragOffset;//startPosition+(curPosition- startPosition)*1.3f;

    }

    void OnMouseUp()
    {
        if (GameManager.instance.pause)
            return;

        if (draggedObject == null)
            return;

        GameManager.instance.DropTiles(draggedObject.transform.position);
        draggedObject = null;

    }
}