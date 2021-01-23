using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class KeypadTask : MonoBehaviour
{

    public Text carcCode;
    public Text inpuCode;
    
    public Button button;
    public bool lunched = false;
    public GameObject camera;
    public TasksManager taksM;
    public int codeLength = 5;
    public float codeResetTimeS = 0.5f;
    public bool isResetting = false;
    

    private void OnEnable()
    {
        string code = string.Empty;
        for (int i = 0; i < codeLength; i++)
        {
            code += Random.Range(1, 10);
        }

        carcCode.text = code;
        inpuCode.text = string.Empty;
        camera.SetActive(true);
    }
    
    public void ButtonClick(int number){
        if (isResetting)
        {
           return;
        }
        inpuCode.text += number;
        
        if (inpuCode.text == carcCode.text)
        {
            inpuCode.text = "Correct";
            button.image.color= Color.green;
            button.interactable = false;
            taksM.completedTaks++;
            taksM.rpcCall();
            this.gameObject.SetActive(false);
            
        }else if (inpuCode.text.Length >= codeLength)
        {
            inpuCode.text = "failed";
            StartCoroutine(ResetCode());
        }
    }

    private IEnumerator ResetCode()
    {
        isResetting = true;
        yield return new WaitForSeconds(codeResetTimeS);
        inpuCode.text = String.Empty;
        isResetting = false;
    }
    
    
}
