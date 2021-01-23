using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class cardLogic : MonoBehaviourPunCallbacks
{
    public Button ButtonC;
    public Image characterImg;

    public Image DeadImg;

    public Image AlertImg;

    public Image VotedImg;

    public Text ChaNameTxt;
    
    public String uniqId;

    

    public VoteManager VM;

    public GameObject ButtonsContainer;
    
    public GameObject VotedPlayerContainers;

    public newPlayerManager thisPlayer;
    public GameObject playerCountImage;
    public List<GameObject> playerCountList;
    public GameObject BlockPanel;
    public PhotonView CardManagerPhotonView;
    public cardManager CM;
    private bool hasCreateCm = false;

    void Start()
    {
        var parent = GameObject.FindWithTag("cardParent");
        transform.SetParent(parent.transform);
        transform.localScale = Vector3.one;

        if (thisPlayer.photonView.IsMine && VM.cardsForAll.Equals(VM._allPlayerManagers.Count))
        {
            hasCreateCm = true;
            createManager();


        }
    }

    public void createManager()
    {
        var cardManager = PhotonNetwork.Instantiate("cardManager", new Vector3(0, 0, 0), Quaternion.identity);
                     var cml = cardManager.GetComponent<cardManager>();
                     cml.testID = thisPlayer.uniqID;
                     cml.newStart();
                     cml.uniqIdCall();
    }


    void Update()
    {
        if (thisPlayer != null)
        {
            if (thisPlayer.report)
            {
                AlertImg.enabled = true;
            }
        }

        if (hasCreateCm != true)
        {
            if(VM.cardsForAll.Equals(VM._allPlayerManagers.Count)&&thisPlayer.photonView.IsMine)
            {
                hasCreateCm = true;
                createManager();
            }
        }
    }

    public void alertCall()
    {
       CM.alertCall();
    }
    public void startBlockPanel()
    {
        if (thisPlayer.photonView.IsMine)
            if (thisPlayer.isDead)
                BlockPanel.SetActive(true);
      
    }
    public void ToggleButtons ()
    {
        if(ButtonsContainer.activeSelf)
            ButtonsContainer.SetActive(false);
        else
            ButtonsContainer.SetActive(true);
            
    }

    public void addPlayerCountImage()
    {
        var image = Instantiate(playerCountImage, VotedPlayerContainers.transform);
        playerCountList.Add(image);
    }

    public void ResetPlayerCountList()
    {
        playerCountList.Clear();
        playerCountList.Capacity = 0;
    }

    public void resetImages()
    {
        AlertImg.enabled = false;
        VotedImg.enabled = false;
        ButtonC.interactable = true;
        if (thisPlayer.isDead != true)
            GetComponent<Button>().interactable = true;

    }
}
