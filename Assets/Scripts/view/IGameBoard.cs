using UnityEngine;
using System.Collections.Generic;

public interface IGameBoard
 {
    void InitBackground();
    void InitTiles();
    void InitCharacters(); 
    void ProcessMilestones();
    void DisplayCharactersAnimation(AnimationState animationState, bool value);
    Vector3 GetUserCharacterPosition();
}
