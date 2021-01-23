using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
public class body : MonoBehaviour, IPunObservable
{

    public SpriteRenderer bodysprite;
    public PhotonView pv;
    public Button reportButton;
    public Text debugText;

    public newGameManager gm;


    public void Start()
    {
        pv = GetComponent<PhotonView>();
        gm = newGameManager.Instance;
        newPlayerManager npm = newPlayerManager.localNpm;
        if (npm.allBodies != null)
        {
            npm.allBodies.Add(transform);
        }

        reportButton = newPlayerManager.localNpm.reportButton;
    }

    public void setColor(Color newColor)
    {
        bodysprite.color = newColor;
    }
    
    public void Report(newGameManager gameManager)
    {
        if(gameManager != null)
            gm = gameManager;
        
        pv.RPC("ReportRpc",RpcTarget.All);
    }

    [PunRPC]
    public void ReportRpc()
    {
        Debug.Log("REPORTED");
        gm.VoteScreen.SetActive(true);
        //Destroy(gameObject);
        
    }
    /*
    *Test purpose
    */
    /*public void onView()
    {
        Debug.Log("Im seen");
    }
    
    public void enView()
    {
        Debug.Log("Im not Seen");
    }
*/
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       if (stream.IsWriting)
       {
           var test = bodysprite.color;
           Vector3 tempVectorColor  = new Vector3(test.r,test.g,test.b);
            stream.SendNext(tempVectorColor);
            
        }
        else
        {
            var tempVector = (Vector3)stream.ReceiveNext();
            bodysprite.color = new Color(tempVector.x,tempVector.y,tempVector.z);

        }
    }
}
