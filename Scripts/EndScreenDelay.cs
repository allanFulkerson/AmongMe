using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenDelay : MonoBehaviour
{
    public int TimeToEnd = 5;
    public bool end = false;
    public newGameManager GameManager;

    void Start()
    {
        StartCoroutine(Delay(TimeToEnd));
    }
    
    IEnumerator  Delay(int time)
    {
        yield return new WaitForSeconds(time);
        if(end)
            GameManager.EndGame();
        gameObject.SetActive(false);

    }
}
