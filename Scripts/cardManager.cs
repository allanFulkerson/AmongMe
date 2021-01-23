using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class cardManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public cardLogic CardLogicParent;
    public Image AlertImg;
    public Image VotedImg;
    public newPlayerManager thisPlayer;
    public PhotonView pv;
    public List<newPlayerManager> _allPlayerManagers;
    public GameObject[] playersGameObjects;
    public List<cardLogic> _CardLogics;
   
    public string testID;

    public void newStart()
    {

                _allPlayerManagers = new List<newPlayerManager>();
                _CardLogics = new List<cardLogic>();
                playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
                foreach (var p in playersGameObjects)
                {
                    _allPlayerManagers.Add(p.GetComponent<newPlayerManager>());
                  
                }

        
                var allClObject = GameObject.FindGameObjectsWithTag("Card");
                foreach (var ClP in allClObject)
                {
                    _CardLogics.Add(ClP.GetComponent<cardLogic>());
                }
            
                setClParent();
          
    }

    public void addVotedImg()
    {
        pv.RPC("againstRpc", RpcTarget.All);
    }

    [PunRPC]
    public void againstRpc()
    {
        var newImg = Instantiate(CardLogicParent.playerCountImage, Vector3.zero, Quaternion.identity,CardLogicParent.VotedPlayerContainers.transform);
 
    }
    public void uniqIdCall()
    {
        //Debug.Log("sending uniIdCall");
        pv.RPC("setUniqID",RpcTarget.Others, this.testID);
    }

    [PunRPC]
    public void setUniqID(string id)
    {
       // Debug.Log("Receive uniqidcall for " + id);
        testID = id;
        newStart();
    }
    
    public void setClParent()
    {
       // Debug.Log("3pASSING SET CL PARENT " + _CardLogics.Count);
           foreach (var cl in _CardLogics)
                {
                   // Debug.Log("the ids " + cl.thisPlayer.uniqID + " my id " + testID);
                    if(cl.thisPlayer.uniqID == testID)
                        transform.SetParent(cl.gameObject.transform);
                }
           editedStart();
    }
    public void editedStart()
    {
        
        CardLogicParent = transform.parent.GetComponent<cardLogic>();
        CardLogicParent.CM = this;
        AlertImg = CardLogicParent.AlertImg;
        VotedImg = CardLogicParent.VotedImg;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void alertCall()
    {
        pv.RPC("updateAlertRpc", RpcTarget.All);
    }
    
    [PunRPC]
    public void updateAlertRpc()
    {
        //Debug.Log("Le call venant de alertRPC");
        AlertImg.enabled = true;
    }

    public void votedCall()
    {
        pv.RPC("updateVotedRpc", RpcTarget.All);
    }
    
    [PunRPC]
    public void updateVotedRpc()
    {
        //Debug.Log("Le call venant de voteRPC");
        VotedImg.enabled = true;
    }
        
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(AlertImg.enabled);
            stream.SendNext(VotedImg.enabled);

        }else
        {
            AlertImg.enabled = (bool) stream.ReceiveNext();
            VotedImg.enabled = (bool) stream.ReceiveNext();
          
        }
    }
    
}
