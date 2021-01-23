using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{

    public Transform spawn;
    public Text nbPlayersText;
    
    public GameObject playerPrefab;

    public Button startButton;
    
    public static Lobby Instance;
    
    public Transform[] spawnsPoints;
    
    public GameObject[] players;
    

    void Start()
    {
        Instance= this;
    
        var currentRoom = PhotonNetwork.CurrentRoom;
        var countP = 0;
       // Debug.Log("Current room name : " + currentRoom ); 
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            countP = PhotonNetwork.CurrentRoom.PlayerCount;
           // Debug.Log("count of players " + countP );
        }
                 
      // Debug.Log(" room actu " + currentRoom); 
      
      // Debug.Log("Nom du level " +  Application.loadedLevelName);

       

       if (playerPrefab == null)
       {
           Debug.LogError("<Color=Green><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
           
       }else
       {
          
           if (newPlayerManager.LocalPlayerInstance == null)
           {
               Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
            
              PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f),Quaternion.identity,0); 
           }
           else
           {
               Debug.LogFormat("Ignoring scene load for {0}",SceneManagerHelper.ActiveSceneName);
           }
       }

       players = GameObject.FindGameObjectsWithTag("Player");
       

       foreach (GameObject player in players)
       {

           newPlayerManager npm = player.GetComponent<newPlayerManager>();
           npm.changeCameraValueByScene();
           npm.transform.localPosition = spawnsPoints[npm.NumberInRoom].position;
       }

       
    }


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("Entrée du joueur " + other.NickName);
        nbPlayersText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            startButton.interactable = PhotonNetwork.IsMasterClient;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
                    nbPlayersText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
                    if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                        startButton.interactable = true;
        }else if (PhotonNetwork.IsConnected)
            nbPlayersText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;


    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    
    public void loadMap(){
        var test = Random.Range(0, players.Length-1);
        players[test].GetComponent<newPlayerManager>().rpcEnemyCall(true);
        PhotonNetwork.LoadLevel("testMap");
    }

}
