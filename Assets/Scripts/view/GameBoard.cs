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

    private Character[] _characters;
    private float CELL_SIZE;
    private uint _charactersPositionsUpdated;
    private Transform _charactersContainer;
    private Transform _tilesContainer;
    private GameObject _selectActionsScreen;

    void Awake() 
    {
        if (mainModel == null) mainModel = GetComponent<MainModel>();
        if (camera == null) camera = GetComponent<PixelPerfectCamera>();
        if (charactersFactory == null) charactersFactory = GetComponent<ICharactersFactory>();

        _charactersContainer = viewContainer.transform.FindChild("Characters").transform;
        _tilesContainer = viewContainer.transform.FindChild("Tiles").transform;
    }

  	public void InitBackground() {

        CELL_SIZE = groundTiles[0].GetComponent<SpriteRenderer>().bounds.size.x * camera.Scale;

		float positionX = 0.0f;
        float positionY = 0.0f;
        
        for (int i = 0; i < mainModel.GameSettings.fieldWidth; i++) 
		{
            for (int j = 0; j < mainModel.GameSettings.fieldLength; j++)
			{				
                GameObject grassTile = groundTiles[i % 2];				
				SpriteRenderer renderer = grassTile.GetComponent<SpriteRenderer>();
                positionX = CELL_SIZE * j;
                positionY = CELL_SIZE * i;
				
				GameObject grassTileInstance = Instantiate(grassTile, new Vector3(positionX,positionY,0.0f), Quaternion.identity) as GameObject;
                grassTileInstance.transform.SetParent(_tilesContainer);
                grassTileInstance.transform.localScale = new Vector3(camera.Scale, camera.Scale, 1);

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
            _characters[i] = character;

            if (mainModel.RoundPlayers[i].id == mainModel.User.id)
            {
                character.displayOutline();
            }
        }
    }
    
    public void UpdateCharactersPositions(bool forced)
    {
        if (forced)
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                Vector3 newPosition = mainModel.RoundResultsByPlayerId[_characters[i].PlayerData.id].milestones[0].position;
                _characters[i].transform.position = new Vector3(newPosition.x * CELL_SIZE, newPosition.y * CELL_SIZE, newPosition.z);
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
        Vector3 targetPosition = milestoneVO.position * CELL_SIZE;
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
