using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFile : MonoBehaviour
{
    Text txt;
    public string fileName;
    void Start()
    {
        txt = gameObject.GetComponent<Text>();

        BetterStreamingAssets.Initialize();

        txt.text = BetterStreamingAssets.ReadAllText(fileName + ".txt");
    }

}
