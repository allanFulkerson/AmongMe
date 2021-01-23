using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Random = System.Random;

public class newPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{

    public MonoBehaviour[] scriptsToIgnore;
    public GameObject camera;
    public Camera realCamera;
    public static Color spriteColor;
    public Collider2D mycollider;
    public SpriteRenderer myAvatarSprite;
    public Collider2D killColl;
    public PlayerController myController;
    public bool inChat = false;
    public GameObject bodyPrefab;
    public LightFOV lumbraFov;
    public string uniqID;
    public Text DebugText;

    public newGameManager GameManager;

    public PhotonView photonView;

    public TMP_Text playerNameText;
    public int VotesNr = 0;

    public bool inLobby;

    public static newPlayerManager thisPlayerManager;
    public static newPlayerManager localNpm;
    
    public static GameObject LocalPlayerInstance;
    public Canvas canvasButton;
    public GameObject playerButtonCanvas;
    public bool isEnemy = false;
    public bool isDead = false;
    public InputAction Kill;
    public Button reportButton;
    public Button killButton;
    private newPlayerManager PlayerTarget;
    private List<newPlayerManager> targets;
    public Transform test;
    public VoteManager voteManager;
    public cardLogic CardLogic;

    public List<Transform> allBodies;
    private List<Transform> bodiesFound;
    public LayerMask IgnoredMask;
    public GameObject CanvasTextObject;
    
    private Player[] allPlayers;
    public Text testText;
    
    public int NumberInRoom;
    public bool hasVoted = false;
    public bool report;
    private bool deathStuff = false;
    

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
       
        //set the instances
        thisPlayerManager = this;
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            uniqID = photonView.Owner.NickName + RandomString(5);
            photonView.RPC("unidIDRpcCall", RpcTarget.OthersBuffered, uniqID);
            LocalPlayerInstance = this.gameObject;
            localNpm = this;
        }
 
        DontDestroyOnLoad(this.gameObject);
    }
    
    public static string RandomString(int length)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    [PunRPC]
    public void unidIDRpcCall(string uId)
    {
        uniqID = uId;
    }
    

    void Start()
    {

        //find & set player number in room
        SetPlayerNumber();
        
        //setting targets lists
        targets = new List<newPlayerManager>();
        
        //initialize the lists for the bodies
        allBodies = new List<Transform>();
        bodiesFound = new List<Transform>();
        
        //Default color setting
        if (spriteColor == Color.clear)
        {
            spriteColor = Color.white;
        }
        myAvatarSprite.color = Color.white;
        
        
        //Setting player name text
        if (playerNameText != null)
            playerNameText.text = photonView.Owner.NickName;
   

        // log the master role for this player
        //Debug.Log(PhotonNetwork.IsMasterClient ? "joueur master" : " not master");
        
         //setting camera
        changeCameraValueByScene();
        
        //Deactivate component if we're not the client of this instance
        if(!photonView.IsMine)
        {
            lumbraFov.enabled = false;
            //loop through the list of scripts to ignore
            foreach (var item in scriptsToIgnore)
            {
                if (item.name == "Hero")
                    test = item.transform;
                item.enabled = false;
            }
            playerButtonCanvas.SetActive(false);
            
        }

    }

    public void activeButton()
    {
        reportButton.interactable = true;
        killButton.interactable = true;
    }
    public void desactiveButton()
    {
        reportButton.interactable = false;
        killButton.interactable = false;
    }

    /*
     * Search and set the player number
     */
    private void SetPlayerNumber()
    {
        if (PhotonNetwork.InRoom)
        {
            allPlayers = PhotonNetwork.PlayerList;

            foreach (Player p in allPlayers)
            {
                if (p != PhotonNetwork.LocalPlayer)
                {
                    NumberInRoom++;
                }
            }
        }
        else
        {
            Debug.Log("Attention ! Can't set the player number");
            return;
        }
    }

    public void ResetRpcCall()
    {
        photonView.RPC("Reset", RpcTarget.All);
    }

    /*
     * Reset the settings of the player for a new game, death, control etc
     */
    [PunRPC]
    public void Reset()
    {
        isDead = false;
        deathStuff = false;
        myController.isDead = false;
        mycollider.enabled = true;
        myAvatarSprite.gameObject.layer = 0;
        isEnemy = false;
        playerNameText.gameObject.layer = 5;
        bodiesFound.Clear();
        bodiesFound.Capacity = 0;
        allBodies.Clear();
        allBodies.Capacity = 0;
        activeButton();
        //gameObject.layer = 12;
        lumbraFov.obstacleMask = LayerMask.GetMask("Wall");
        
    }
   
    
