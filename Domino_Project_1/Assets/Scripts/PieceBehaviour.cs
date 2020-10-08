using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    public int value = 1;
    int otherValue;

    public bool isBorder = true;

    /*
    public bool GetIsOnHand()
    {
        if (transform.parent.CompareTag("PlayerHand"))
            return true;
        return false;
    }
    */

    public int GetValue()
    {
        return this.value;
    }

    public bool GetIsBorder()
    {
        return isBorder;
    }

    public void SetValue(int x)
    {
        value = x;
    }

    public void SetBool(bool b)
    {
        isBorder = b;
    }
}
