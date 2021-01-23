using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EmergencyButton : MonoBehaviour
{
    public GameObject VotePanel;
    
    private PhotonView photonView;


    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
    }


    public void startVoteCall()
    {
        if (newPlayerManager.LocalPlayerInstance.GetComponent<newPlayerManager>().isDead == true)
            return;
        
        var playerGO = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in playerGO)
        {
            if (p.GetComponent<PhotonView>().IsMine)
            {
                p.GetComponent<newPlayerManager>().report = true;
            }
        }
        photonView.RPC("starVotePanel", RpcTarget.All);
    }
    
    [PunRPC]
    public void starVotePanel()
    {
        VotePanel.SetActive(true);
    }
    
    
}
