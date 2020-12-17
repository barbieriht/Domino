using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBehaviour : MonoBehaviour
{
    public GameObject[] Pieces;
    public bool[] auxPieces;

    public Transform PlayerHand;

    private void Start()
    {
        Pieces = new GameObject[27];
        auxPieces = new bool[27];

        for(int j = 0; j<28; j++)
        {
            auxPieces[j] = false;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }
}
