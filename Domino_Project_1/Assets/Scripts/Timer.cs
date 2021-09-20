using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
public class Timer : MonoBehaviour
{
    float countdown = 30f;
    float startTime;

    public GameController gameController;
    public ServerData serverData;

    public Color orangeColor;

    public Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        serverData = GameObject.Find("GameController").GetComponent<ServerData>();

        startTime = (float)Time.time;
        //StartCoroutine(WaitForDestroy(countdown));
    }

    // Update is called once per frame
    void Update()
    {
        float timer = (float)Time.time - startTime;
        float countdownTemp = countdown - timer;

        string seconds = (countdownTemp % 60).ToString("00");

        if (countdownTemp > 15.5f)
            timerText.color = Color.white;
        else if (countdownTemp > 5.5f && countdownTemp <= 15.5f)
            timerText.color = orangeColor;
        else
            timerText.color = Color.red;

        if (countdownTemp < 0.0f)
        {
            if(serverData.IsMyTurn())
                gameController.OnTurnTimeEnds();
            Destroy(this.gameObject);
        }

        timerText.text = seconds;
    }

    public void DestroyTimer()
    {
        Destroy(this.gameObject);
    }
}
