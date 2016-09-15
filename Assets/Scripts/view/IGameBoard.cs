using UnityEngine;
using System.Collections.Generic;

public interface IGameBoard
 {
    void InitBackground();
    void InitCharacters(); 
    void UpdateCharactersPositions(bool forced);
}
