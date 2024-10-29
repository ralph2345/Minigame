using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameAudio : MonoBehaviour
{
    public static MinigameAudio Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clip Source")]
    public AudioClip minigameMusic;
    public AudioClip cardClickSound;
    public AudioClip cardMatchSound;
    public AudioClip cardMismatchSound;
    public AudioClip victorySound;
    public AudioClip gameOverSound;
    public AudioClip pauseSound;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();  
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource == null || minigameMusic == null)
        {
            Debug.LogError("Music Source or Minigame Music AudioClip is not assigned!");
            return;
        }

        musicSource.clip = minigameMusic;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log("Background music is playing.");
    }

   
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            Debug.LogError("SFX Source or AudioClip is not assigned!");
            return;
        }

        sfxSource.PlayOneShot(clip);  
    }

   
    public void PlayCardClickSound()
    {
        PlaySFX(cardClickSound);
    }

   
    public void PlayCardMatchSound()
    {
        PlaySFX(cardMatchSound);
    }

    public void PlayCardMismatchSound()
    {
        PlaySFX(cardMismatchSound);
    }

    public void PlayVictorySound()
    {
        PlaySFX(victorySound);
    }

    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
    }

    public void PlayPauseSound()
    {
        PlaySFX(pauseSound);
    }
}
