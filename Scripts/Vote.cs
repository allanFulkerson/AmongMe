using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vote : MonoBehaviour
{
    public newPlayerManager Against { get; set; }
    public newPlayerManager Who { get; set; }
    
    public bool IsSkipped { get; set; }
    

    public int VoteId { get; set; }

    public override string ToString()
    {
        if (IsSkipped)
            return "ID: " + VoteId + "   Name: " + Who.playerNameText.text + "as skipped ";
        
        return "ID: " + VoteId + "   Name: " + Who.playerNameText.text + "as voted against " + Against.playerNameText.text;
    }
   
}