/**
 * Use for player action and update basic stat
 */
    void Update()
    {

        if (photonView.IsMine)
        {
            if (Input.GetKeyDown("space") )
            {
                //Debug.Log("space key was pressed and is an impostor ? " + isEnemy);
                if (isEnemy)
                {
                    killPLayer();  
                }
              
            }
            
            // active the searching for bodies counting on the count
            if (allBodies.Count > 0)
            {
                searchingBodies();
            }
            else if(isEnemy || isDead)
            {
                reportButton.interactable = false;
            }

            if (targets.Count == 0 || inChat || isEnemy != true)
                killButton.interactable = false;
        }

        // Set the anim bool for controller to false
        if(photonView.IsMine!=true)
        {
            if (isDead && deathStuff != true)
                    {
                        myController.isDead = isDead;
                        Debug.Log("Doing death stuff");
                        myAvatarSprite.gameObject.layer = 12;
                        playerNameText.gameObject.layer = 12;
                        CanvasTextObject.layer = 12;
                        mycollider.enabled = false;
                        deathStuff = true;
                    }
        }
        

    }
    
    
/***
 * Detecting collision with other player and process targeting 
 */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag=="Player")
        {
            //setting target
            newPlayerManager target = other.GetComponent<newPlayerManager>();

            //checking players roles, process targeting for the kill action
            if (isEnemy && target.isEnemy != true && isDead != true)
            {
                PlayerTarget = target;
                targets.Add(target);
                killButton.interactable = true;
            }
        }
    }

/**
 * On exit collision with an player, remove it from targets list
 */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            newPlayerManager tempTarget = other.GetComponent<newPlayerManager>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }


/**
 * Check with raycast if we're around a body to report
 */
    void searchingBodies()
    { 
        foreach (Transform body in allBodies)
        {
                    RaycastHit hit;
                    Ray ray = new Ray(transform.position, body.position - transform.position);
                    if (Physics.Raycast(ray, out hit, 1000f, ~IgnoredMask))
                    {
                        if (hit.transform == body)
                        {
                            if(bodiesFound.Contains(body.transform))
                                return;
                            bodiesFound.Add(body.transform);
                            reportButton.interactable = true;
                        }
                        else
                        {
                            bodiesFound.Remove(body.transform);
                        }
                    }
        }
    }

/**
 * Proced to the reporting action ( remove from list and looking for the rpc call
 */
    public void ReportBody()
    {
        if(bodiesFound == null || bodiesFound.Count == 0)
            return;
        
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        allBodies.Remove(tempBody);
        bodiesFound.Remove(tempBody);
        report = true;
        tempBody.GetComponent<body>().Report(GameManager);

    }

/**
 * Killing process, looking first in the targets array and kill the lastone
 * TODO Redefine the target from the closest one
 */
public void killPLayer()
    {
        if(targets.Count == 0 || inChat || isEnemy != true)
                return;
            else
            {
                if (targets[targets.Count-1].isDead || targets[targets.Count-1].isEnemy)
                    return;
                else
                {
                    myController.transform.position = targets[targets.Count-1].myController.transform.position;
                    targets[targets.Count-1].DieRPCCall(true);
                    /*targets[targets.Count-1].myController.isDead = true;
                    targets[targets.Count-1].mycollider.enabled = false;*/
                    
                    targets.RemoveAt(targets.Count-1);
                }
            }
    }

/**
 * set the color for the player sprite
 */
    public void setColor(Color newColor)
    {
        if (photonView.IsMine)
        {
            spriteColor = newColor;
            if (myAvatarSprite != null)
            {
                myAvatarSprite.color = spriteColor;
            }
        }
       
    }
