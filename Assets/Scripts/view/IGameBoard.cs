using UnityEngine;
using System.Collections.Generic;

public interface IGameBoard
 {
    void InitializeBackground();
    void InitCharacters(PlayerVO[] roundMembers, PlayerVO user);
    void UpdateCharactersPositions();
    void ShowRoundPreResults(List<uint> results);
 }
