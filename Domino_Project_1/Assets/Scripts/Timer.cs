using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
public class Timer : MonoBehaviour
{
    float countdown = 30f;
    float startTime;

    public Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        startTime = (float)PhotonNetwork.Time;
        StartCoroutine(WaitForDestroy(countdown));
    }

    // Update is called once per frame
    void Update()
    {
        float timer = (float)PhotonNetwork.Time - startTime;
        float countdownTemp = countdown - timer;

        string seconds = (countdown % 60).ToString("00");

        if (countdownTemp < 0.0f)
            return;

        timerText.text = seconds;
    }

    IEnumerator WaitForDestroy(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }

    public void DestroyTimer()
    {
        Destroy(this.gameObject);
    }
}
