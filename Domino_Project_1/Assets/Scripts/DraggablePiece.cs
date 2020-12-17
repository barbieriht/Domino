using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablePiece : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    //private Transform TableTransform;
    /*
    private void Start()
    {
        TableTransform = GameObject.FindGameObjectWithTag("Table").transform;
    }
    */
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }

    

    void OnMouseDrag()
    {
       // this.GetComponent<CollidersBehaviour>().isHolding = true;

        //não pode ser movida se já pertencer à mesa
        if (this.transform.IsChildOf(GameObject.FindGameObjectWithTag("Table").transform))
        {
            return;
        }

        
        if (Input.GetMouseButtonDown(1))
        {
            this.transform.Rotate(0, 0, 90, Space.Self);
        }
        

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }
    /*
    private void OnMouseUp()
    {
        this.GetComponent<CollidersBehaviour>().isHolding = false;
    }
    */

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
