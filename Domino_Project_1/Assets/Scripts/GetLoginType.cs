using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetLoginType : MonoBehaviour
{
    private string fileName = "room_code";
    private Text txt;

    void Start()
    {
        txt = gameObject.GetComponent<Text>();

        BetterStreamingAssets.Initialize();

        txt.text = BetterStreamingAssets.ReadAllText(fileName + ".txt");
    }

    public string GetRoomCode()
    {
        return txt.text;
    }
}
