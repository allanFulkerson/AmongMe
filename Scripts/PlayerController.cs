using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    public bool testing;
    public Transform avatarSprite;
    public Animator anim;
    public bool isDead;
    public float Direction = 1;
    [SerializeField]
    private bool hasControl = false;
    [SerializeField]
    
    private float speed;
    public float x;
    public float y;

    private Vector3 velocity;
    private Vector3 prevPos;

    private Rigidbody2D myRB;

    public Vector2 movement;
    private PhotonView photonView;

    
     void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        Direction = 1;
        avatarSprite.localScale = new Vector2(1, 1);
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
            hasControl = true;
        else
            hasControl = false;
    }

    void Update()
    {
        avatarSprite.localScale = new Vector2(Direction, 1);

        if (isDead)
        {
            anim.SetBool("isDead", true);

            
        }else
            anim.SetBool("isDead", false);
  
        if (x != 0)
        {
            Direction = Mathf.Sign(x);
        }      
        if (!photonView.IsMine)
            return;

        
        x = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        y = Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime;
        
       
    }
    private void FixedUpdate() {
        
            movement.x = x;
            movement.y = y;
            if (hasControl)
            {
            myRB.MovePosition(myRB.position + movement * speed * Time.fixedDeltaTime);
            }
            
        anim.SetFloat("moveX",movement.x);
       anim.SetFloat("moveY",movement.y);
       anim.SetFloat("speed",movement.magnitude);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Color tempColor = avatarSprite.GetComponent<SpriteRenderer>().color;
            Vector3 colorToVector = new Vector3(tempColor.r,tempColor.g,tempColor.b);
            float testX = anim.GetFloat("moveX");
            float testy = anim.GetFloat("moveY");
            bool testBool = anim.GetBool("isDead");
            stream.SendNext(Direction);
            stream.SendNext(colorToVector);
            stream.Serialize(ref testX);
            stream.Serialize(ref testy);
            stream.Serialize(ref testBool);
        }
        else
        {
            this.Direction = (float)stream.ReceiveNext();
            var tempsVector3 = (Vector3)stream.ReceiveNext();
            Color vectorToColor = new Color(tempsVector3.x,tempsVector3.y,tempsVector3.z);
            this.avatarSprite.GetComponent<SpriteRenderer>().color = vectorToColor;
            float testX = 0f;
            float testy = 0f;
            bool newBool = false;
            stream.Serialize(ref testX);
            stream.Serialize(ref testy);
            stream.Serialize(ref newBool);
            this.x = testX;
            this.y = testy;
            this.isDead = newBool;

        }
    }
}

