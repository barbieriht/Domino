using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviour
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

    [PunRPC]
    public GameObject TurnPieceVisibleGO()
    {
        int i = Random.Range(0, 27);
        while (auxPieces[i] == true)
        {
            i = Random.Range(0, 27);
        }
        Pieces[i].SetActive(true);
        //string thisPieceName = GetPieceName(Pieces[i]);
        //PhotonNetwork.Instantiate(Pieces[i].name, new Vector3(0, 0, 0), Quaternion.identity);
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
        Pieces[i].SetActive(true);
        
        auxPieces[i] = true;
    }
    /*
    public string GetPieceName(GameObject Piece)
    {
        string namePiece;
        int mvalue = 0;
        int Mvalue = 0;
        int x = 0;
        int y = 0;

        int childs;

        childs = Piece.transform.childCount;

        for(int i = 0; i < childs; i++)
        {
            if (i == 0)
                x = Piece.transform.GetChild(i).GetComponent<PieceBehaviour>().GetValue();
            else
                y = Piece.transform.GetChild(i).GetComponent<PieceBehaviour>().GetValue();
        }

        OrganizeValues(x, y, mvalue, Mvalue);

        namePiece = "Piece " + mvalue + " - " + Mvalue;

        return namePiece;
    }

    private void OrganizeValues(int i, int j, int mvalue, int Mvalue)
    {
        if(i < j)
        {
            mvalue = i;
            Mvalue = j;
        }
        else
        {
            mvalue = j;
            Mvalue = i;
        }
    }*/
}
