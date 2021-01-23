using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class createPanel : MonoBehaviour
{

    public InputField input;
    public InputField nbInput;
    public Button button;
    public string roomName;
    public int nbPlayers;
    
    //TODO more control of the inputs
    public void activeButton(){
        button.interactable = true;
        roomName = input.text.ToString();
        nbPlayers = int.Parse(nbInput.text.ToString());

    }
}
