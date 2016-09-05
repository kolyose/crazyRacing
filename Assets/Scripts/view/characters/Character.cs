using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    public Material outlinedMaterial;

    public PlayerVO PlayerData {get; private set;}
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }
  
    public Vector3 Size { get; private set; }

    private SpriteRenderer _renderer;
    private Material _defaultMaterial;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _defaultMaterial = _renderer.material;
        Size = _renderer.bounds.size;
    }

	public void SetData(PlayerVO data)
    {
        PlayerData = data;
    }

    public void displayOutline()
    {
        _renderer.material = outlinedMaterial;
    }
}