/**
public void testDetectBody()
{
    Debug.Log("WE DETECTING BODYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY");
}*/
/**
 * RPC CALL FOR THE DEATH 
 */

    public void DieRPCCall(bool body)
    {
       photonView.RPC("Die", RpcTarget.All, body);
    }

/**
 * Killing process; 
 */
    [PunRPC]
    public void Die(bool body)
    {
        if (!photonView.IsMine)
            return; 
        
        desactiveButton();

        //active animation and disable the collider for the player
        isDead = true;
        myController.isDead = true;
        mycollider.enabled = false;

        myAvatarSprite.gameObject.layer = 12;
        playerNameText.gameObject.layer = 12;

        lumbraFov.obstacleMask = LayerMask.GetMask("Nothing");

        realCamera.cullingMask =  realCamera.cullingMask ^ (1 << 12);

        if (body)
        {
                    body tempBody = PhotonNetwork.Instantiate("body", transform.position, transform.rotation).GetComponent<body>();
                    tempBody.gm = GameManager;
                    tempBody.reportButton = reportButton;
                    //set the color of the body according to the player current color;
                    tempBody.setColor(myAvatarSprite.color); 
        }

        
        
    }

[PunRPC]
public void deathCheckRPC()
{
    Debug.Log("call du deathcheck");
    GameManager.CheckNumberOfDeads();
}


/**
 * Set the our camera to the canvas of clicks action
 */
public void setCanvasButton(Canvas canvasB)
    {

        canvasButton = canvasB;
      
        if (photonView.IsMine)
        {
            Debug.Log(" attribution de la camera pour les boutons pour  " + playerNameText.text);
            canvasB.worldCamera = realCamera;
        }
       
    }
    

/**
 * Check the scene to set on or off the camera of the player;
 */
    public void changeCameraValueByScene()
    {
        
        Scene currentScene = SceneManager.GetActiveScene ();
        // Retrieve the name of this scene.
        string sceneName = currentScene.name;
        
        //
        if (photonView.IsMine && currentScene.name.Contains("map")|| photonView.IsMine && currentScene.name.Contains("Map"))
        {
            Debug.Log("Camera gestion true ");
            camera.SetActive(true);
            
            
        }
        else
        {
            Debug.Log("Camera gestion false"); camera.SetActive(false);
        }
            

    }

/**
 * Photon instantiate event, for now we just log the event
 */
    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate event in playerManager");
    }
    
/**
 * Listening the changing scene stat; 
 */
    private void ChangedActiveScene(Scene current, Scene next)
    {
        
        string currentName = current.name;

        if (currentName == null)
        {
            // Scene1 has been removed
            currentName = "Replaced";
        }

        if (next.name.Contains("map")||next.name.Contains("Map"))
        {
            camera.SetActive(true);
        }

        //Debug.Log("Scenes: " + currentName + ", " + next.name);
    }

    public void rpcColorCall()
    {
        
        photonView.RPC("rpcColor",RpcTarget.OthersBuffered, new Vector3(spriteColor.r,spriteColor.g,spriteColor.b));
    }
    
    [PunRPC]
    void rpcColor( Vector3 vcolor)
    {
        Color color = new Color(vcolor.x,vcolor.y,vcolor.z);
        myAvatarSprite.color = color;
    }  

    public void rpcEnemyCall(bool role)
    {
        Debug.Log("Attribution du role imposteur pour " + playerNameText.text);
        
        photonView.RPC("rpcEnemy", RpcTarget.AllBuffered, role);
    }
    
    [PunRPC]
    void rpcEnemy( bool venemy)
    {
        Debug.Log("Attribution du role imposteur pour " + playerNameText.text);
        isEnemy = venemy;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            bool tempdeath = this.isDead;
            bool repottp = this.report;

            stream.Serialize(ref tempdeath);
            stream.Serialize(ref repottp);
        }
        else
        {
            bool tempdeath = false;
            bool repottp = false;
            stream.Serialize(ref tempdeath);
            stream.Serialize(ref repottp);
            this.isDead = tempdeath;
            this.report = repottp;
            if(tempdeath)
                photonView.RPC("deathCheckRPC", RpcTarget.All);
        }
    }
}


