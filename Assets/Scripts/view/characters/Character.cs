using UnityEngine;
using UnityEngine.UI;
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
    private Text _tfNickname;

    void Awake()
    {
        _tfNickname = transform.GetComponentInChildren<Text>();
        _renderer = GetComponent<SpriteRenderer>();
        _defaultMaterial = _renderer.material;
        Size = _renderer.bounds.size;
    }

	public void SetData(PlayerVO data)
    {
        PlayerData = data;
        _tfNickname.text = data.name;
    }

    public void displayOutline()
    {
        _renderer.material = outlinedMaterial;
    }
}
