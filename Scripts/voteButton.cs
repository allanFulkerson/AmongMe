using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class voteButton : MonoBehaviour
{

    public cardLogic cl;
    
    public void Vote()
    {
        var playerManager = newPlayerManager.LocalPlayerInstance.GetComponent<newPlayerManager>();
        VoteManager myVM = cl.VM;

        //Debug.Log(newPlayerManager.LocalPlayerInstance.GetPhotonView().IsMine + " owner active " + newPlayerManager.LocalPlayerInstance.GetComponent<newPlayerManager>().playerNameText.text );
       
       if(playerManager.photonView.IsMine && playerManager.hasVoted != true);
        {

            myVM.AgainstVote(playerManager,cl.thisPlayer);
            cl.GetComponent<Button>().interactable = false;
            gameObject.transform.parent.transform.gameObject.SetActive(false);
            playerManager.hasVoted = true;

        }
        
         
       
    }
}
