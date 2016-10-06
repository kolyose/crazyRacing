using UnityEngine;
using UnityEngine.UI;

public enum AnimationState
{
    Running
}

public class Character : MonoBehaviour {

    public const uint DEFAULT_ANIMATION_FRAMERATE = 10;

    public Material outlinedMaterial;

    public PlayerVO PlayerData {get; private set;}
    public Vector3 Size { get; private set; }
    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    private SpriteRenderer _renderer;
    private Material _defaultMaterial;
    private Text _tfNickname;
    private Animator _animator;

    void Awake()
    {
        _tfNickname = transform.GetComponentInChildren<Text>();
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _defaultMaterial = _renderer.material;
        Size = _renderer.bounds.size;
        DisplayAnimation(AnimationState.Running, false);
    }

	public void SetData(PlayerVO data)
    {
        PlayerData = data;
        _tfNickname.text = data.name;
    }

    public void DisplayOutline()
    {
        _renderer.material = outlinedMaterial;
    }

    public void DisplayAnimation(AnimationState animationState, bool value, float frameRateFactor=1.0f)
    {
        _animator.speed = frameRateFactor;
        _animator.SetBool(animationState.ToString(), value);
    }
}
