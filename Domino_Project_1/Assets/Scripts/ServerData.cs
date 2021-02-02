using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class ServerData : MonoBehaviourPun
{
    public GameController gameController;
    public GameObject[] Pieces;
    public Transform tableTransform;
    public Transform playerHand;

    public Text playerNickName;

    public bool isFirst = false;
    public int biggestBomb = 0;
    public int biggestGlobalBomb = 0;
    public int playerIndex = 0;

    public static List<string> AllPlayers = new List<string>();
    public static List<string> AllPlayersOrganized = new List<string>();
    public static List<int> CardsAlreadyGived = new List<int>();
    public static List<int> AllBiggestBombs = new List<int>();


    public int RoundNumber
    {
        get => this.RoundNumber;
        set => this.photonView.RPC("RpcSetRoundNumber", RpcTarget.All, (object)value);
    }

    private void Start()
    {
        Pieces = GameObject.FindGameObjectsWithTag("FullPiece");
        tableTransform = GameObject.FindGameObjectWithTag("Table").transform;
        playerHand = GameObject.FindGameObjectWithTag("PlayerHand").transform;
        playerNickName.text = PhotonNetwork.NickName;
    }


    public void AddPlayer(string thisNickName) => photonView.RPC("AddPlayerPUN", RpcTarget.All, thisNickName);

    public void AddOnListOrganized(string nick) => photonView.RPC("AddOnListOrganizedPUN", RpcTarget.All, nick);

    public void AddMyBombs(int bomb) => photonView.RPC("AddMyBombsPUN", RpcTarget.MasterClient, bomb);

    public void SortBombs() => photonView.RPC("SortBombsPUN", RpcTarget.MasterClient);

    public void PrintAllPlayers() => photonView.RPC("PrintAllPlayersPUN", RpcTarget.All);

    public void SetPieceOff(string pieceName) => photonView.RPC("SetPieceOffPUN", RpcTarget.All, pieceName);

    public void SetPieceOn(string namePiece, Vector3 position, Quaternion rotation, bool parent) => photonView.RPC("SetPieceOnPUN", RpcTarget.Others, namePiece, position, rotation, parent);

    public void SavePickedPieces(int i, bool option) => photonView.RPC("SavePickedPiecesPUN", RpcTarget.All, i, option);

    public void SavePlayersData(string playerNick, int amountPieces) => photonView.RPC("SavePlayersDataPUN", RpcTarget.All, playerNick, amountPieces);

    //public void Distribute() => photonView.RPC("DistributePUN", RpcTarget.All);

    public void DistributeByArray(int[] array1, int[] array2, int[] array3, int[] array4) => photonView.RPC("DistributeByArrayPUN", RpcTarget.All, array1, array2, array3, array4);

    public void SetBiggestBomb(int bb) => photonView.RPC("SetBiggestBombPUN", RpcTarget.All, bb);

    public void Select1stPlayer() => photonView.RPC("Select1stPlayerPUN", RpcTarget.All);

    public void SubtractCard() => photonView.RPC("SubtractCardPUN", RpcTarget.All);

    public void ItsTheBiggest(int bomb) => photonView.RPC("ItsTheBiggestPUN", RpcTarget.All, bomb);

    public void PrintInformationTest(string nickname, int cardnumber, int amount, int index) => photonView.RPC("PrintTestPUN", RpcTarget.All, nickname, cardnumber, amount, index);

    public void PrintAllBombs() => photonView.RPC("PrintAllBombsPUN", RpcTarget.All);

    public void PrintText(string text) => photonView.RPC("PrintTextPUN", RpcTarget.MasterClient, text);

    [PunRPC]
    void PrintTextPUN(string text)
    {
        Debug.Log(text);
    }

    public void AddFirstPlayer()
    {
        if (isFirst)
        {
            AddOnListOrganized(PhotonNetwork.NickName);

            foreach(string nick in AllPlayers)
            {
                if(nick != PhotonNetwork.NickName)
                {
                    AddOnListOrganized(PhotonNetwork.NickName);
                }
            }
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
    void AddMyBombsPUN(int bomb)
    {
        AllBiggestBombs.Add(bomb);
        Debug.Log("Added bomb: " + bomb);
    }

    [PunRPC]
    void SortBombsPUN()
    {
        AllBiggestBombs.Sort();
        //PrintAllBombs();
        biggestGlobalBomb = AllBiggestBombs[AllBiggestBombs.Count - 1];
        PrintText("The biggest bomb is " + biggestGlobalBomb);
        SetBiggestBomb(biggestGlobalBomb);
        Select1stPlayer();
    }

    [PunRPC]
    void SetBiggestBombPUN(int bb)
    {
        biggestGlobalBomb = bb;
    }

    [PunRPC]
    void Select1stPlayerPUN()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == PhotonNetwork.NickName)
            {
                if (biggestBomb == biggestGlobalBomb)
                {
                    isFirst = true;
                    PrintText(PhotonNetwork.NickName + " got the biggest bomb: " + biggestGlobalBomb);
                }
            }
        }
    }

    [PunRPC]
    void PrintAllBombsPUN()
    {
        foreach(int bomb in AllBiggestBombs)
        {
            Debug.Log("Bomb: " + bomb);
        }
    }

    [PunRPC]
    void PrintTestPUN(string nick, int card, int amount, int index)
    {
        Debug.Log(nick + ": " + card + " (" + amount + ") " + index);
    }

    [PunRPC]
    void AddOnListOfCardsPUN(int number)
    {
        CardsAlreadyGived.Add(number);
    }


    [PunRPC]
    void AddOnListOrganizedPUN(string nick)
    {
        AllPlayersOrganized.Add(nick);
    }


    [PunRPC]
    void ItsTheBiggestPUN(int bomb)
    {
        biggestGlobalBomb = bomb;
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

    /*
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
    */

    [PunRPC]
    void DistributeByArrayPUN(int[] array1, int[] array2, int[] array3, int[] array4)
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == PhotonNetwork.NickName)
            {
                break;
            }
            playerIndex++;
        }

        switch (playerIndex)
        {
            case 0:
                for (int i = 0; i < array1.Length; i++)
                {
                    gameController.TurnPieceVisible(array1[i], playerIndex);
                }
                break;
            case 1:
                for (int i = 0; i < array2.Length; i++)
                {
                    gameController.TurnPieceVisible(array2[i], playerIndex);
                }
                break;
            case 2:
                for (int i = 0; i < array3.Length; i++)
                {
                    gameController.TurnPieceVisible(array3[i], playerIndex);
                }
                break;
            case 3:
                for (int i = 0; i < array4.Length; i++)
                {
                    gameController.TurnPieceVisible(array4[i], playerIndex);
                }
                break;
            default:
                break;
        }

        if(playerIndex + 1 == PhotonNetwork.PlayerList.Length)
        {
            PrintText("PlayersWithCards: " + PhotonNetwork.PlayerList.Length);
            SortBombs();
        }
    }


    [PunRPC]
    void SubtractCardPUN()
    {
        gameController.amountOfCards--;
    }


    [PunRPC]
    void SavePickedPiecesPUN(int i, bool option)
    {

        if (option)
        {
            gameController.fullDeck[i] = true;

        }
        else
            gameController.fullDeck[i] = false;

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
