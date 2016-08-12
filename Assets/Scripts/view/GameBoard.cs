using UnityEngine;
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

    private GameObject _selectActionsScreen;

    void Awake() 
    {
        if (mainModel == null) mainModel = GetComponent<MainModel>();
        if (camera == null) camera = GetComponent<PixelPerfectCamera>();
        if (charactersFactory == null) charactersFactory = GetComponent<ICharactersFactory>();          
    }

  	public void InitializeBackground() {

        CELL_SIZE = groundTiles[0].GetComponent<SpriteRenderer>().bounds.size.x * camera.Scale;

		float positionX = 0.0f;
        float positionY = 0.0f;
        
        for (int i = 0; i < mainModel.FieldWidth; i++) 
		{
            for (int j = 0; j < mainModel.FieldLength; j++)
			{				
                GameObject grassTile = groundTiles[i % 2];				
				SpriteRenderer renderer = grassTile.GetComponent<SpriteRenderer>();
                positionX = CELL_SIZE * j;
                positionY = CELL_SIZE * i;
				
				GameObject grassTileInstance = Instantiate(grassTile, new Vector3(positionX,positionY,0.0f), Quaternion.identity) as GameObject;
                grassTileInstance.transform.SetParent(viewContainer.transform.FindChild("Tiles").transform);
                grassTileInstance.transform.localScale = new Vector3(camera.Scale, camera.Scale, 1);

            }
		}
	}

    public void InitCharacters(PlayerVO[] roundMembers)
    {
        _characters = new Character[roundMembers.Length];

       for (int i = 0; i < roundMembers.Length; i++)
        {   
            Character character = charactersFactory.GetCharacter(roundMembers[i]);
            character.transform.SetParent(viewContainer.transform.FindChild("Characters").transform);
            Vector3 newPosition = mainModel.MilestonesByPlayer[roundMembers[i]][0].position;
            character.transform.position = new Vector3(newPosition.x * CELL_SIZE, newPosition.y * CELL_SIZE, newPosition.z);
            _characters[i] = character;
        }
    }

    public void UpdateCharactersPositions()
    {
        for (int i = 0; i < _characters.Length; i++)
        {         
           updateCharacterPosition(_characters[i], mainModel.MilestonesByPlayer[_characters[i].PlayerData]);                
        }
    }

    protected void updateCharacterPosition(Character character, List<MilestoneVO> milestones)
    {
        if (milestones.Count > 0)
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

    protected IEnumerator MoveToNextMilestone(Character character, List<MilestoneVO> milestones)
    {
        //shifting first elements from the list
        MilestoneVO milestoneVO = milestones[0];
        milestones.RemoveAt(0);

        //calculating target position and a distance to it
        Vector3 targetPosition = milestoneVO.position * CELL_SIZE;
        float sqrRemainingDistance = (character.Position - targetPosition).sqrMagnitude;

        //updating position smoothly
        while (Mathf.Round(sqrRemainingDistance) > 0)
        {
           Vector3 newPosition = Vector3.MoveTowards(character.Position, targetPosition, mainModel.MovingSpeed * milestoneVO.speed);          
           character.transform.position = newPosition;
           sqrRemainingDistance = (character.transform.position - targetPosition).sqrMagnitude;
           yield return null;
        }

        //recursive call
        updateCharacterPosition(character, milestones);
    }
   
    public void ShowRoundPreResults(List<uint> results)
    {
        Messenger.Broadcast(ViewEvent.COMPLETE);
    }

    public void ShowGameResults()
    {
    }
}
