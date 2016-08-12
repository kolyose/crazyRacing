public interface IServerDataParser
 {
    PlayerVO[] GetPlayersData(string rawData);
    RoundResultVO[] GetRoundResultsData(string rawData);
}