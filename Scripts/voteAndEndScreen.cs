using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voteAndEndScreen : MonoBehaviour
{
    public newGameManager ngm;

    private void OnEnable()
    {
        StartCoroutine(desactivationDelay());
    }

    IEnumerator  desactivationDelay()
    {
        yield return new WaitForSeconds(5);
        
        gameObject.SetActive(false);
        ngm.EndGame();

    }
}
