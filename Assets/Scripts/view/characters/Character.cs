using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public PlayerVO PlayerData {get; private set;}
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
  
    public Vector3 Size { get; private set; }
   
	public void SetData(PlayerVO data)
    {
        PlayerData = data;
    }

    void Awake()
    {
        Size = GetComponent<SpriteRenderer>().bounds.size;
    }
}
