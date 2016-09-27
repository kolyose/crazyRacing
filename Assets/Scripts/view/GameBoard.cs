using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour, IGameBoard {
       
    [HideInInspector]
    public MainModel mainModel;
    [HideInInspector]
    public ICharactersFactory charactersFactory;
    [HideInInspector]
    public PixelPerfectCamera camera;

    public GameObject viewContainer;	
    public GameObject[] groundTiles;
    public GameObject tribunesTile;
    public GameObject finishTile;

    private Character[] _characters;
    //private float CELL_SIZE;
    private uint _charactersPositionsUpdated;
    private Transform _charactersContainer;
    private Transform _tilesContainer;
    private Transform _backgroundContainer;

    void Awake() 
    {
        if (mainModel == null) mainModel = GetComponent<MainModel>();
        if (camera == null) camera = GetComponent<PixelPerfectCamera>();
        if (charactersFactory == null) charactersFactory = GetComponent<ICharactersFactory>();

        _charactersContainer = viewContainer.transform.FindChild("Characters").transform;
        _tilesContainer = viewContainer.transform.FindChild("Tiles").transform;
        _backgroundContainer = viewContainer.transform.FindChild("Background").transform;
    }

    public void InitTiles()
    {
        float positionX = 0.0f;
        float positionY = 0.0f;

        for (int i = 0; i < mainModel.GameSettings.fieldWidth; i++)
        {
            for (int j = 0; j < mainModel.GameSettings.fieldLength; j++)
            {
                GameObject grassTile = groundTiles[i % 2];
                SpriteRenderer renderer = grassTile.GetComponent<SpriteRenderer>();
                positionX = camera.unitSize * j;
                positionY = camera.unitSize * i;

                GameObject grassTileInstance = Instantiate(grassTile, new Vector3(positionX, positionY, 0.0f), Quaternion.identity) as GameObject;
                grassTileInstance.transform.SetParent(_tilesContainer);
                grassTileInstance.transform.localScale = new Vector3(camera.Scale, camera.Scale, 1);
            }
        }

        positionY = camera.unitSize * mainModel.GameSettings.fieldWidth - camera.unitSize/4;
        GameObject finishTileInstance = Instantiate(finishTile, new Vector3(positionX, positionY, 0.0f), Quaternion.identity) as GameObject;
        finishTileInstance.transform.SetParent(_tilesContainer);
    }

  	public void InitBackground() 
    {
        float positionX = 0.0f;
        float positionY = 0.0f;
        float tribunesPixelWidth = camera.orthographicSize * 2 - mainModel.GameSettings.fieldWidth * camera.unitSize;
        
        while (true)
        {
            while (true)
            {
                GameObject tribunesTileInstance = Instantiate(tribunesTile, new Vector3(positionX, positionY, 0), Quaternion.identity) as GameObject;
                tribunesTileInstance.transform.SetParent(_backgroundContainer);
                tribunesTileInstance.transform.localScale = new Vector3(camera.Scale, camera.Scale, 1);
                positionX += tribunesTile.GetComponent<SpriteRenderer>().bounds.size.x;

                if (positionX > mainModel.GameSettings.fieldLength * camera.unitSize)
                {
                    break;
                }
            }
                 
            if (positionY < tribunesPixelWidth)
            {
                positionX = 0;      
                positionY += tribunesTile.GetComponent<SpriteRenderer>().bounds.size.y;        
            }
            else
            {
                _backgroundContainer.position = new Vector3(_backgroundContainer.localPosition.x, mainModel.GameSettings.fieldWidth * camera.unitSize, _backgroundContainer.localPosition.z);
                break;  
            }
        }       
	}

    public void InitCharacters()
    {
        int length = mainModel.RoundPlayers.Length;
        _characters = new Character[length];

        for (int i = 0; i < length; i++)
        {
            Character character = charactersFactory.GetCharacter(mainModel.RoundPlayers[i]);
            character.transform.SetParent(_charactersContainer);
            character.transform.position = new Vector3(-camera.unitSize, -camera.unitSize, 0);
            _characters[i] = character;

            if (mainModel.RoundPlayers[i].id == mainModel.User.id)
            {
                character.DisplayOutline();
            }
        }
    }
    
    public void DisplayCharactersAnimation(AnimationState animationState, bool value)
    {
        foreach (Character character in _characters)
        {
            character.DisplayAnimation(animationState, value);
        }
    }

    public void UpdateCharactersPositions(bool forced)
    {
        if (forced)
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                Vector3 newPosition = mainModel.RoundResultsByPlayerId[_characters[i].PlayerData.id].milestones[0].position;
                _characters[i].transform.position = new Vector3(newPosition.x * camera.unitSize, newPosition.y * camera.unitSize, newPosition.z);
            }

            Messenger.Broadcast(ViewEvent.COMPLETE);
        }
        else
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                updateCharacterPosition(_characters[i], mainModel.RoundResultsByPlayerId[_characters[i].PlayerData.id].milestones);
            }
        }
    }    

    protected void updateCharacterPosition(Character character, MilestoneVO[] milestones)
    {
        if (milestones.Length > 0)
        {
            StartCoroutine(MoveToNextMilestone(character, milestones));
            return;
        }

        _charactersPositionsUpdated++;
        if (_charactersPositionsUpdated == _characters.Length)
        {
            _charactersPositionsUpdated = 0;
            Messenger.Broadcast(ViewEvent.COMPLETE);
        }
    }

    protected IEnumerator MoveToNextMilestone(Character character, MilestoneVO[] milestones)//List<MilestoneVO> milestones)
    {
        //shifting first elements from the list
        MilestoneVO milestoneVO = milestones[0];
        //milestones.RemoveAt(0);
        int remainingLength = milestones.Length - 1;
        MilestoneVO[] remainingMilestones = new MilestoneVO[remainingLength];
        Array.Copy(milestones, 1, remainingMilestones, 0, remainingLength);

        //calculating target position and a distance to it
        Vector3 targetPosition = milestoneVO.position * camera.unitSize;
        float remainingDistance = (character.Position - targetPosition).magnitude;

        //updating position smoothly
        while (Mathf.Round(remainingDistance) > 0)
        {
            Vector3 newPosition = Vector3.MoveTowards(character.Position, targetPosition, mainModel.MovingSpeed * milestoneVO.speed);          
            character.transform.position = newPosition;
            remainingDistance = (character.Position - targetPosition).magnitude;
            
            if (character.PlayerData.id == mainModel.User.id)
            {
                Vector3 position = character.transform.position;
                position.x += character.GetComponent<SpriteRenderer>().bounds.size.x / 2;
                Messenger<Vector3>.Broadcast(ViewEvent.POSITION_UPDATED, position);
            }

            yield return null;
        }

        //recursive call
        updateCharacterPosition(character, remainingMilestones);
    }
}
