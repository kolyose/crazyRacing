using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    public AudioSource _audioSource;
    public AudioClip backgroundClip;
    public AudioClip countdownClip;
    public AudioClip runningClip;

	// Use this for initialization
	void Awake () 
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
	}

    public void PlayBackground()
    {
        PlaySound(backgroundClip);
    }

    public void PlayCountdown()
    {
        PlaySound(countdownClip);
    }

    public void PlayRunning()
    {
        PlaySound(runningClip);
    }

    public void StopSound()
    {
        _audioSource.Stop();
    }

	private void PlaySound(AudioClip clip)
    {       
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
