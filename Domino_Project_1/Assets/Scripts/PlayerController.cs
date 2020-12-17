using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    PhotonView photonView;
    private void Awake()
    {
        photonView = this.gameObject.GetComponent<PhotonView>();
    }

    private void Start()
    {
        OnPlayerInstantiated();
    }

    private void OnPlayerInstantiated()
    {
        //se sou um jogador humano 
        if (this.photonView.IsMine)
        {
            
        }
    }
}
