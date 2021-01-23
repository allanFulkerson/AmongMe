using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class newGameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public Canvas buttonCanvas;
    public static newGameManager Instance;
    public GameObject welcomeScreen;
    public GameObject EnemyScreen;
    public GameObject VoteScreen;
    public Text debugText;
    public Animator transition;
    public float transitionTime = 3f;
    
    public GameObject[] players;
    public List<newPlayerManager> _allPlayerManagers;
    

    public Transform[] spawnsPoints;

    void Start()
    {
        
        players = GameObject.FindGameObjectsWithTag("Player");
        _allPlayerManagers = new List<newPlayerManager>();
        EnemyScreen.GetComponent<EndScreenDelay>().GameManager = this;
         
        foreach (GameObject player in players)
        {
        
        _allPlayerManagers.Add(player.GetComponent<newPlayerManager>());             
        newPlayerManager npm = player.GetComponent<newPlayerManager>();
        npm.GameManager = this;
        npm.changeCameraValueByScene();
        npm.setCanvasButton(buttonCanvas);
        npm.transform.localPosition = spawnsPoints[npm.NumberInRoom].position;
        npm.DebugText = debugText;
        }
        
        welcomeScreen.GetComponent<welcomeScreen>().player = newPlayerManager.LocalPlayerInstance.GetComponent<newPlayerManager>();
        welcomeScreen.SetActive(true);
        welcomeScreen.GetComponent<welcomeScreen>().switchScreen();
        StartCoroutine(loading());
        Instance= this;
        StartCoroutine(welcomePanelDelay());
        
    }
    
    public void CheckNumberOfDeads()
    {
        int count=0;
        int minus = 0;
        
        foreach (var p in _allPlayerManagers)
        {
            if (p.isDead)
                count++;
            else if (p.isEnemy && p.isDead != true)
                minus++;
        }
        if (count >= _allPlayerManagers.Count - minus)
            EnemyScreen.SetActive(true);
    }

    IEnumerator loading()
    {
        
        yield return new WaitForSeconds(transitionTime);
        welcomeScreen.GetComponent<Animator>().enabled = true;
    }
    
    IEnumerator  welcomePanelDelay()
    {
        yield return new WaitForSeconds(5);
        
        welcomeScreen.SetActive(false);

    }
        public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

        public IEnumerator EndGameBackToLobby()
        {
            yield return new WaitForSeconds(5);
            PhotonNetwork.LoadLevel("lobby");
        }
        public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    
        void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}",PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("testMap");
    }


     public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

    }

     public void EndGame()
     {
         foreach (var p in _allPlayerManagers)
         {
             p.ResetRpcCall();
         }
         PhotonNetwork.LoadLevel("lobby");
     }

}
