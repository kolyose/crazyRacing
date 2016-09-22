using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class MainModel : MonoBehaviour
{
    public float MovingSpeed = 1.0f;
    private bool _isNewGame;
    public bool IsNewGame {
        get
        {
            bool result = _isNewGame;
            _isNewGame = false;
            return result;
        }
        set
        {
            _isNewGame = value;
        }
    }
    public SettingsVO GameSettings { get; set;}
    public PlayerVO User {get; set; }
    public PlayerVO[] RoundPlayers {get; private set;}
    public Dictionary<string, RoundResultVO> RoundResultsByPlayerId { get; set; }

    public MainModel()
    {
        RoundPlayers = new PlayerVO[] { };
    }

    public void AddPlayers(PlayerVO[] players)
    {
        RoundPlayers = RoundPlayers.Concat(players).ToArray();
    }

    public void RemovePlayers(PlayerVO[] players)
    {
       //TODO: add functionality
    }

    public void ResetGameData()
    {
        IsNewGame = true;
        RoundResultsByPlayerId = new Dictionary<string, RoundResultVO>();
    }

    public void SaveRoundResults(RoundResultVO[] results)
    {
        foreach (RoundResultVO result in results)
        {
            RoundResultsByPlayerId[result.playerId] = result;
        }
    }

    public bool IsGameEnd()
    {
        return GetUserResults().place > 0;
    }

    public RoundResultVO GetUserResults()
    {
        return RoundResultsByPlayerId[User.id];
    }

    public bool IsBoostAllowed()
    {
        return !GetUserResults().boosted;
    }

    public SortedDictionary<uint, string> GetPlayersByPlace()
    {
        SortedDictionary<uint, string> playersByPlace = new SortedDictionary<uint, string>();
        foreach (KeyValuePair<string, RoundResultVO> element in RoundResultsByPlayerId)
        {
            playersByPlace.Add(element.Value.place, GetPlayerById(element.Key).name);
        }

        return playersByPlace;
    }

    public PlayerVO GetPlayerById(string playerId)
    {
        foreach (PlayerVO player in RoundPlayers)
        {
            if (player.id == playerId)
            {
                return player;
            }
        }

        return null;
    }
}
