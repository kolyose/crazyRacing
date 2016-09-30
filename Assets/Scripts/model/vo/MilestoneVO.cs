using System;
using UnityEngine;

[Serializable]
public enum MilestoneType
{
    MOVE=1,
    BOOST=2,
    BLOCK=3
}

[Serializable]
 public class MilestoneVO
 {
     public MilestoneType type;
     public float x;
     public float y;
     public float speed=1;
    
     public Vector3 position 
     {
         get 
         {
             return new Vector3(x, y, 0); 
         }
     }
 }
