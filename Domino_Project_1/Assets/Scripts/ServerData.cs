using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerData : MonoBehaviourPunCallbacks
{
    private GameController gameController;
    public GameObject[] Pieces;
    public Transform tableTransform;
    public Transform playerHand;

    private void Start()
    {
        gameController = this.GetComponent<GameController>();
        Pieces = GameObject.FindGameObjectsWithTag("FullPiece");
        tableTransform = GameObject.FindGameObjectWithTag("Table").transform;
        playerHand = GameObject.FindGameObjectWithTag("PlayerHand").transform;
    }

    public void SetPieceOff(string pieceName) => photonView.RPC("SetPieceOffPUN", RpcTarget.All, pieceName);

    public void SetPieceOn(string namePiece, Vector3 position, Quaternion rotation, bool parent) => photonView.RPC("SetPieceOnPUN", RpcTarget.All, namePiece, position, rotation, parent);

    public void SavePickedPieces(int i, bool option) => photonView.RPC("SavePickedPiecesPUN", RpcTarget.All, i, option);

    public void SavePlayersData(string playerNick, int amountPieces) => photonView.RPC("SavePlayersDataPUN", RpcTarget.All, playerNick, amountPieces);

    public void Distribute() => photonView.RPC("DistributePUN", RpcTarget.All);

    public void RefreshAmountOfCards(int amount) => photonView.RPC("RefreshAmountOfCardsPUN", RpcTarget.All, amount);


    [PunRPC]
    void SetPieceOffPUN(string pieceName)
    {
        for(int i = 0; i < Pieces.Length; i++)
        {
            if(Pieces[i].name == pieceName)
            {
                Pieces[i].GetComponent<DraggablePiece>().Destroy();
            }
        }
    }

    [PunRPC]
    void SetPieceOnPUN(string namePiece, Vector3 position, Quaternion rotation, bool parent)
    {
        GameObject thisPiece = GameObject.Find(namePiece);
        if (parent)
            thisPiece.transform.SetParent(tableTransform, true);
        else
            thisPiece.transform.SetParent(playerHand, true);
        thisPiece.transform.position = position;
        thisPiece.transform.rotation = rotation;
    }

    [PunRPC]
    void SavePickedPiecesPUN(int i, bool option)
    {
       
        if(option)
        {
            gameController.fullDeck[i] = true;
            
        }
        else
            gameController.fullDeck[i] = false;

    }

    
    [PunRPC]
    void DistributePUN()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber != this.photonView.Owner.ActorNumber)
                return;

            Debug.Log("ActorNumber = " + player.ActorNumber + "   photonView.ActorNumber = " + this.photonView.Owner.ActorNumber);

            for (int k = 0; k < 7; k++)
            {
                int i = Random.Range(0, 27);
                while (gameController.fullDeck[i] == true)
                {
                    i = Random.Range(0, 27);
                }

                GameObject thisPiece = Pieces[i];

                thisPiece.transform.SetParent(playerHand, true);
                thisPiece.transform.position = new Vector3(0, 0, 0);
                thisPiece.transform.rotation = new Quaternion(0, 0, 0, 0);

                SavePickedPieces(i, true);
            }
            
        }
    }
    
    [PunRPC]
    void RefreshAmountOfCardsPUN(int amount)
    {
        gameController.amountOfCards = amount;
    }

    [PunRPC]
    void SavePlayersDataPUN(string playerNick, int amountPieces)
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if(player.NickName == playerNick)
            {
                player.PiecesAmount(amountPieces, true);
            }
        }
    }

}
