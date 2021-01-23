using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ColorCustomizer : MonoBehaviour
{

    public Color[] allColors;

    public void SetColor(int colorIndex)
    {
        var playerManager = newPlayerManager.LocalPlayerInstance.GetComponent<newPlayerManager>();
        //Debug.Log(newPlayerManager.LocalPlayerInstance.GetPhotonView().IsMine + " owner active " + newPlayerManager.LocalPlayerInstance.GetComponent<newPlayerManager>().playerNameText.text );
        if(playerManager.photonView.IsMine);
             playerManager.setColor(allColors[colorIndex]);
             playerManager.rpcColorCall();
    }

}
