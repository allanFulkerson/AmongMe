using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NewLauncher : MonoBehaviourPunCallbacks
{
    public GameObject Player;

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]

    [SerializeField]
    private GameObject controlPanel;

    [SerializeField]
    private GameObject createPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    [SerializeField]
    private GameObject rooms;

    [SerializeField]
    private GameObject raw;

    public Transform parent;

    public createPanel cL;

    private static NewLauncher nl;
    public bool test = true;


    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

// Start is called before the first frame update
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
        nl = this;
             
        //PhotonNetwork.ConnectUsingSettings();
        Debug.Log("nickname " + PhotonNetwork.NickName);
       // Connect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Nous avons passé l'étape de connexion ");

    if (isConnecting || PhotonNetwork.IsConnected)
    {
        Debug.Log("Nous sommes connecté, alors nous joignons un lobby" );
        PhotonNetwork.JoinLobby();
        isConnecting = false;
    }
    Debug.Log("PUN OnConnectedToMaster() was called by PUN");
    }

     public override void OnJoinedLobby()
    {
         Debug.Log("JoinedLobby lance le callBack des rooms");
            
       // PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions {MaxPlayers=4}, TypedLobby.Default);
    }

    public override void OnJoinedRoom(){

      //  PhotonNetwork.Instantiate("Hero", new Vector2(0,0), Quaternion.identity);
     if(test)
         PhotonNetwork.LoadLevel("lobby");
     else
        PhotonNetwork.LoadLevel("lobby");

    }

    public void Connect(string name ="")
    { 
        Debug.Log("Somme nous connecté ? " + PhotonNetwork.IsConnected);
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
       
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Nous somme connecté");
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
           if(name!=""){
              PhotonNetwork.JoinRoom(name);
             // PhotonNetwork.LoadLevel("lobby");
            }else
                Debug.Log("ERROR we need a correct name to log in");
            
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

        public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}",cause);
    }

   public override void OnRoomListUpdate(List<RoomInfo> roomList){
       Debug.Log(roomList.Count + " Rooms");

        base.OnRoomListUpdate(roomList);
        if(roomList.Count>=1){
             rooms.SetActive(true);
        int iter = 0;
        foreach (var Item in roomList)
         {
                
             Debug.Log(Item.MaxPlayers);
             GameObject newRaw = Instantiate(raw, new Vector3(0,0,0),Quaternion.identity);
             rowLogic rl = newRaw.GetComponent<rowLogic>();
             rl.title.text = Item.Name;
             rl.max.text = "/ " + Item.MaxPlayers.ToString();
             rl.nb.text= Item.PlayerCount.ToString();
             rl.button.onClick.AddListener(() => {Connect(Item.Name);});
            newRaw.transform.parent = parent;
            Vector3 pos= new Vector3(91,173,0);
            newRaw.transform.localScale = new Vector3(1f,1f,1f);
            int y = 30*iter;
            pos.y -= y;
            newRaw.transform.localPosition = pos;
            iter++;
         }
        }else{
            createPanel.SetActive(true);
            controlPanel.SetActive(false);            
        }


    }

    public void generateRaw(){
        GameObject newRaw = Instantiate(raw, new Vector3(0,0,0),Quaternion.identity);
        newRaw.transform.parent = parent;
        //newRaw.transform.position.z = 0;
        newRaw.transform.localScale = new Vector3(1f,1f,1f);
        int y = 30;
        Vector3 pos= new Vector3(91,173,0);
            pos.y -= y;
        newRaw.transform.localPosition = pos;
        Debug.Log("la position " + newRaw.transform.position + " et le scale "+ newRaw.transform.localScale);
    }

    public void createRoom(){
        createPanel.SetActive(false);
        progressLabel.SetActive(true);
        PhotonNetwork.JoinOrCreateRoom(cL.roomName, new RoomOptions {MaxPlayers = Convert.ToByte(cL.nbPlayers)}, TypedLobby.Default);
    }

    public void joinTest()
    {
        test = true;
        PhotonNetwork.JoinOrCreateRoom("test", new RoomOptions {MaxPlayers = 3}, TypedLobby.Default);
    }

}
