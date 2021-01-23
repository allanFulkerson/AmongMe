using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class welcomeScreen : MonoBehaviour
{

    public newPlayerManager player;

    public GameObject CrewScreen;
    public GameObject ImpostorScreen;


    public void switchScreen()
    {
                if (player.isEnemy)
                {
                    CrewScreen.SetActive(false);
                    ImpostorScreen.SetActive(true);
                }
                else
                {
                    CrewScreen.SetActive(true);
                    ImpostorScreen.SetActive(false);
                }
    }
    
}
