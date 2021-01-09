using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerData : MonoBehaviourPun
{
    public GameController gameController;
    public GameObject[] Pieces;
    public Transform tableTransform;
    public Transform playerHand;
    public bool isFirst = false;
    public int biggestBomb = 0;
    public int biggestGlobalBomb = 0;
    public static List<string> AllPlayers = new List<string>();
    public static List<string> AllPlayersOrganized = new List<string>();
    public static List<int> CardsAlreadyGived = new List<int>();


    public int roundNumber
    {
        get => this.roundNumber;
        set => this.photonView.RPC("RpcSetRoundNumber", RpcTarget.All, (object)value);
    }

    private void Start()
    {
        Pieces = GameObject.FindGameObjectsWithTag("FullPiece");
        tableTransform = GameObject.FindGameObjectWithTag("Table").transform;
        playerHand = GameObject.FindGameObjectWithTag("PlayerHand").transform;
    }

    public void AddPlayer(string thisNickName) => photonView.RPC("AddPlayerPUN", RpcTarget.All, thisNickName);

    public void AddOnListOrganized(string nick) => photonView.RPC("AddOnListOrganizedPUN", RpcTarget.All, nick);


    public void PrintAllPlayers() => photonView.RPC("PrintAllPlayersPUN", RpcTarget.All);

    public void SetPieceOff(string pieceName) => photonView.RPC("SetPieceOffPUN", RpcTarget.All, pieceName);

    public void SetPieceOn(string namePiece, Vector3 position, Quaternion rotation, bool parent) => photonView.RPC("SetPieceOnPUN", RpcTarget.All, namePiece, position, rotation, parent);

    public void SavePickedPieces(int i, bool option) => photonView.RPC("SavePickedPiecesPUN", RpcTarget.All, i, option);

    public void SavePlayersData(string playerNick, int amountPieces) => photonView.RPC("SavePlayersDataPUN", RpcTarget.All, playerNick, amountPieces);

    public void Distribute() => photonView.RPC("DistributePUN", RpcTarget.All);

    public void RefreshAmountOfCards(int amount) => photonView.RPC("RefreshAmountOfCardsPUN", RpcTarget.All, amount);


    public void OrganizePlayerListpt1()
    {
        if (this.biggestBomb == biggestGlobalBomb)
            AddOnListOrganized(PhotonNetwork.NickName);
    }

    public void OrganizePlayerListpt2()
    {
        if (this.biggestBomb != biggestGlobalBomb)
            AddOnListOrganized(PhotonNetwork.NickName);
    }

    public void SelectBiggestBomb()
    {
        if(biggestBomb > biggestGlobalBomb)
        {
            ItsTheBiggest(biggestBomb);
        }
    }

    public int AddOnListOfCards()
    {
        int number = Random.Range(0, 10000) % 28;
        bool canPut = true;

        foreach(int card in CardsAlreadyGived)
        {
            if (number == card)
                canPut = false;
        }

        if (canPut == true)
        {
            photonView.RPC("AddOnListOfCardsPUN", RpcTarget.All, number);
            return number;
        }
        number = AddOnListOfCards();
        return number;
    }

    [PunRPC]
    void AddOnListOfCardsPUN(int number)
    {
        CardsAlreadyGived.Add(number);
    }


    public void ItsTheBiggest(int bomb) => photonView.RPC("ItsTheBiggestPUN", RpcTarget.All, bomb);

    [PunRPC]
    void AddOnListOrganizedPUN(string nick)
    {
        AllPlayersOrganized.Add(nick);
    }

    [PunRPC]
    void ItsTheBiggestPUN(int bomb)
    {
        this.biggestGlobalBomb = bomb;
    }

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
    void AddPlayerPUN(string thisNickName)
    {
        AllPlayers.Add(thisNickName);
    }


    [PunRPC]
    void PrintAllPlayersPUN()
    {
        foreach (string thisNickName in AllPlayers)
        {

            Debug.Log("NickName = " + thisNickName + " Bomb: " + this.biggestBomb);

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
        foreach (string thisNickName in AllPlayers)
        {
            if (thisNickName != PhotonNetwork.NickName)
                return;

            for (int k = 0; k < 7; k++)
            {
                if (gameController == null)
                    Debug.LogError("gamecontroller fudeu");
                gameController.TurnPieceVisible();
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
