using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameController : MonoBehaviourPun, IPunTurnManagerCallbacks
{
    public GameObject[] Pieces;
    [SerializeField]
    public bool[] fullDeck;
    string thisPieceName;
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

        AddPlayerOnList();

        //serverData.PrintAllPlayers();

        if (PhotonNetwork.IsMasterClient)
            serverData.Distribute();

        serverData.SelectBiggestBomb();
        OrganizePlayerList();

        //serverData.PrintAllPlayers();
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


    public void TurnPieceVisible()
    {
        if (amountOfCards == 0)
            return;

        /*int j = Random.Range(0, 10000)%28;
        while (fullDeck[j] == true)
        {
            j = Random.Range(0, 10000)%28;
        }*/

        int j = serverData.AddOnListOfCards();

        //Debug.Log("Numero sorteado: " + j);

        if (fullDeck == null)
            Debug.LogError("fullDeck deu ruim");

        GameObject thisPiece = Pieces[j];

        if (thisPiece == null)
            Debug.LogError("thisPiece deu ruim");

        thisPiece.transform.SetParent(playerHand, true);
        thisPiece.transform.position = new Vector3(0, 0, 0);
        thisPiece.transform.rotation = new Quaternion(0, 0, 0, 0);

        if(Pieces[j].GetComponent<DraggablePiece>().isDouble && Pieces[j].GetComponentInChildren<PieceBehaviour>().value > serverData.biggestBomb)
            serverData.biggestBomb = Pieces[j].GetComponentInChildren<PieceBehaviour>().value;

        serverData.SubtractCard();
        serverData.SavePickedPieces(j, true);
        serverData.PrintInformationTest(PhotonNetwork.NickName, j, amountOfCards);
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
