using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class RoundResultVO
{
    public string playerId;  
    public uint random;
    public uint place;
    public MilestoneVO[] milestones;    
}
