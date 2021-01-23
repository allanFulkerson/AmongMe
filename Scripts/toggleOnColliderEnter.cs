using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class toggleOnColliderEnter : MonoBehaviour
{

    public Collider2D Collider;

    public Button wireButton;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Player"))
        {
                 newPlayerManager collidedPlayerManager = other.gameObject.GetComponent<newPlayerManager>();
                 if (!collidedPlayerManager.isEnemy)
                 {
                     wireButton.interactable = true;
                 }   
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.tag.Contains("Player"))
        { 
            newPlayerManager collidedPlayerManager = other.gameObject.GetComponent<newPlayerManager>();
                 if (!collidedPlayerManager.isEnemy)
                 {
                     wireButton.interactable = false;
                 }
        }
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Contains("Player"))
        {newPlayerManager collidedPlayerManager = other.gameObject.GetComponent<newPlayerManager>();
        if (!collidedPlayerManager.isEnemy)
        {
            wireButton.interactable = true;
        }
     }

    }
    
}
