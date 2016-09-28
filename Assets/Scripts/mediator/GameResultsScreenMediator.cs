using UnityEngine.UI;
using System.Collections.Generic;

public class GameResultsScreenMediator : BaseScreenMediator
{
    private Text  _tfLabel;

    protected override void Awake()
    {
        base.Awake();
        _tfLabel = transform.Find("Label").GetComponent<Text>();
        Messenger<SortedDictionary<uint, string>>.AddListener(ViewEvent.SET_GAME_RESULTS, OnSetGameResults);
    }

    public override ScreenID GetScreenID()
    {
        return ScreenID.GAME_RESULTS;
    }

    protected void OnSetGameResults(SortedDictionary<uint, string> playersByPlace)
    {
        _tfLabel.text = "Race results:\n";

        foreach (KeyValuePair<uint, string> data in playersByPlace)
        {
            string text = _tfLabel.text;
            text = text + "\n" + data.Key + " place - " + data.Value;
            _tfLabel.text = text;
        }
    }

    protected override void OnReset(ScreenID screenId)
    {
        base.OnReset(screenId);
        _tfLabel.text = "";
    }
}
