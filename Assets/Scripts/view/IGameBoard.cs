using UnityEngine;
using System.Collections.Generic;

public interface IGameBoard
 {
    void InitBackground();
    void InitTiles();
    void InitCharacters(); 
    void UpdateCharactersPositions(bool forced);
    void DisplayCharactersAnimation(AnimationState animationState, bool value);
}
