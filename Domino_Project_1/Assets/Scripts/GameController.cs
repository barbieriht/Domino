﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameController : MonoBehaviourPun
{
    public GameObject[] Pieces;

    public bool isGameFinished = false;

    public bool[] fullDeck;

    private int[] allCards = new int[28];
    private int[] cardsToGive1 = new int[7];
    private int[] cardsToGive2 = new int[7];
    private int[] cardsToGive3 = new int[7];
    private int[] cardsToGive4 = new int[7];

    public int sumLeftCards;
    public int sumLeftCardsGlobal;

    public static List<int> AllAvailableValues = new List<int>();
    [SerializeField]
    public List<string> PlayersWhoPassedTurn = new List<string>();
    public List<int> PiecesLeft = new List<int>();

    public int amountOfCards = 28;
    public int thisPlayerAmountOfCards = 7;

    public ServerData serverData;
    public ChatBehaviour chatBehaviour;

    public Canvas canvas;
    public GameObject Timer;
    private GameObject Table;
    private Transform playerHand;

    public Text FirstPlayerTxT;
    public Text WinnerTxT;
    public Image WinnerImage;
    public Button buyButton;
    public Button passButton;

    private void Start()
    {
        amountOfCards = 28;
        fullDeck = new bool[28];
        ResetCards(fullDeck);

        playerHand = GameObject.FindGameObjectWithTag("PlayerHand").transform;
        serverData = this.GetComponent<ServerData>();

        Table = GameObject.FindGameObjectWithTag("Table");

        if (Table == null)
            Debug.LogError("There's no table");


        //serverData.PrintAllPlayers();

        AddPlayerOnList();

        if (PhotonNetwork.IsMasterClient)
        {
            DistributeByArrays();
            serverData.DistributeByArray(cardsToGive1, cardsToGive2, cardsToGive3, cardsToGive4);
        }

        //serverData.PrintAllPlayers();
    }


    public void DistributeByArrays()
    {
        for(int i = 0; i < 28; i++)
        {
            allCards[i] = i;
        }

        Shuffle(allCards);

        for(int j = 0; j < 4; j++)
        {
            for(int k = 0; k < 7; k++)
            {
                switch(j)
                {
                    case 0:
                        cardsToGive1[k] = allCards[k];
                        break;
                    case 1:
                        cardsToGive2[k] = allCards[7 + k];
                        break;
                    case 2:
                        cardsToGive3[k] = allCards[14 + k];
                        break;
                    case 3:
                        cardsToGive4[k] = allCards[j * 7 + k];
                        break;
                }
            }
        }
    }

    public void Shuffle(int[] lista)
    {

        for (int i = 0; i < lista.Length; i++)
        {
            int a = RandomValue();
            int temp = lista[i];
            lista[i] = lista[a];
            lista[a] = temp;
        }
    }

    public void AddPlayerOnList()
    {
        serverData.AddPlayer(PhotonNetwork.NickName);
        //serverData.AddPlayerPC(playerController);
    }

    public int RandomValue()
    {
        return (Random.Range(0, 10000) % 28);
    }

    public int RandomPiece()
    {
        int x;
        x = (Random.Range(0, 10000) % 28);

        if (!fullDeck[x])
            return x;
        return (RandomPiece());
    }

    public void TurnPieceVisible(int j, int index)
    {
        if (amountOfCards == 0)
            return;


        //int j = serverData.AddOnListOfCards();

        //Debug.Log("Numero sorteado: " + j);

        if (fullDeck == null)
            Debug.LogError("fullDeck deu ruim");

        GameObject thisPiece = Pieces[j];

        if (thisPiece == null)
            Debug.LogError("thisPiece deu ruim");

        thisPiece.transform.SetParent(playerHand, true);
        thisPiece.transform.position = new Vector3(0, 0, 0);
        thisPiece.transform.rotation = new Quaternion(0, 0, 0, 0);

        //thisPiece.GetComponent<Renderer>().sortingLayerName = "PiecesInHand";

        if (Pieces[j].GetComponent<DraggablePiece>().isDouble)
        {
            serverData.AddMyBombs(Pieces[j].GetComponentInChildren<PieceBehaviour>().value);
            if (Pieces[j].GetComponentInChildren<PieceBehaviour>().value > serverData.biggestBomb)
                serverData.biggestBomb = Pieces[j].GetComponentInChildren<PieceBehaviour>().value;
        }

        for(int k = 0; k < 2; k++)
            AllAvailableValues.Add(Pieces[j].GetComponent<DraggablePiece>().ValuesInThisPiece[k]);

        serverData.SubtractCard();

        fullDeck[j] = true;
        serverData.SavePickedPieces(j, true);
        //serverData.PrintInformationTest(PhotonNetwork.NickName, j, amountOfCards, index);
    }

    public void PickAPiece()
    {
        if (amountOfCards <= 0)
        {
            passButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
            chatBehaviour.SendMessageToChat("Pieces over :(", Message.MessageType.warning);
            return;
        }

        int j = RandomPiece();

        GameObject thisPiece = Pieces[j];

        //serverData.PrintText("Sorteada: " + j);

        if (thisPiece == null)
            Debug.LogError("thisPiece deu ruim");

        if(thisPiece.GetComponent<DraggablePiece>().ChangeColor() > 0)
        {
            buyButton.gameObject.SetActive(false);
        }

        thisPiece.transform.SetParent(playerHand, true);

        //thisPiece.GetComponent<Renderer>().sortingLayerName = "PiecesInHand";

        for (int k = 0; k < 2; k++)
            AllAvailableValues.Add(Pieces[j].GetComponent<DraggablePiece>().ValuesInThisPiece[k]);

        serverData.SubtractCard();

        fullDeck[j] = true;
        serverData.SavePickedPieces(j, true);

        thisPlayerAmountOfCards++;
    }

    private void ResetCards(bool[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = false;
        }
    }

    public void OnTurnBegins()
    {

        if (serverData.RoundNumber == 2)
            FirstPlayerTxT.gameObject.SetActive(false);

        serverData.roundNumberTxt.text = "Round: " + serverData.RoundNumber.ToString();

        if (!serverData.IsMyTurn())
        {
            buyButton.gameObject.SetActive(false);
            passButton.gameObject.SetActive(false);

        }
        else
        {
            
            if (PlayersWhoPassedTurn != null)
            {
                photonView.RPC("PlayersPassedTurnPUN", RpcTarget.All, PhotonNetwork.NickName, false);
            }

            photonView.RPC("InstantiateTimerPUN", RpcTarget.All);

            if(serverData.RoundNumber != 1)
                chatBehaviour.SendMessageToChat(PhotonNetwork.NickName + " turn.", Message.MessageType.info);

            int aux = 0;

            foreach(DraggablePiece pieces in playerHand.GetComponentsInChildren<DraggablePiece>())
            {
                aux = aux + pieces.ChangeColor();
            }

            if (aux == 0)
            {
                buyButton.gameObject.SetActive(true);
            }
        }
    }

    public void OnTurnCompleted()
    {
        if(serverData.RoundNumber != 1)
        {
            photonView.RPC("DestroyTimerPUN", RpcTarget.All);

            chatBehaviour.SendMessageToChat(PhotonNetwork.NickName + " ended it's turn with " + thisPlayerAmountOfCards + " cards.", Message.MessageType.info);
        }
        else
            chatBehaviour.SendMessageToChat(PhotonNetwork.NickName + " had the biggest bomb!", Message.MessageType.info);

        foreach (DraggablePiece pieces in playerHand.GetComponentsInChildren<DraggablePiece>())
        {
            pieces.ReturnToWhite();
        }

        if (thisPlayerAmountOfCards == 0)
        {
            OnPlayerWin(PhotonNetwork.NickName);
            return;
        }

        serverData.SetRoundNumber(serverData.RoundNumber + 1);
    }

    public void OnPassedTurn()
    {
        photonView.RPC("DestroyTimerPUN", RpcTarget.All);

        photonView.RPC("PlayersPassedTurnPUN", RpcTarget.All, PhotonNetwork.NickName, true);

        chatBehaviour.SendMessageToChat(PhotonNetwork.NickName + " passed the turn.", Message.MessageType.info);

        serverData.SetRoundNumber(serverData.RoundNumber + 1);
    }

    public void OnTurnTimeEnds()
    {
        if (serverData.RoundNumber != 1)
        {
            photonView.RPC("DestroyTimerPUN", RpcTarget.All);

            chatBehaviour.SendMessageToChat(PhotonNetwork.NickName + " lost it's turn.", Message.MessageType.info);
        }

        foreach (DraggablePiece pieces in playerHand.GetComponentsInChildren<DraggablePiece>())
        {
            pieces.ReturnToWhite();
        }
        serverData.SetRoundNumber(serverData.RoundNumber + 1);
    }

    public void OnPlayerWin(string nick)
    {
        serverData.TheWinner(nick);
    }

    public void OnPiecesOver()
    {
        serverData.PrintText("OnPiecesOverrr");
    }

    [PunRPC]
    void InstantiateTimerPUN()
    {
        Instantiate(Timer, canvas.transform);

    }

    [PunRPC]
    void DestroyTimerPUN()
    {
        if (GameObject.FindGameObjectWithTag("Timer") != null)
            GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>().DestroyTimer();
    }

    [PunRPC]
    void PlayersPassedTurnPUN(string nick, bool add)
    {
        if(add)
            PlayersWhoPassedTurn.Add(nick);
        else
            PlayersWhoPassedTurn.Remove(nick);

        if (PlayersWhoPassedTurn.Count >= PhotonNetwork.PlayerList.Length)
        {
            OnPiecesOverPUN();
        }
    }

    void OnPiecesOverPUN()
    {
        int index = 0;
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if(player.NickName == PhotonNetwork.NickName)
            {
                int aux = 0;

                foreach (DraggablePiece pieces in playerHand.GetComponentsInChildren<DraggablePiece>())
                {
                    for (int i = 0; i < 2; i++)
                    {
                        aux += pieces.ValuesInThisPiece[i];
                    }
                }

                photonView.RPC("AddOnPiecesLeftPUN", RpcTarget.All, aux);
                index++;
            }
        }

        if(index == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SortPiecesLeftPUN", RpcTarget.All);
        }
    }

    [PunRPC]
    void AddOnPiecesLeftPUN(int amount)
    {
        PiecesLeft.Add(amount);
    }

    [PunRPC]
    void SortPiecesLeftPUN()
    {
        PiecesLeft.Sort();

        sumLeftCardsGlobal = PiecesLeft[0];

        if (sumLeftCards == sumLeftCardsGlobal)
        {
            OnPlayerWin(PhotonNetwork.NickName);
        }
    }
    
}
