public interface IServerDataParser
 {
    SettingsVO GetSettingsData(string rawData);
    PlayerVO[] GetPlayersData(string rawData);
    RoundResultVO[] GetRoundResultsData(string rawData);
}