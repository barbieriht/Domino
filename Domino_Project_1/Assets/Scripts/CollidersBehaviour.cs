using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidersBehaviour : MonoBehaviour
{
    private int value;
    private int otherValue;
    //private bool isBorder;
    public bool canAttach;
    public bool isHolding;

    public Collider2D HPieceCollider;

    //private Transform TableTransform;
    public Transform FullPiece;

    void Start()
    {
        HPieceCollider = this.GetComponent<Collider2D>();
        //TableTransform = GameObject.FindGameObjectWithTag("Table").transform;
        FullPiece = this.transform.parent.transform.parent;

        value = GetComponentInParent<PieceBehaviour>().GetValue();
        //isBorder = GetComponentInParent<PieceBehaviour>().GetIsBorder();
        //isOnHand = transform.parent.GetComponentInParent<PieceBehaviour>().GetIsOnHand();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //verifica se a peça atual é borda
        //isBorder = GetComponentInParent<PieceBehaviour>().GetIsBorder();
        //Debug.Log(this.name + " is colliding with " + other.gameObject.name);

        if(other.gameObject.CompareTag("FullPiece"))
        {
            return;
        }

        if (isHolding == true)
            return;

        canAttach = !this.GetComponentInParent<HalfPiece>().halfPieceConnected;

        //se ambos colisores são do "lado" da peça, elas não podem se juntar
        if (!canAttach || !other.gameObject.GetComponent<CollidersBehaviour>().canAttach)
            return;

        //se ambas as peças são da mão do jogador, não podem se juntar
        if (FullPiece.transform.parent == other.GetComponent<CollidersBehaviour>().FullPiece.transform.parent)
            return;

        //se o outro objeto tambem é borda
        if (!other.gameObject.GetComponentInParent<HalfPiece>().halfPieceConnected)
        {
            Debug.Log(other.gameObject.name + " is a Border Piece");

            //lê o valor da peça oposta
            otherValue = other.gameObject.GetComponent<CollidersBehaviour>().value;

            //compara os valores
            if (otherValue == this.value)
            {
                Debug.Log(other.gameObject.name + " got the same value: " + otherValue);

                //transforma o objeto atual em um "filho" da mesa
                if(FullPiece.transform.parent == GameObject.FindGameObjectWithTag("Table").transform)
                {
                    return;
                }

                FullPiece.transform.SetParent(GameObject.FindGameObjectWithTag("Table").transform, true);
                
                //faz com que as peças posicionadas não sejam mais consideradas bordas
                //this.GetComponentInParent<PieceBehaviour>().isBorder = false;
                //other.GetComponentInParent<PieceBehaviour>().isBorder = false;
                this.GetComponentInParent<HalfPiece>().halfPieceConnected = true;
                other.GetComponentInParent<HalfPiece>().halfPieceConnected = true;
            }
            else
            {
                Debug.Log(other.gameObject.name + " got a different value: " + otherValue + " // this: " + value );
            }
        }
        else
        {
            Debug.Log(other.gameObject.name + " is not a Border Piece");
        }
    }
}
