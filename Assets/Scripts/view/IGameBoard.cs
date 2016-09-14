using UnityEngine;
using System.Collections.Generic;

public interface IGameBoard
 {
    void InitializeBackground();
    void UpdateCharactersPositions(PlayerVO[] roundMembers, PlayerVO user);
    void UpdateCharactersPositions();
 }
