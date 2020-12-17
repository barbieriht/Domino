using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PieceBehaviour : MonoBehaviour
{
    public int value = 1;

   // public bool isBorder = true;


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
