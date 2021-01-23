using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WireTask : MonoBehaviour {
   
   public Button button;
   public bool lunched = false;
   public GameObject camera;
   public List<Color> _wireColors = new List<Color>();
   public List<Wire> _leftWires = new List<Wire>(); 
   public List<Wire> _rightWires = new List<Wire>();
   public TasksManager taksM;
   
   public Wire CurrentDraggedWire;
   public Wire CurrentHoveredWire;
 
   public bool IsTaskCompleted = false;
   
   private List<Color> _availableColors;
   private List<int> _availableLeftWireIndex;
   private List<int> _availableRightWireIndex;
   
   void OnEnable()
   {
      if (lunched)
      {
         StartCoroutine(CheckTaskCompletion());
      }
   }

   
   
   private void Start() {
      if (!lunched)
      {
           _availableColors = new List<Color>(_wireColors);
               _availableLeftWireIndex = new List<int>();
               _availableRightWireIndex = new List<int>();
               camera.SetActive(true);
               
               for (int i = 0; i < _leftWires.Count; i++) { 
                  _availableLeftWireIndex.Add(i); 
               }
           
               for (int i = 0; i < _rightWires.Count; i++) {
                  _availableRightWireIndex.Add(i);
               }
          
               while (_availableColors.Count > 0 && 
                      _availableLeftWireIndex.Count > 0 && 
                      _availableRightWireIndex.Count > 0) {
                  Color pickedColor = 
                   _availableColors[Random.Range(0, _availableColors.Count)];
           
                  int pickedLeftWireIndex = Random.Range(0,
                                            _availableLeftWireIndex.Count);
                  int pickedRightWireIndex = Random.Range(0,
                                            _availableRightWireIndex.Count);
                  _leftWires[_availableLeftWireIndex[pickedLeftWireIndex]]
                                                    .SetColor(pickedColor);
                  _rightWires[_availableRightWireIndex[pickedRightWireIndex]]
                                                    .SetColor(pickedColor);
                
                  _availableColors.Remove(pickedColor);
                  _availableLeftWireIndex.RemoveAt(pickedLeftWireIndex);
                  _availableRightWireIndex.RemoveAt(pickedRightWireIndex);
              }

               lunched = true;
               StartCoroutine(CheckTaskCompletion());
               
      }

   }
 
   private IEnumerator CheckTaskCompletion() {
      while (!IsTaskCompleted) {
         int successfulWires = 0;
         
         for (int i = 0; i < _rightWires.Count; i++) {
            if (_rightWires[i].IsSuccess) { successfulWires++; }
         }
         if (successfulWires >= _rightWires.Count) {
            Debug.Log("TASK COMPLETED");
            
            button.image.color= Color.green;
            button.interactable = false;
            this.gameObject.SetActive(false);
            taksM.completedTaks++;
            taksM.rpcCall();
         }else
            Debug.Log("task pending !");
         
       
         yield return new WaitForSeconds(0.5f);
     }
   }
   

}