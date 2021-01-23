using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ejectLogic : MonoBehaviour
{
    public Image AvatarImage;
    public Animator anim;
    public Rigidbody2D myRB;
    public float ProjectileForce;
    public newPlayerManager EjectedPlayer;
    public Text ejectTxt;
    public Vector2 movement;

    void Start()
    {

        AvatarImage.color = EjectedPlayer.myAvatarSprite.color;
        ejectTxt.text = EjectedPlayer.playerNameText.text + " was ejected";
        myRB.AddForce(Vector2.right*ProjectileForce);

    }

}
