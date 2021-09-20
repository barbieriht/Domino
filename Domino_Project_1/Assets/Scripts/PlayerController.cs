using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public int cardsInHand { get; set; }
    public string nickName { get; set; }
    public int biggestBombInHand { get; set; }

    private string typeName;

    void Start()
    {
        BetterStreamingAssets.Initialize();

        typeName = BetterStreamingAssets.ReadAllText("config-type.txt");
    }
}
