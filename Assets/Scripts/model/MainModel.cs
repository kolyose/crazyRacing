using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class MainModel : MonoBehaviour
{
    public float MovingSpeed = 1.0f;
    public SettingsVO GameSettings { get; set;}
    public PlayerVO User {get; set; }
    public PlayerVO[] RoundPlayers {get; private set;}

//    public Dictionary<string, List<MilestoneVO>> MilestonesByPlayerId { get; set; }
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
        RoundResultsByPlayerId = new Dictionary<string, RoundResultVO>();
    }

    public void SaveRoundResults(RoundResultVO[] results)
    {
        foreach (RoundResultVO result in results)
        {
            /*for (int i=0; i<RoundPlayers.Length; i++)
            {
                 if (result.playerId == RoundPlayers[i].id)
                 {
                    MilestonesByPlayerId[result.playerId] = MilestonesByPlayerId[result.playerId].Concat(result.milestones.ToList()).ToList();                  
                    break;
                 }
            }*/

            RoundResultsByPlayerId[result.playerId] = result;
        }
    }

    public bool IsGameEnd()
    {
        return RoundResultsByPlayerId[User.id].place > 0;
    }

    public CharacterVO GetActiveCharacterVO()
    {
        //TODO: remove hardcode
        CharacterVO characterVO = new CharacterVO();
        characterVO.pictureId = 1;
        return characterVO;
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
