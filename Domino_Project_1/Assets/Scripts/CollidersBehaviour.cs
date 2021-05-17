using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidersBehaviour : MonoBehaviour
{
    private int value;
    private int otherValueOfThis;

    private int otherValue;
    public bool isPieceTip;

    public Collider2D HPieceCollider;

    //private Transform TableTransform;
    public Transform FullPiece;
    public ServerData serverData;
    public Transform TableTransform;
    public GameController gameController;

    void Start()
    {
        this.gameObject.tag = "Collider";
        this.gameObject.layer = 14;
        HPieceCollider = this.GetComponent<Collider2D>();
        TableTransform = GameObject.FindGameObjectWithTag("Table").transform;
        FullPiece = this.transform.parent.transform.parent;
        serverData = GameObject.FindGameObjectWithTag("GameController").GetComponent<ServerData>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        value = GetComponentInParent<PieceBehaviour>().GetValue();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.gameObject.CompareTag("FullPiece"))
        {
            return;
        }

        //se ambos colisores são do "lado" da peça, elas não podem se juntar
        if (!isPieceTip && !other.GetComponent<CollidersBehaviour>().isPieceTip)
            return;

        //se ambas as peças são da mão do jogador, não podem se juntar
        if (FullPiece.transform.parent == other.GetComponent<CollidersBehaviour>().FullPiece.transform.parent)
            return;

        if (!FullPiece.GetComponent<DraggablePiece>().canConnect)
            return;

        if (FullPiece.GetComponent<DraggablePiece>().isColliding)
            return;

        if (this.gameObject.GetComponentInParent<HalfPiece>().halfPieceConnected)
            return;

        //se o outro objeto ainda não foi ligado
        if (!other.gameObject.GetComponentInParent<HalfPiece>().halfPieceConnected)
        {
            //Debug.Log(other.gameObject.name + " is a Border Piece");

            //lê o valor da peça oposta
            otherValue = other.gameObject.GetComponent<CollidersBehaviour>().value;

            //compara os valores
            if (otherValue == this.value)
            {
                //Debug.Log(other.gameObject.name + " got the same value: " + otherValue);

                //transforma o objeto atual em um "filho" da mesa
                if(FullPiece.transform.parent == TableTransform)
                {
                    return;
                }

                Vector3 distance = this.transform.position - other.transform.position;

                FullPiece.transform.position -= distance;

                FullPiece.transform.SetParent(TableTransform, true);
                //FullPiece.GetComponent<Renderer>().sortingLayerName = "Pieces";

                gameController.thisPlayerAmountOfCards--;
                serverData.SetPieceOn(FullPiece.name, FullPiece.position, TableTransform.position, FullPiece.rotation, true);

                FullPiece.GetComponent<Renderer>().sortingLayerName = "Pieces";

                foreach (Renderer child in FullPiece.GetComponentsInChildren<Renderer>())
                {
                    child.sortingLayerName = "Pieces";
                }

                this.GetComponentInParent<HalfPiece>().halfPieceConnected = true;
                other.GetComponentInParent<HalfPiece>().halfPieceConnected = true;

                for(int i = 0; i < 2; i++)
                {
                    if (GetComponentInParent<DraggablePiece>().ValuesInThisPiece[i] != value)
                    {
                        otherValueOfThis = GetComponentInParent<DraggablePiece>().ValuesInThisPiece[i];
                    }

                }

                if (GetComponentInParent<DraggablePiece>().isDouble)
                    otherValueOfThis = value;

                serverData.ValuesToPut(otherValueOfThis, value);

                this.FullPiece.GetComponent<DraggablePiece>().ReturnToWhite();

                gameController.OnTurnCompleted();
            }
            else
            {
                //Debug.Log(other.gameObject.name + " got a different value: " + otherValue + " // this: " + value );
            }
        }
        else
        {
            //Debug.Log(other.gameObject.name + " is not a Border Piece");
        }
    }

}
