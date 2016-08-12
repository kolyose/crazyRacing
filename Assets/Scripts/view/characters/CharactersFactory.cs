using System;
using UnityEngine;

public class CharactersFactory : MonoBehaviour, ICharactersFactory
{
    public GameObject[] characterPrefabs;

    public Character GetCharacter(PlayerVO data)
    {
        GameObject characterGO = Instantiate(characterPrefabs[data.characterData.pictureId], Vector3.zero, Quaternion.identity) as GameObject;
        Character character = characterGO.GetComponent<Character>();
        character.SetData(data);
        return character;
    }
}

