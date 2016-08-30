using System;
using UnityEngine;

public class JSONParser : MonoBehaviour, IServerDataParser
{
    public PlayerVO[] GetPlayersData(string rawData)
    {
        PlayerVO[] players = JsonUtility.FromJson<PlayerVOArray>(rawData).players;
        return players;
    }

    public RoundResultVO[] GetRoundResultsData(string rawData)
    {
        RoundResultVO[] results = JsonUtility.FromJson<RoundResultVOArray>(rawData).results;
        return results;
    }
}