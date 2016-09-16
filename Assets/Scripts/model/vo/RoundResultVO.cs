using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class RoundResultVO
{
    public string playerId;  
    public uint distance;
    public uint place;
    public bool boosted;
    public MilestoneVO[] milestones;    
}
