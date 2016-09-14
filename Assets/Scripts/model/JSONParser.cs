using System;
using UnityEngine;

public class JSONParser : MonoBehaviour, IServerDataParser
{
    public SettingsVO GetSettingsData(string rawData)
    {
        SettingsVO settings = JsonUtility.FromJson<SettingsVO>(rawData);
        return settings;
    }

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