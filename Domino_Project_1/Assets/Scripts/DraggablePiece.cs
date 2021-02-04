using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablePiece : MonoBehaviour
{
    public int cardNumber;
    public bool isDouble;

    private Vector3 screenPoint;
    private Vector3 offset;

    public ServerData serverData;
    public GameController gameController;

    public Transform TableTransform;

    public int[] ValuesInThisPiece = new int[2];

    public Renderer pieceColor;
    public Color availablePieceColor;

    private void Start()
    {
        availablePieceColor = new Color(124, 195, 135, 255);
        pieceColor = GetComponent<Renderer>();

        int index = 0;
        foreach(PieceBehaviour piece in GetComponentsInChildren<PieceBehaviour>())
        {
            ValuesInThisPiece[index] = piece.GetValue();
            index++;
        }
    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }


    void OnMouseDrag()
    {
        // this.GetComponent<CollidersBehaviour>().isHolding = true;
        if (!serverData.IsMyTurn())
            return;

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

        if (serverData.isFirst && isDouble && GetComponentInChildren<PieceBehaviour>().value == serverData.biggestGlobalBomb)
        {
            //serverData.PrintText("Click!");
            this.gameObject.transform.SetParent(TableTransform, true);
            this.gameObject.transform.position = new Vector3(0, 0, 0);
            this.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

            serverData.isFirst = false;
            serverData.SetPieceOn(this.gameObject.name, this.gameObject.transform.position, TableTransform.position, this.gameObject.transform.rotation, true);
            serverData.ValuesToPut(GetComponentInChildren<PieceBehaviour>().value, GetComponentInChildren<PieceBehaviour>().value);

            gameController.OnTurnCompleted();
            //serverData.PrintText("Vai começar o round 2");
            return;
        } 
    }

    public void ChangeColor()
    {
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 2; j++)
            {
                if(ValuesInThisPiece[i] == serverData.AvailablePieces[j])
                    pieceColor.material.color = availablePieceColor;
            }
        }
    }

    public void ReturnToWhite()
    {
        pieceColor.material.color = Color.white;

    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
