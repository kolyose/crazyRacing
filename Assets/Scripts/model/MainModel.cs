using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class MainModel : MonoBehaviour
{
    //TODO: make following parameters to be set up in Settings class
    public uint FieldLength = 10;
    public uint FieldWidth = 6;
    public float MovingSpeed = 1.0f;
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

    public void InitRoundData(RoundResultVO[] results)
    {
       // MilestonesByPlayerId = new Dictionary<string, List<MilestoneVO>>();
        RoundResultsByPlayerId = new Dictionary<string, RoundResultVO>();
        SaveRoundResults(results);
       /* for (int i = 0; i < results.Length; i++)
        {
            foreach (PlayerVO player in RoundPlayers)
            {
                if (player.id == results[i].playerId)
                {
                    MilestonesByPlayerId[player.id] = new List<MilestoneVO>().Concat(results[i].milestones.ToList()).ToList();
                    break;
                }
            }           
        }*/
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
