using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class VoteManager : MonoBehaviour, IPunObservable
{
    public List<Vote> votes = new List<Vote>();
    public List<newPlayerManager> _allPlayerManagers;
    public List<cardLogic> allCardsList;
    public GameObject[] playersGameObjects;
    public int VotesNr = 0;
    public GameObject VotePanel;
    public GameObject SkipePanel;
    public GameObject TiePanel;
    public GameObject PassPanel;
    public GameObject EjectPanel;
    public Transform PlayesGrid;
    public GameObject PlayerCardPrefab;
    public ejectLogic EjectLogic;
    public bool TimerEnd = false;
    public int CanVoteNbr;
    private PhotonView pv;
    public GameObject BlockPanel;
    public Text CountText;
    public int cardsForAll = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        pv = GetComponent<PhotonView>();
        
        _allPlayerManagers = new List<newPlayerManager>();
        playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        
        
            
        
        foreach (var p in playersGameObjects)
        {
            _allPlayerManagers.Add(p.GetComponent<newPlayerManager>());
            var card = Instantiate(PlayerCardPrefab, Vector3.zero, Quaternion.identity,PlayesGrid);
           // var card = PhotonNetwork.Instantiate("PlayerVoteCard", new Vector3(0, 0, 0), Quaternion.identity);
            
            cardLogic cl = card.GetComponent<cardLogic>();
            newPlayerManager pm = p.GetComponent<newPlayerManager>();
            pm.voteManager = this;
            cl.characterImg.color = pm.myAvatarSprite.color;
            cl.ChaNameTxt.text = pm.playerNameText.text;
            cl.thisPlayer = pm;
            cl.VM = this;
            cl.BlockPanel = BlockPanel;
            cl.startBlockPanel();
            pm.CardLogic = cl;
            
            if (pm.report)
                cl.AlertImg.enabled = true;
            
            allCardsList.Add(cl);
            
            // setting the right of vote
            if (pm.isDead)
                setDeadCard(card,cl);
            else 
                CanVoteNbr++;
            


        } 
        pv.RPC("cardsCountUpdate",RpcTarget.All);

    }

    private void setDeadCard(GameObject card, cardLogic cl)
    {
        Debug.Log("set dead card working here");
            card.GetComponent<Button>().interactable = false;
        var colorBlock = card.GetComponent<Button>().colors;
        colorBlock.disabledColor = Color.gray;
        cl.DeadImg.enabled = true;
        cl.startBlockPanel();
    }

    [PunRPC]
    public void cardsCountUpdate()
    {
        cardsForAll++;
    }
    private void OnEnable()
    {
        CanVoteNbr = 0;
        foreach (cardLogic card in allCardsList)
        {
            card.startBlockPanel();
            if (card.thisPlayer.report)
                card.AlertImg.enabled = true;
            else if (card.thisPlayer.isDead)
                setDeadCard(card.transform.gameObject, card);

            //card.alertCall();
        }

        
        foreach (var p in _allPlayerManagers)
        {

            if (p.isDead)
            {
                if (p.photonView.IsMine)
                    BlockPanel.SetActive(true);
            }
            else
                CanVoteNbr++;
        }
        
    }

    public void AgainstVote(newPlayerManager who, newPlayerManager against)
    {

        if (who.isDead || against == who)
        {
            return;
        }
            
        votes.Add(new Vote(){ Who=who, VoteId=who.NumberInRoom,Against = against});
        
        foreach (var p in _allPlayerManagers)
        {
            if (against == p)
            {
               p.VotesNr++;
            }
                

            if (p == who)
            {
               p.hasVoted = true; 
              
            }
                
        }

        voteImageCall(who);
        playerVotedImageCall(against);
        VotesNr++;
        pv.RPC("voteRPC",RpcTarget.Others, against.uniqID);
    }

    public void voteImageCall(newPlayerManager who)
    {
       who.CardLogic.CM.votedCall();
    }

    public void playerVotedImageCall(newPlayerManager against)
    {
        against.CardLogic.CM.addVotedImg();
    }

     [PunRPC]
     public void voteRPC(string uniqID)
     {
         foreach (var p in _allPlayerManagers)
         {
            
             if (uniqID == p.uniqID)
                 p.VotesNr++;
         }
         
         VotesNr++;
        
     }

    public void SkippedVote(newPlayerManager who)
    {
        votes.Add(new Vote(){Who=who,VoteId=who.NumberInRoom, IsSkipped=true});
        VotesNr++;
    }

    public void updateCountText()
    {
        CountText.text = VotesNr + "/" + CanVoteNbr;
    }
    public void EndVote()
    {
        //return;
        int CurrentVotesNr = 0;
        int SkippedNr = 0;
        int HalfP = _allPlayerManagers.Count / 2;
        newPlayerManager higherScore = null;
        bool TaTie = false;
        
        foreach (var p in _allPlayerManagers)
        {
            if (higherScore != null)
            {
                if (higherScore.VotesNr < p.VotesNr)
                {
                    higherScore = p;
                }else if (higherScore.VotesNr == p.VotesNr)
                {
                    TaTie = true;
                }
            }else
                higherScore = p;
        }
        
        var result = votes.Where( y => y.IsSkipped==true );
        SkippedNr = result.Count();
        
        if (SkippedNr>=VotesNr)
        {
            Skipped();
        }else if (HalfP >= VotesNr)
        {
            Pass();
        }else if (TaTie)
        {
            Tie();
        }
        else
        {
          EjectP(higherScore);  
        }

        resetCards();
        resetVoteStatuPlayer();
        resetPlayersVoteNbr();
        VotesNr = 0;
        VotePanel.SetActive(false);
    }

    public void resetPlayersVoteNbr()
    {
        foreach (var p in _allPlayerManagers)
        {
            p.VotesNr = 0;
            foreach (Transform child in p.CardLogic.VotedPlayerContainers.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            
        }
    }

    public void resetVoteStatuPlayer()
    {
        foreach (var player in _allPlayerManagers)
        {
            player.hasVoted = false;
        }
    }
    public void resetCards()
    {
        foreach (cardLogic card in allCardsList)
        { 

            card.resetImages();
            card.ResetPlayerCountList();
            card.thisPlayer.report = false;
            if(!card.thisPlayer.isDead)
                card.GetComponent<Button>().interactable = true;
            
           
        }
    }
    public  void Tie()
    {
        TiePanel.SetActive(true);
    }
    public  void Pass()
    {
        PassPanel.SetActive(true);
    }
    public void Skipped()
    {
        SkipePanel.SetActive(true);
    }

    public  void EjectP(newPlayerManager playerToEject)
    {
        playerToEject.DieRPCCall(false);
        EjectLogic.EjectedPlayer = playerToEject;
        EjectPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        updateCountText();
        if (TimerEnd || VotesNr == CanVoteNbr)
        {
            EndVote();
        }
            
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.IsWriting)
        {
            Debug.Log("voteManager écrit" );
            int tempVote = this.VotesNr;
            Debug.Log("voteManager écrit ceci " + tempVote  );
            // We own this player: send the others our data
            stream.Serialize(ref tempVote);
        }
        else
        {
            Debug.Log("voteManager recoit" );
            // Network player, receive data
            int tempVote = 0;
            stream.Serialize(ref tempVote);
            this.VotesNr = tempVote;
            Debug.Log("voteManager recoit ceci " + tempVote);
            
        }*/
    }
}
