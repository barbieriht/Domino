using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    private GameObject[] Pieces;
    private bool[] auxPieces;
    private GameObject FirstPiece;

    private Transform PlayerHand;

    private void Start()
    {
        Pieces = GameObject.FindGameObjectsWithTag("FullPiece");
        PlayerHand = GameObject.FindGameObjectWithTag("PlayerHand").transform;
        auxPieces = new bool[28];

        for (int j = 0; j < 28; j++)
        {
            auxPieces[j] = false;
            Pieces[j].transform.position = new Vector3(0,0,0);
            Pieces[j].SetActive(false);
        }

        FirstPiece = TurnPieceVisibleGO();
        FirstPiece.transform.SetParent(GameObject.FindGameObjectWithTag("Table").transform);
    }

    public GameObject TurnPieceVisibleGO()
    {
        int i = Random.Range(0, 27);
        while (auxPieces[i] == true)
        {
            i = Random.Range(0, 27);
        }
        Pieces[i].SetActive(true);
        auxPieces[i] = true;

        return Pieces[i];
    }

    public void TurnPieceVisible()
    {
        int i = Random.Range(0, 27);
        while (auxPieces[i] == true)
        {
            i = Random.Range(0, 27);
        }
        Pieces[i].transform.SetParent(PlayerHand, true);
        Pieces[i].transform.position = new Vector3(0, -2, 0);
        Pieces[i].SetActive(true);

        auxPieces[i] = true;
    }
}
