using System;
using UnityEngine;

public interface ICharactersFactory
{
    Character GetCharacter(PlayerVO data);
}
