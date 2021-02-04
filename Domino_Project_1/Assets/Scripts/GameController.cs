using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameController : MonoBehaviourPun
{
    public GameObject[] Pieces;

    public bool[] fullDeck;

    private int[] allCards = new int[28];
    private int[] cardsToGive1 = new int[7];
    private int[] cardsToGive2 = new int[7];
    private int[] cardsToGive3 = new int[7];
    private int[] cardsToGive4 = new int[7];

    private float timeToPlay;

    public static List<int> AllAvailableValues = new List<int>();

    public int amountOfCards = 28;
    public int thisPlayerAmountOfCards = 7;

    public ServerData serverData;

    private GameObject Table;
    private Transform playerHand;


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

        /*
        if (PhotonNetwork.IsMasterClient)
            serverData.Distribute();
        */

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

        if(Pieces[j].GetComponent<DraggablePiece>().isDouble)
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
            return;

        int j = RandomPiece();

        GameObject thisPiece = Pieces[j];

        //serverData.PrintText("Sorteada: " + j);

        if (thisPiece == null)
            Debug.LogError("thisPiece deu ruim");

        thisPiece.transform.SetParent(playerHand, true);
      
        for (int k = 0; k < 2; k++)
            AllAvailableValues.Add(Pieces[j].GetComponent<DraggablePiece>().ValuesInThisPiece[k]);

        serverData.SubtractCard();

        fullDeck[j] = true;
        serverData.SavePickedPieces(j, true);

        if(amountOfCards == 0)
        {
            //ativar o botão de passar a rodada
        }

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
        timeToPlay = 30;

        //serverData.PrintText("Iniciou o round: " + serverData.RoundNumber);

        serverData.roundNumberTxt.text = "Round: " + serverData.RoundNumber.ToString();

        if (!serverData.IsMyTurn())
        {
            //mostrar de quem é o turno
        }
        else
        {
            if(serverData.RoundNumber == 1)
            {
                //destacar a maior bomba
            }
            //mostrar que é meu turno
            //ativar o botão de comprar cartas
            //atualizar as cartas que podem ser colocadas
            //destacar as minhas cartas que podem ser jogadas
        }
    }

    public void OnTurnCompleted()
    {

        if (thisPlayerAmountOfCards == 0)
        {
            OnPlayerWin(PhotonNetwork.NickName);
        }

        serverData.SetRoundNumber(serverData.RoundNumber + 1);
    }

    public void OnTurnTimeEnds()
    {
        serverData.SetRoundNumber(serverData.RoundNumber + 1);
    }

    public void OnPlayerWin(string nick)
    {
        //finalizar o jogo
    }

    public void OnPiecesOver()
    {
        //finalizar o jogo
    }
}
