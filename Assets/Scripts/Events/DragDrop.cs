using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject BoardArea;
    bool isDragging = false;
    bool isOverDropZone = false;
    GameObject dropZone;
    GameObject startParent;
    Vector2 startPosisition;

    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        BoardArea = GameObject.Find("BoardArea");
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding");
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Exit Collide");
        isOverDropZone = false;
        dropZone = null;
    }

    public void StartDrag()
    {
        if (!isOverDropZone)
        {
            startParent = transform.parent.gameObject;
            Debug.Log("start parent is " + startParent);
            startPosisition = transform.position;
            isDragging = true;
            Debug.Log(startPosisition);
        }
    }

    public void EndDrag()
    {
        isDragging = false;
        if (isOverDropZone)
        {
            transform.SetParent(dropZone.transform, false);
        }
        else
        {
            transform.SetParent(startParent.transform, false);
            transform.position = startPosisition;
        }
    }
}
