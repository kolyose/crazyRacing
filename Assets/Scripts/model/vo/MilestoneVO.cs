using System;
using UnityEngine;

[Serializable]
 public class MilestoneVO
 {
     public float x;
     public float y;
     public float s;
    
     public Vector3 position 
     {
         get 
         {
             return new Vector3(x, y, 0); 
         }
     }

     public float speed
     {
         get 
         {           
             return s; 
         }
     }

     public MilestoneVO(Vector3 position, float speed) 
     {
         x = position.x;
         y = position.y;
         s = speed;
     }
 }
