using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MainModel : MonoBehaviour
{
    //TODO: make following parameters to be set up in Settings class
    public uint FieldLength = 10;
    public uint FieldWidth = 6;
    public float MovingSpeed = 1.0f;

    public PlayerVO User {get; set; }
    public PlayerVO[] RoundPlayers {get; private set;}

    public Dictionary<PlayerVO, List<MilestoneVO>> MilestonesByPlayer { get; set; }

    private RoundResultVO[] _roundResults;


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
        MilestonesByPlayer = new Dictionary<PlayerVO, List<MilestoneVO>>();

        for (int i = 0; i < results.Length; i++)
        {
            foreach (PlayerVO player in RoundPlayers)
            {
                if (player.id == results[i].playerId)
                {
                   // MilestonesByPlayer[player] = new List<MilestoneVO>();
                    MilestonesByPlayer[player] = new List<MilestoneVO>().Concat(results[i].milestones.ToList()).ToList();
                    break;
                }
            }           
        }
    }

    public void SaveRoundResults(RoundResultVO[] results)
    {
        _roundResults = results;

       foreach (RoundResultVO result in _roundResults)
        {
           for (int i=0; i<RoundPlayers.Length; i++)
           {
                if (result.playerId == RoundPlayers[i].id)
                {
                   MilestonesByPlayer[RoundPlayers[i]] = MilestonesByPlayer[RoundPlayers[i]].Concat(result.milestones.ToList()).ToList();                    
                   break;
                }
           }
        }

        /*for (int i = 0; i < RoundPlayers.Length; i++)
        {    
            //setting fake coordinates jsut for test
            //TODO add real values here after parsing
            MilestonesByPlayer[RoundPlayers[i]].Add(new MilestoneVO(new Vector3(i, i, 0), 1));
        }*/
    }

    public List<uint> GetRoundPreResults() //TODO: change type to real one
    {
        return null;//TODO: add functionality
    }

    public CharacterVO GetActiveCharacterVO()
    {
        //TODO: remove hardcode
        CharacterVO characterVO = new CharacterVO();
        characterVO.pictureId = 1;
        return characterVO;

        //return _playerVO.ActiveCharacterVO;
    }

    private Vector2[] ParseCharactersPositions(string data)
    {
        return null; //TODO: add functionality
    }
}
