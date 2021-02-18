using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PieceBehaviour : MonoBehaviour
{
    public int value = 1;
    private int index;
    private int cardNumber;
    public bool upSide;

    private SpriteRenderer spriteRenderer;
    private Sprite sprite;

    private string pieceName;
    // public bool isBorder = true;

    private void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        pieceName = this.transform.parent.name;


        if (value != (int)char.GetNumericValue(pieceName[6]))
            index = (int)char.GetNumericValue(pieceName[6]);
        else
            index = (int)char.GetNumericValue(pieceName[10]);

        cardNumber = this.GetComponentInParent<DraggablePiece>().cardNumber;
        sprite = Resources.Load<Sprite>(this.value.ToString() + "-" + index.ToString());
        spriteRenderer.sprite = sprite;
    }

    public int GetValue()
    {
        return this.value;
    }

   /* public bool GetIsBorder()
    {
        return isBorder;
    }*/

    public void SetValue(int x)
    {
        value = x;
    }

    [PunRPC]
    public void TurnActive(bool x)
    {
        this.gameObject.SetActive(x);
    }

    /*public void SetBool(bool b)
    {
        isBorder = b;
    }*/
}
