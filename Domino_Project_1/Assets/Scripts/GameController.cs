using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameController : MonoBehaviourPun, IPunTurnManagerCallbacks
{
    public GameObject[] Pieces;
    [SerializeField]

    public bool[] fullDeck;

    public int[] allCards = new int[28];
    public int[] cardsToGive1 = new int[7];
    public int[] cardsToGive2 = new int[7];
    public int[] cardsToGive3 = new int[7];
    public int[] cardsToGive4 = new int[7];


    public int amountOfCards = 28;
    public ServerData serverData;

    
    //PlayerController playerController;

    //private GameObject FirstPiece;

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

        OrganizePlayerList();

        //serverData.PrintAllPlayers();
    }

    public void DistributeByArrays()
    {
        for(int i = 0; i < 28; i++)
        {
            allCards[i] = i;
        }

        embaralhar(allCards);

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

    static void embaralhar(int[] lista)
    {

        for (int i = 0; i < lista.Length; i++)
        {
            int a = Random.Range(0, 10000) % 28;
            int temp = lista[i];
            lista[i] = lista[a];
            lista[a] = temp;
        }
    }

    public void OrganizePlayerList()
    {
        serverData.OrganizePlayerListpt1();
        serverData.OrganizePlayerListpt2();

    }
    public void AddPlayerOnList()
    {
        serverData.AddPlayer(PhotonNetwork.NickName);
        //serverData.AddPlayerPC(playerController);
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


        serverData.SubtractCard();
        serverData.SavePickedPieces(j, true);
        //serverData.PrintInformationTest(PhotonNetwork.NickName, j, amountOfCards, index);
    }

    private void ResetCards(bool[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = false;
        }
    }

    void IPunTurnManagerCallbacks.OnTurnBegins(int turn)
    {

    }

    void IPunTurnManagerCallbacks.OnTurnCompleted(int turn)
    {

    }

    void IPunTurnManagerCallbacks.OnPlayerMove(Player player, int turn, object move)
    {

    }

    void IPunTurnManagerCallbacks.OnPlayerFinished(Player player, int turn, object move)
    {

    }

    void IPunTurnManagerCallbacks.OnTurnTimeEnds(int turn)
    {

    }
}
