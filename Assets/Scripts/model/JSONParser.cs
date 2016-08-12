using System;
using UnityEngine;

public class JSONParser : MonoBehaviour, IServerDataParser
{
    public PlayerVO[] GetPlayersData(string rawData)
    {
       return JsonUtility.FromJson<PlayerVOArray>(rawData).players;
    }

    public RoundResultVO[] GetRoundResultsData(string rawData)
    {
        return JsonUtility.FromJson<RoundResultVOArray>(rawData).results;
    }
}