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

    private const float TIME_TO_PASS_UNIT = 1.0f;

    private Character[] _characters;
    //private float CELL_SIZE;
    private uint _characterMilestonesProcessedCounter;
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

    public Vector3 GetUserCharacterPosition()
    {
        foreach (Character character in _characters)
        {
            if (character.PlayerData.id == mainModel.User.id)
            {
                return character.Position;
            }
        }

        return Vector3.zero;
    }

    public void ProcessMilestones()
    {
        for (int i = 0; i < _characters.Length; i++)
        {
            processMilestonesByCharacter(_characters[i]);
        }
    }    

    protected void processMilestonesByCharacter(Character character)
    {
        MilestoneVO[] milestones = mainModel.RoundResultsByPlayerId[character.PlayerData.id].milestones;
        //if there is milestone available we need to process it
        if (milestones.Length > 0)
        {
            //shifting first element from the milestones array
            MilestoneVO currentMilestoneVO = milestones[0];
            int remainingLength = milestones.Length - 1;
            MilestoneVO[] remainingMilestones = new MilestoneVO[remainingLength];
            Array.Copy(milestones, 1, remainingMilestones, 0, remainingLength);
            mainModel.RoundResultsByPlayerId[character.PlayerData.id].milestones = remainingMilestones;

            //processing the element
            switch (currentMilestoneVO.type)
            {
                case MilestoneType.INIT:
                    {
                        Vector3 newPosition = currentMilestoneVO.position;
                        character.transform.position = new Vector3(newPosition.x * camera.unitSize, newPosition.y * camera.unitSize, newPosition.z);
                        DispatchUserCharacterPosition(character);
                        processMilestonesByCharacter(character);
                        break;
                    }

                case MilestoneType.MOVE:
                case MilestoneType.BOOST:
                    {
                        character.DisplayAnimation(AnimationState.Running, true, currentMilestoneVO.speed);
                        StartCoroutine(MoveCharacter(character, currentMilestoneVO));
                        break;
                    }
                case MilestoneType.BLOCK:
                    {
                        StartCoroutine(HoldCharacter(character, currentMilestoneVO));
                        break;
                    }

               default:
                    {                       
                        break;
                    }                 
            }
            
            currentMilestoneVO = null;
            return;
        }

        //if there is no milestone available - it means all character's milestones have been processed
        _characterMilestonesProcessedCounter++;
        if (_characterMilestonesProcessedCounter == _characters.Length)
        {
            _characterMilestonesProcessedCounter = 0;
            Messenger.Broadcast(ViewEvent.COMPLETE);
        }
    }

    protected IEnumerator MoveCharacter(Character character, MilestoneVO milestoneVO)
    {      
        //calculating target position and a distance to it in pixels
        Vector3 startPosition = character.Position;
        Vector3 endPosition = milestoneVO.position * camera.unitSize;
        float totalDistance = Vector3.Distance(startPosition, endPosition);
                
        //assuming we have constant time to pass one unit (let's say 1 sec for simplicity)
        //than total time to pass all distance in units is equal to a total distance in units divided by speed
        //so

        float totalTime = ((totalDistance / camera.unitSize) * TIME_TO_PASS_UNIT) / (mainModel.MovingSpeed * milestoneVO.speed);
        float startTime = Time.time;
        float timePassed = Time.time - startTime;
        //updating position smoothly
        while (totalTime > timePassed)
        {
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, timePassed/totalTime);          
            character.transform.position = newPosition;
            
            //if it is User's character recently moved we need camera to be centered at the character's position
            if (character.PlayerData.id == mainModel.User.id)
            {
                DispatchUserCharacterPosition(character);
            }

            timePassed = Time.time - startTime;
            yield return null;
        }

        character.DisplayAnimation(AnimationState.Running, false);

        //recursive call
        processMilestonesByCharacter(character);
    }

    private IEnumerator HoldCharacter(Character character, MilestoneVO milestoneVO)
    {
        float waitingTime = TIME_TO_PASS_UNIT / mainModel.MovingSpeed;
        Debug.Log("waitingTime: " + waitingTime);
        yield return new WaitForSeconds(waitingTime);

        //recursive call
        processMilestonesByCharacter(character);
    }

    private void DispatchUserCharacterPosition(Character character)
    {
        Vector3 position = character.transform.position;
        position.x += character.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        Messenger<Vector3>.Broadcast(ViewEvent.POSITION_UPDATED, position);
    }
}
