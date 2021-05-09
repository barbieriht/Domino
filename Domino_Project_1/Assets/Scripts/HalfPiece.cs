using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPiece : MonoBehaviour
{
    public bool halfPieceConnected;

    void Start()
    {
        this.gameObject.tag = "HalfPiece";
        this.gameObject.layer = 13;
        halfPieceConnected = false;
    }

}
