using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class TasksManager : MonoBehaviourPunCallbacks
{
    public Transform parentTasks;
    public int nbTaks;
    public Text infoText;
    public Text TestTest;
    public int completedTaks = 0;
    private PhotonView pv;
    public GameObject winScreen;
    public GameObject[] AllTaks;
    
    public static TasksManager tasksManager;
     
    

        void Start()
        {
            
            tasksManager = this;
        
            pv = GetComponent<PhotonView>();
            infoText.text = "Pas de message";

             AllTaks = GameObject.FindGameObjectsWithTag("taks");

            foreach (var task in AllTaks)
            {
                nbTaks++;
            }

        setText();


        }
        
        public void rpcCall()
        {
            pv.RPC("taskUpdate",RpcTarget.All, completedTaks);
        }
        
        [PunRPC]
        void taskUpdate( int Message)
        {
            completedTaks = Message;
            setText();
            if(completedTaks == nbTaks)
                winScreen.SetActive(true);
           
        }

        void setText()
        {
            string test = " " + completedTaks + "/" + nbTaks + "";
            infoText.text = test ;
        }


}
